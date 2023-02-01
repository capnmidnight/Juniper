import { AudioManager } from "@juniper-lib/audio/AudioManager";
import { DeviceManager } from "@juniper-lib/audio/DeviceManager";
import { LocalUserMicrophone } from "@juniper-lib/audio/LocalUserMicrophone";
import { StreamChangedEvent } from "@juniper-lib/audio/StreamChangedEvent";
import { TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { PointerID } from "@juniper-lib/tslib/events/Pointers";
import { WindowQuitEventer } from "@juniper-lib/tslib/events/WindowQuitEventer";
import { singleton } from "@juniper-lib/tslib/singleton";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { IDisposable } from "@juniper-lib/tslib/using";
import "webrtc-adapter";
import {
    ConferenceErrorEvent,
    ConferenceEvents, ConferenceServerConnectedEvent,
    ConferenceServerDisconnectedEvent,
    RoomJoinedEvent,
    RoomLeftEvent, UserChatEvent,
    UserJoinedEvent,
    UserLeftEvent
} from "./ConferenceEvents";
import { ConnectionState, settleConnected, whenDisconnected } from "./ConnectionState";
import { DEFAULT_LOCAL_USER_ID } from "./constants";
import { GainDecayer } from "./GainDecayer";
import {
    HubAnswerReceivedEvent,
    HubIceReceivedEvent,
    HubOfferReceivedEvent,
    HubReconnectingEvent,
    HubUserChatEvent,
    HubUserJoinedEvent,
    HubUserLeftEvent,
    HubUserPointerEvent,
    HubUserPosedEvent,
    IHub
} from "./IHub";
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

const sockets = singleton("Juniper:Sockets", () => new Array<WebSocket>());
function fakeSocket(...args: any[]): WebSocket {
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

    private readonly remoteGainDecay: GainDecayer;

    private readonly windowQuitter = new WindowQuitEventer();

    constructor(
        public readonly audio: AudioManager,
        private readonly microphones: LocalUserMicrophone,
        private readonly devices: DeviceManager,
        private readonly hub: IHub) {
        super();

        const onStreamChanged = (evt: StreamChangedEvent) => {
            for (const user of this.users.values()) {
                if (evt.oldStream) {
                    user.removeStream(evt.oldStream);
                }
                if (evt.newStream) {
                    user.sendStream(evt.newStream);
                }
            }
        };

        this.devices.addEventListener("streamchanged", onStreamChanged);

        this.hub.addEventListener("close", this.onClose.bind(this));
        this.hub.addEventListener("reconnecting", this.onReconnecting.bind(this));
        this.hub.addEventListener("reconnected", this.onReconnected.bind(this));
        this.hub.addEventListener("userJoined", this.onUserJoined.bind(this));
        this.hub.addEventListener("iceReceived", this.onIceReceived.bind(this));
        this.hub.addEventListener("offerReceived", this.onOfferReceived.bind(this));
        this.hub.addEventListener("answerReceived", this.onAnswerReceived.bind(this));
        this.hub.addEventListener("userLeft", this.onUserLeft.bind(this));
        this.hub.addEventListener("userPosed", this.onUserPosed.bind(this));
        this.hub.addEventListener("userPointer", this.onUserPointer.bind(this));
        this.hub.addEventListener("chat", this.onChat.bind(this));

        this.remoteGainDecay = new GainDecayer(
            this.audio.context,
            this.microphones.autoGainNode,
            0.05,
            1,
            0.25,
            0,
            250,
            0.25,
            1000,
            250
        );
        this.audio.destination.remoteUserInput.connect(this.remoteGainDecay);

        this.remoteGainDecay.enabled = !this.audio.useHeadphones;
        this.audio.addEventListener("useheadphonestoggled", () => {
            this.remoteGainDecay.enabled = !this.audio.useHeadphones;
        });

        this.windowQuitter.addScopedEventListener(this, "quitting", () => {
            if (this.conferenceState === ConnectionState.Connected) {
                this.toRoom("leave");
            }
        });
    }

    get connectionState() {
        return this.hub.connectionState;
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
            this.windowQuitter.removeScope(this);
            this.remoteGainDecay.dispose();
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
            this.removeUser(user);
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

    private onClose() {
        this.lastRoom = null;
        this.lastUserID = null;
        this.setConferenceState(ConnectionState.Disconnected);
    }

    private onReconnecting(evt: HubReconnectingEvent) {
        this.dispatchEvent(new ConferenceErrorEvent(evt.error));
    }

    private async onReconnected() {
        this.lastRoom = null;
        this.lastUserID = null;
        this.setConferenceState(ConnectionState.Disconnected);
        await this.identify(this.localUserName);
        await this.join(this.roomName);
    }

    private async onUserJoined(evt: HubUserJoinedEvent) {
        if (this.users.has(evt.fromUserID)) {
            const user = this.users.get(evt.fromUserID);
            user.start();
        }
        else {
            if (this.users.size === 0) {
                this.remoteGainDecay.start();
            }

            const rtcConfig = await this.getRTCConfiguration();
            const user = new RemoteUser(evt.fromUserID, evt.fromUserName, rtcConfig, true);
            this.users.set(user.userID, user);

            user.addEventListener("iceError", this.onIceError.bind(this));
            user.addEventListener("iceCandidate", this.onIceCandidate.bind(this));
            user.addEventListener("offer", this.onOfferCreated.bind(this));
            user.addEventListener("answer", this.onAnswerCreated.bind(this));
            user.addEventListener("streamNeeded", this.onStreamNeeded.bind(this));
            user.addEventListener("trackAdded", this.onTrackAdded.bind(this));
            user.addEventListener("trackMuted", this.onTrackMuted.bind(this));
            user.addEventListener("trackRemoved", this.onTrackRemoved.bind(this));
            user.addEventListener("userLeft", (evt) => this.removeUser(evt.user));

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

    private async onIceReceived(evt: HubIceReceivedEvent) {
        try {
            const user = this.users.get(evt.fromUserID);
            if (user) {
                const ice = JSON.parse(evt.candidateJSON) as RTCIceCandidate;
                await user.addIceCandidate(ice);
            }
        }
        catch (exp) {
            this.err("iceReceived", `${exp.message} [${evt.fromUserID}] (${evt.candidateJSON})`);
        }
    }

    private async onOfferCreated(evt: RemoteUserOfferEvent) {
        await this.sendOffer(evt.user.userID, evt.offer);
    }

    private async onOfferReceived(evt: HubOfferReceivedEvent) {
        try {
            const user = this.users.get(evt.fromUserID);
            if (user) {
                const offer = JSON.parse(evt.offerJSON) as RTCSessionDescription;
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

    private async onAnswerReceived(evt: HubAnswerReceivedEvent) {
        try {
            const user = this.users.get(evt.fromUserID);
            if (user) {
                const answer = JSON.parse(evt.answerJSON) as RTCSessionDescription;
                await user.acceptAnswer(answer);
            }
        }
        catch (exp) {
            this.err("answerReceived", exp.message);
        }
    }

    private onStreamNeeded(evt: RemoteUserStreamNeededEvent) {
        evt.user.sendStream(...this.devices.outStreams);
    }

    private onTrackAdded(evt: RemoteUserTrackAddedEvent) {
        this.dispatchEvent(evt);
    }

    private onTrackMuted(evt: RemoteUserTrackMutedEvent) {
        this.dispatchEvent(evt);
    }

    private onTrackRemoved(evt: RemoteUserTrackRemovedEvent) {
        this.dispatchEvent(evt);
    }

    private onUserLeft(evt: HubUserLeftEvent): void {
        this.removeUser(this.users.get(evt.fromUserID));
    }

    private removeUser(user: RemoteUser): void {
        if (isDefined(user)) {
            user.dispose();
            this.users.delete(user.userID);
            if (this.users.size === 0) {
                this.remoteGainDecay.stop();
            }

            this.audio.removeUser(user.userID);
            this.dispatchEvent(new UserLeftEvent(user));
        }
    }

    private onUserPosed(evt: HubUserPosedEvent) {
        const user = this.users.get(evt.fromUserID);
        if (user) {
            user.recvPose(
                evt.px, evt.py, evt.pz,
                evt.fx, evt.fy, evt.fz,
                evt.ux, evt.uy, evt.uz,
                evt.height);
        }
    }

    private onUserPointer(evt: HubUserPointerEvent) {
        const user = this.users.get(evt.fromUserID);
        if (user) {
            user.recvPointer(
                evt.pointerID,
                evt.px, evt.py, evt.pz,
                evt.fx, evt.fy, evt.fz,
                evt.ux, evt.uy, evt.uz);
        }
    }

    private onChat(evt: HubUserChatEvent) {
        const user = this.users.get(evt.fromUserID);
        if (user) {
            this.dispatchEvent(new UserChatEvent(user, evt.text));
        }
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
        if (this.conferenceState === ConnectionState.Connected) {
            await Promise.all(
                Array.from(this.users.values())
                    .map((user) =>
                        user.sendPose(px, py, pz, fx, fy, fz, ux, uy, uz, height)));
        }
    }

    async setLocalPointer(pointerID: PointerID, px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): Promise<void> {
        if (this.conferenceState === ConnectionState.Connected) {
            await Promise.all(
                Array.from(this.users.values())
                    .map((user) =>
                        user.sendPointer(pointerID, px, py, pz, fx, fy, fz, ux, uy, uz)));
        }
    }

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
            .map((u) => [u.userID, u.userName]);
    }
}