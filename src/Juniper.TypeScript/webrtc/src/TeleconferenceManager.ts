import {
    HttpTransportType,
    HubConnection,
    HubConnectionBuilder,
    HubConnectionState
} from "@microsoft/signalr";
import type { AudioManager, DeviceManagerAudioInputChangedEvent } from "juniper-audio";
import { MediaStreamSource, removeVertex } from "juniper-audio";
import type { IDisposable } from "juniper-tslib";
import { assertNever, PointerName, TypedEventBase } from "juniper-tslib";
import adapter from 'webrtc-adapter';
import {
    ConferenceErrorEvent,
    ConferenceEvents,
    ConferenceEventTypes,
    ConferenceServerConnectedEvent,
    ConferenceServerDisconnectedEvent,
    RoomJoinedEvent,
    RoomLeftEvent,
    StreamType,
    UserAudioMutedEvent,
    UserAudioStreamAddedEvent,
    UserAudioStreamRemovedEvent,
    UserChatEvent,
    UserJoinedEvent,
    UserLeftEvent,
    UserPoseEvent,
    UserVideoMutedEvent,
    UserVideoStreamAddedEvent,
    UserVideoStreamRemovedEvent
} from "./ConferenceEvents";
import { ConnectionState, settleConnected, whenDisconnected } from "./ConnectionState";
import { DecayingGain } from "./DecayingGain";
import {
    RemoteUser,
    RemoteUserAnswerEvent,
    RemoteUserIceCandidateEvent,
    RemoteUserIceErrorEvent,
    RemoteUserOfferEvent,
    RemoteUserStreamNeededEvent,
    RemoteUserTrackAddedEvent,
    RemoteUserTrackMutedEvent,
    RemoteUserTrackRemovedEvent
} from "./RemoteUser";

export const DEFAULT_LOCAL_USER_ID = "local-user";

let loggingEnabled = window.location.hostname === "localhost"
    || /\bdebug\b/.test(window.location.search);

const sockets = new Array<WebSocket>();
const fakeSocket = function (...args: any[]): WebSocket {
    console.log("New connection", ...args);
    const socket = new (WebSocket as any)(...args);
    sockets.push(socket);
    return socket;
};

fakeSocket.CLOSED = WebSocket.CLOSED;
fakeSocket.CLOSING = WebSocket.CLOSING;
fakeSocket.CONNECTING = WebSocket.CONNECTING;
fakeSocket.OPEN = WebSocket.OPEN;

Object.assign(window, {
    sockets: {
        list: () => {
            return sockets;
        },
        kill: () => {
            for (const socket of sockets) {
                socket.close();
            }
        }
    }
});

export enum ClientState {
    InConference = "in-conference",
    JoiningConference = "joining-conference",
    Connected = "connected",
    Connecting = "connecting",
    Prepaired = "prepaired",
    Prepairing = "prepairing",
    Unprepared = "unprepaired"
}

export class TeleconferenceManager
    extends TypedEventBase<ConferenceEvents>
    implements IDisposable {

    toggleLogging() {
        loggingEnabled = !loggingEnabled;
    }

    private _isAudioMuted: boolean = null;
    get isAudioMuted() { return this._isAudioMuted; }

    private _isVideoMuted: boolean = null;
    get isVideoMuted() { return this._isVideoMuted; }

    private _localUserID: string = DEFAULT_LOCAL_USER_ID;
    get localUserID() { return this._localUserID; }

    private _localUserName: string = null;
    get localUserName() { return this._localUserName; }

    private _roomName: string = null;
    get roomName() { return this._roomName; }

    private _conferenceState = ConnectionState.Disconnected;

    private _hasAudioPermission = false;
    get hasAudioPermission() { return this._hasAudioPermission; }

    private _hasVideoPermission = false;
    get hasVideoPermission() { return this._hasVideoPermission; }



    private lastRoom: string = null;
    private lastUserID: string = null;

    private users = new Map<string, RemoteUser>();

    private localStreamIn: MediaStreamAudioSourceNode = null;

    private readonly hub: HubConnection;
    private readonly remoteGainDecay: DecayingGain;

    private _ready: Promise<void> = null;
    public get ready(): Promise<void> {
        if (this._ready === null) {
            this._ready = this.startInternal();
        }

        return this._ready;
    }

    constructor(
        public readonly audio: AudioManager,
        private readonly signalRPath: string,
        private readonly autoSetPosition = true,
        public readonly needsVideoDevice = false) {
        super();

        if (loggingEnabled) {
            console.log(adapter);
        }

        let hubBuilder = new HubConnectionBuilder()
            .withAutomaticReconnect();

        hubBuilder.withUrl(this.signalRPath, HttpTransportType.WebSockets);

        this.hub = hubBuilder.build();

        this.audio.devices.addEventListener("audioinputchanged", (evt) =>
            this.onAudioInputChanged(evt));

        Object.assign(window, {
            users: this.users
        });

        this.hub.onclose(() => {
            this.lastRoom = null;
            this.lastUserID = null;
            this.setConferenceState(ConnectionState.Disconnected);
        });

        this.hub.onreconnecting((err) => {
            this.dispatchEvent(new ConferenceErrorEvent(err));
        });

        this.hub.onreconnected(async () => {
            this.lastRoom = null;
            this.lastUserID = null;
            this.setConferenceState(ConnectionState.Disconnected);
            await this.identify(this.localUserName);
            await this.join(this.roomName);
        });

        this.hub.on("userJoined", this.onUserJoined.bind(this));
        this.hub.on("iceReceived", this.onIceReceived.bind(this));
        this.hub.on("offerReceived", this.onOfferReceived.bind(this));
        this.hub.on("answerReceived", this.onAnswerReceived.bind(this));
        this.hub.on("userLeft", (fromUserID: string) => {
            const user = this.users.get(fromUserID);
            if (user) {
                this.onUserLeft(user);
            }
        });

        this.hub.on("userPosed",
            (fromUserID: string, px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number, height: number) => {
                const user = this.users.get(fromUserID);
                if (user) {
                    user.recvPose(
                        px, py, pz,
                        fx, fy, fz,
                        ux, uy, uz,
                        height);
                }
            });

        this.hub.on("userPointer",
            (fromUserID: string, name: PointerName, px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number) => {
                const user = this.users.get(fromUserID);
                if (user) {
                    user.recvPointer(
                        name,
                        px, py, pz,
                        fx, fy, fz,
                        ux, uy, uz);
                }
            });

        this.hub.on("chat",
            (fromUserID: string, text: string) => {
                const user = this.users.get(fromUserID);
                if (user) {
                    this.dispatchEvent(new UserChatEvent(user, text));
                }
            });

        this.remoteGainDecay = new DecayingGain(
            this.audio.audioCtx,
            this.audio.audioDestination.remoteUserInput,
            this.audio.localAutoControlledGain,
            0.05,
            1,
            0.25,
            0,
            250,
            0.25,
            1000,
            250
        );

        this.remoteGainDecay.setEnabled(!this.audio.useHeadphones);
        this.audio.addEventListener("useheadphonestoggled", () => {
            this.remoteGainDecay.setEnabled(!this.audio.useHeadphones);
            this.restartStream();
        });

        const onWindowClosed = () => {
            if (this.conferenceState === ConnectionState.Connected) {
                this.toRoom("leave");
            }
        };
        window.addEventListener("beforeunload", onWindowClosed);
        window.addEventListener("unload", onWindowClosed);
        window.addEventListener("pagehide", onWindowClosed);

        this.localStream = this.audio.devices.currentStream;

        Object.seal(this);
    }

    private async startInternal(): Promise<void> {
        await this.audio.ready;
        await this.audio.devices.startPreferredAudioInput();
        this.localStream = this.audio.devices.currentStream;
    }

    get connectionState() {
        switch (this.hub.state) {
            case HubConnectionState.Connected: return ConnectionState.Connected;
            case HubConnectionState.Connecting: case HubConnectionState.Reconnecting: return ConnectionState.Connecting;
            case HubConnectionState.Disconnected: return ConnectionState.Disconnected;
            case HubConnectionState.Disconnecting: return ConnectionState.Disconnecting;
            default: assertNever(this.hub.state);
        }
    }

    get echoControl() {
        return this.remoteGainDecay;
    }

    get isConnected(): boolean {
        return this.connectionState === ConnectionState.Connected;
    }

    get conferenceState(): ConnectionState {
        return this._conferenceState;
    }

    get isConferenced(): boolean {
        return this.conferenceState === ConnectionState.Connected;
    }

    protected setConferenceState(state: ConnectionState): void {
        this._conferenceState = state;
    }

    private disposed = false;
    dispose(): void {
        if (!this.disposed) {
            this.leave();
            this.disconnect();
            this.disposed = true;
        }
    }

    private err(source: string, ...msg: any[]): void {
        console.warn(source, ...msg);
    }

    private async toServer<T>(method: string, ...rest: any[]): Promise<T> {
        return await this.hub.invoke(method, ...rest);
    }

    private async toRoom(method: string, ...rest: any[]) {
        if (this.isConnected) {
            await this.hub.invoke(method, this.localUserID, this.roomName, ...rest);
        }
    }

    private async toUser(method: string, toUserID: string, ...rest: any[]) {
        if (this.isConnected) {
            await this.hub.invoke(method, this.localUserID, toUserID, ...rest);
        }
    }

    async connect(): Promise<void> {
        await this.ready;
        await whenDisconnected("Connecting", () => this.connectionState, async () => {
            await this.hub.start();
            this.dispatchEvent(new ConferenceServerConnectedEvent());
        });
    }

    async join(roomName: string): Promise<void> {
        if (this.lastRoom !== null && roomName !== this.lastRoom) {
            await this.leave();
        }

        await whenDisconnected(`Joining room ${roomName}`, () => this.conferenceState, async () => {
            this.setConferenceState(ConnectionState.Connecting);

            this._roomName = roomName;
            await this.toServer("join", this.roomName);

            this.lastRoom = this.roomName;

            this.setConferenceState(ConnectionState.Connected);

            const destination = this.audio.setLocalUserID(this.localUserID);
            this.dispatchEvent(new RoomJoinedEvent(this.localUserID, destination.pose));

            await this.toRoom("greetEveryone", this.localUserName);
        });
    }

    async identify(userName: string): Promise<void> {
        if (this.localUserID === DEFAULT_LOCAL_USER_ID) {
            this._localUserID = null;
        }

        this._localUserID = this.localUserID || await this.toServer<string>("getNewUserID");
        this._localUserName = userName;

        if (this.localUserID
            && this.localUserID !== this.lastUserID
            && this.isConnected) {
            this.lastUserID = this.localUserID;
            await this.toServer("identify", this.localUserID);
        }
    }

    async leave(): Promise<void> {
        await settleConnected(`Leaving room ${this.lastRoom}`, () => this.conferenceState, async () => {
            this.setConferenceState(ConnectionState.Disconnecting);
            await this.toRoom("leave");
        });

        for (const user of this.users.values()) {
            this.onUserLeft(user);
        }

        this._roomName
            = this.lastRoom
            = null;

        this.setConferenceState(ConnectionState.Disconnected);
        this.dispatchEvent(new RoomLeftEvent(this.localUserID));
    }

    async disconnect(): Promise<void> {
        if (this.conferenceState !== ConnectionState.Disconnected) {
            await this.leave();
        }

        await settleConnected("Disconnecting", () => this.connectionState, async () => {
            await this.hub.stop();
            this._localUserID
                = this.lastUserID
                = DEFAULT_LOCAL_USER_ID;
            this.audio.setLocalUserID(this.localUserID);
            this.dispatchEvent(new ConferenceServerDisconnectedEvent());
        });
    }

    private async onUserJoined(fromUserID: string, fromUserName: string) {
        if (this.users.has(fromUserID)) {
            const user = this.users.get(fromUserID);
            user.start();
        }
        else {
            if (this.users.size === 0) {
                this.remoteGainDecay.start();
            }

            const rtcConfig = await this.getRTCConfiguration();
            const user = new RemoteUser(fromUserID, fromUserName, rtcConfig, true);
            this.users.set(user.userID, user);

            user.addEventListener("iceError", this.onIceError.bind(this));
            user.addEventListener("iceCandidate", this.onIceCandidate.bind(this));
            user.addEventListener("offer", this.onOfferCreated.bind(this));
            user.addEventListener("answer", this.onAnswerCreated.bind(this));
            user.addEventListener("streamNeeded", this.onStreamNeeded.bind(this));
            user.addEventListener("trackAdded", this.onTrackAdded.bind(this));
            user.addEventListener("trackMuted", this.onTrackMuted.bind(this));
            user.addEventListener("trackRemoved", this.onTrackRemoved.bind(this));
            user.addEventListener("userLeft", (evt) => this.onUserLeft(evt.user));
            user.addEventListener("userPosed", (evt) => {
                this.onRemoteUserPosed(evt);
                const { p, f, u } = evt.pose;
                this.audio.setUserPose(
                    evt.user.userID,
                    p[0], p[1], p[2],
                    f[0], f[1], f[2],
                    u[0], u[1], u[2]);
            });
            user.addEventListener("userPointer", (evt) => this.onRemoteUserPosed(evt));

            this.toUser("greet", user.userID, this.localUserName);

            const source = this.audio.createUser(user.userID, user.userName);

            this.dispatchEvent(new UserJoinedEvent(user, source));
        }
    }

    private async getRTCConfiguration(): Promise<RTCConfiguration> {
        return await this.toServer<RTCConfiguration>("getRTCConfiguration", this.localUserID);
    }

    private onIceError(evt: RemoteUserIceErrorEvent) {
        this.err("icecandidateerror", this.localUserName, evt.user.userName, `${evt.url} [${evt.errorCode}]: ${evt.errorText}`);
    }

    private async onIceCandidate(evt: RemoteUserIceCandidateEvent) {
        await this.sendIce(evt.user.userID, evt.candidate);
    }

    private async onIceReceived(fromUserID: string, candidateJSON: string) {
        try {
            const user = this.users.get(fromUserID);
            if (user) {
                const ice = JSON.parse(candidateJSON) as RTCIceCandidate;
                await user.addIceCandidate(ice);
            }
        }
        catch (exp) {
            this.err("iceReceived", `${exp.message} [${fromUserID}] (${candidateJSON})`);
        }
    }

    private async onOfferCreated(evt: RemoteUserOfferEvent) {
        await this.sendOffer(evt.user.userID, evt.offer);
    }

    private async onOfferReceived(fromUserID: string, offerJSON: string) {
        try {
            const user = this.users.get(fromUserID);
            if (user) {
                const offer = JSON.parse(offerJSON) as RTCSessionDescription;
                await user.acceptOffer(offer);
            }
        }
        catch (exp) {
            this.err("offerReceived", exp.message);
        }
    }

    private async onAnswerCreated(evt: RemoteUserAnswerEvent) {
        await this.sendAnswer(evt.user.userID, evt.answer);
    }

    private async onAnswerReceived(fromUserID: string, answerJSON: string) {
        try {
            const user = this.users.get(fromUserID);
            if (user) {
                const answer = JSON.parse(answerJSON) as RTCSessionDescription;
                await user.acceptAnswer(answer);
            }
        }
        catch (exp) {
            this.err("answerReceived", exp.message);
        }
    }

    private onStreamNeeded(evt: RemoteUserStreamNeededEvent) {
        evt.user.sendStream(this.audio.output.stream);
    }

    private onTrackAdded(evt: RemoteUserTrackAddedEvent) {
        if (evt.track.kind === "audio") {
            this.audio.setUserStream(evt.user.userID, evt.user.userName, evt.stream);
            this.dispatchEvent(new UserAudioStreamAddedEvent(evt.user, evt.stream));
        }
        else if (evt.track.kind === "video") {
            this.dispatchEvent(new UserVideoStreamAddedEvent(evt.user, evt.stream));
        }
    }

    private onTrackMuted(evt: RemoteUserTrackMutedEvent) {
        if (evt.track.kind === "audio") {
            this.dispatchEvent(new UserAudioMutedEvent(evt.user, evt.track.muted));
        }
        else if (evt.track.kind === "video") {
            this.dispatchEvent(new UserVideoMutedEvent(evt.user, evt.track.muted));
        }
    }

    private onTrackRemoved(evt: RemoteUserTrackRemovedEvent) {
        if (evt.track.kind === "audio") {
            this.audio.setUserStream(evt.user.userID, evt.user.userName, null);
            this.dispatchEvent(new UserAudioStreamRemovedEvent(evt.user, evt.stream));
        }
        else if (evt.track.kind === "video") {
            this.dispatchEvent(new UserVideoStreamRemovedEvent(evt.user, evt.stream));
        }
    }

    private onUserLeft(user: RemoteUser) {
        user.dispose();
        this.users.delete(user.userID);
        if (this.users.size === 0) {
            this.remoteGainDecay.stop();
        }

        this.audio.removeUser(user.userID);
        this.dispatchEvent(new UserLeftEvent(user));
    }

    private async sendIce(toUserID: string, candidate: RTCIceCandidate): Promise<void> {
        await this.toUser("sendIce", toUserID, JSON.stringify(candidate));
    }

    private async sendOffer(toUserID: string, offer: RTCSessionDescription): Promise<void> {
        await this.toUser("sendOffer", toUserID, JSON.stringify(offer));
    }

    private async sendAnswer(toUserID: string, answer: RTCSessionDescription): Promise<void> {
        await this.toUser("sendAnswer", toUserID, JSON.stringify(answer));
    }

    async setLocalPose(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number, height: number): Promise<void> {
        if (this.autoSetPosition) {
            this.audio.setUserPose(this.localUserID, px, py, pz, fx, fy, fz, ux, uy, uz);
        }

        if (this.conferenceState === ConnectionState.Connected) {
            await Promise.all(
                Array.from(this.users.values())
                    .map(user =>
                        user.sendPose(px, py, pz, fx, fy, fz, ux, uy, uz, height)));
        }
    }

    async setLocalPointer(name: PointerName, px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): Promise<void> {
        if (this.conferenceState === ConnectionState.Connected) {
            await Promise.all(
                Array.from(this.users.values())
                    .map(user =>
                        user.sendPointer(name, px, py, pz, fx, fy, fz, ux, uy, uz)));
        }
    }

    private onRemoteUserPosed<T extends ConferenceEventTypes>(evt: UserPoseEvent<T>): void {
        const offset = this.audio.getUserOffset(evt.user.userID);
        if (offset) {
            evt.pose.setOffset(offset[0], offset[1], offset[2]);
        }
    };

    chat(text: string): void {
        if (this.conferenceState === ConnectionState.Connected) {
            this.toRoom("chat", text);
        }
    }

    userExists(id: string): boolean {
        return this.users.has(id);
    }

    getUserNames(): [string, string][] {
        return Array.from(this.users.values())
            .map(u => [u.userID, u.userName]);
    }

    protected async onAudioInputChanged(evt: DeviceManagerAudioInputChangedEvent): Promise<void> {
        const deviceId = evt.audio && evt.audio.deviceId;

        await this.startStream(
            deviceId,
            this.audio.useHeadphones);
    }

    private async startStream(deviceId: string, usingHeadphones: boolean) {
        if (!deviceId) {
            this.localStream = null;
        }
        else {
            this.localStream = await navigator.mediaDevices.getUserMedia({
                audio: {
                    deviceId,
                    echoCancellation: !usingHeadphones,
                    autoGainControl: true,
                    noiseSuppression: true
                }
            });
        }
    }

    private async restartStream() {
        this.localStream = null;
        await this.startStream(
            this.audio.devices.preferredAudioOutputID,
            this.audio.useHeadphones);
    }

    get localStream(): MediaStream {
        return this.localStreamIn && this.localStreamIn.mediaStream || null;
    }

    set localStream(v: MediaStream) {
        if (v !== this.localStream) {
            if (this.localStreamIn) {
                removeVertex(this.localStreamIn);
                this.localStreamIn = null;
            }

            this.audio.devices.currentStream = v;

            if (v) {
                this.localStreamIn = MediaStreamSource(
                    "local-mic",
                    this.audio.audioCtx,
                    v,
                    this.audio);
            }
        }
    }

    private isMediaMuted(type: StreamType) {
        for (const track of this.audio.output.stream.getTracks()) {
            if (track.kind === type) {
                return !track.enabled;
            }
        }

        return true;
    }

    private setMediaMuted(type: StreamType, muted: boolean) {
        for (const track of this.audio.output.stream.getTracks()) {
            if (track.kind === type) {
                track.enabled = !muted;
            }
        }
    }

    get audioMuted(): boolean {
        return this.isMediaMuted(StreamType.Audio);
    }

    set audioMuted(muted: boolean) {
        this.setMediaMuted(StreamType.Audio, muted);
    }

    get videoMuted(): boolean {
        return this.isMediaMuted(StreamType.Video);
    }

    set videoMuted(muted: boolean) {
        this.setMediaMuted(StreamType.Video, muted);
    }
}