import { HttpTransportType, HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";
import { MediaStreamSource, removeVertex } from "juniper-audio";
import { assertNever, TypedEventBase } from "juniper-tslib";
import adapter from 'webrtc-adapter';
import { ConferenceErrorEvent, ConferenceServerConnectedEvent, ConferenceServerDisconnectedEvent, RoomJoinedEvent, RoomLeftEvent, StreamType, UserAudioMutedEvent, UserAudioStreamAddedEvent, UserAudioStreamRemovedEvent, UserChatEvent, UserJoinedEvent, UserLeftEvent, UserVideoMutedEvent, UserVideoStreamAddedEvent, UserVideoStreamRemovedEvent } from "./ConferenceEvents";
import { ConnectionState, settleConnected, whenDisconnected } from "./ConnectionState";
import { DecayingGain } from "./DecayingGain";
import { RemoteUser } from "./RemoteUser";
export const DEFAULT_LOCAL_USER_ID = "local-user";
let loggingEnabled = window.location.hostname === "localhost"
    || /\bdebug\b/.test(window.location.search);
const sockets = new Array();
const fakeSocket = function (...args) {
    console.log("New connection", ...args);
    const socket = new WebSocket(...args);
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
export var ClientState;
(function (ClientState) {
    ClientState["InConference"] = "in-conference";
    ClientState["JoiningConference"] = "joining-conference";
    ClientState["Connected"] = "connected";
    ClientState["Connecting"] = "connecting";
    ClientState["Prepaired"] = "prepaired";
    ClientState["Prepairing"] = "prepairing";
    ClientState["Unprepared"] = "unprepaired";
})(ClientState || (ClientState = {}));
export class TeleconferenceManager extends TypedEventBase {
    audio;
    signalRPath;
    autoSetPosition;
    needsVideoDevice;
    toggleLogging() {
        loggingEnabled = !loggingEnabled;
    }
    _isAudioMuted = null;
    get isAudioMuted() { return this._isAudioMuted; }
    _isVideoMuted = null;
    get isVideoMuted() { return this._isVideoMuted; }
    _localUserID = DEFAULT_LOCAL_USER_ID;
    get localUserID() { return this._localUserID; }
    _localUserName = null;
    get localUserName() { return this._localUserName; }
    _roomName = null;
    get roomName() { return this._roomName; }
    _conferenceState = ConnectionState.Disconnected;
    _hasAudioPermission = false;
    get hasAudioPermission() { return this._hasAudioPermission; }
    _hasVideoPermission = false;
    get hasVideoPermission() { return this._hasVideoPermission; }
    lastRoom = null;
    lastUserID = null;
    users = new Map();
    localStreamIn = null;
    hub;
    remoteGainDecay;
    _ready = null;
    get ready() {
        if (this._ready === null) {
            this._ready = this.startInternal();
        }
        return this._ready;
    }
    constructor(audio, signalRPath, autoSetPosition = true, needsVideoDevice = false) {
        super();
        this.audio = audio;
        this.signalRPath = signalRPath;
        this.autoSetPosition = autoSetPosition;
        this.needsVideoDevice = needsVideoDevice;
        if (loggingEnabled) {
            console.log(adapter);
        }
        let hubBuilder = new HubConnectionBuilder()
            .withAutomaticReconnect();
        hubBuilder.withUrl(this.signalRPath, HttpTransportType.WebSockets);
        this.hub = hubBuilder.build();
        this.audio.devices.addEventListener("audioinputchanged", (evt) => this.onAudioInputChanged(evt));
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
        this.hub.on("userLeft", (fromUserID) => {
            const user = this.users.get(fromUserID);
            if (user) {
                this.onUserLeft(user);
            }
        });
        this.hub.on("userPosed", (fromUserID, px, py, pz, fx, fy, fz, ux, uy, uz, height) => {
            const user = this.users.get(fromUserID);
            if (user) {
                user.recvPose(px, py, pz, fx, fy, fz, ux, uy, uz, height);
            }
        });
        this.hub.on("userPointer", (fromUserID, name, px, py, pz, fx, fy, fz, ux, uy, uz) => {
            const user = this.users.get(fromUserID);
            if (user) {
                user.recvPointer(name, px, py, pz, fx, fy, fz, ux, uy, uz);
            }
        });
        this.hub.on("chat", (fromUserID, text) => {
            const user = this.users.get(fromUserID);
            if (user) {
                this.dispatchEvent(new UserChatEvent(user, text));
            }
        });
        this.remoteGainDecay = new DecayingGain(this.audio.audioCtx, this.audio.audioDestination.remoteUserInput, this.audio.localAutoControlledGain, 0.05, 1, 0.25, 0, 250, 0.25, 1000, 250);
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
    async startInternal() {
        await this.audio.ready;
        await this.audio.devices.startPreferredAudioInput();
        this.localStream = this.audio.devices.currentStream;
    }
    get connectionState() {
        switch (this.hub.state) {
            case HubConnectionState.Connected: return ConnectionState.Connected;
            case HubConnectionState.Connecting:
            case HubConnectionState.Reconnecting: return ConnectionState.Connecting;
            case HubConnectionState.Disconnected: return ConnectionState.Disconnected;
            case HubConnectionState.Disconnecting: return ConnectionState.Disconnecting;
            default: assertNever(this.hub.state);
        }
    }
    get echoControl() {
        return this.remoteGainDecay;
    }
    get isConnected() {
        return this.connectionState === ConnectionState.Connected;
    }
    get conferenceState() {
        return this._conferenceState;
    }
    get isConferenced() {
        return this.conferenceState === ConnectionState.Connected;
    }
    setConferenceState(state) {
        this._conferenceState = state;
    }
    disposed = false;
    dispose() {
        if (!this.disposed) {
            this.leave();
            this.disconnect();
            this.disposed = true;
        }
    }
    err(source, ...msg) {
        console.warn(source, ...msg);
    }
    async toServer(method, ...rest) {
        return await this.hub.invoke(method, ...rest);
    }
    async toRoom(method, ...rest) {
        if (this.isConnected) {
            await this.hub.invoke(method, this.localUserID, this.roomName, ...rest);
        }
    }
    async toUser(method, toUserID, ...rest) {
        if (this.isConnected) {
            await this.hub.invoke(method, this.localUserID, toUserID, ...rest);
        }
    }
    async connect() {
        await this.ready;
        await whenDisconnected("Connecting", () => this.connectionState, async () => {
            await this.hub.start();
            this.dispatchEvent(new ConferenceServerConnectedEvent());
        });
    }
    async join(roomName) {
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
    async identify(userName) {
        if (this.localUserID === DEFAULT_LOCAL_USER_ID) {
            this._localUserID = null;
        }
        this._localUserID = this.localUserID || await this.toServer("getNewUserID");
        this._localUserName = userName;
        if (this.localUserID
            && this.localUserID !== this.lastUserID
            && this.isConnected) {
            this.lastUserID = this.localUserID;
            await this.toServer("identify", this.localUserID);
        }
    }
    async leave() {
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
    async disconnect() {
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
    async onUserJoined(fromUserID, fromUserName) {
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
                this.audio.setUserPose(evt.user.userID, p[0], p[1], p[2], f[0], f[1], f[2], u[0], u[1], u[2]);
            });
            user.addEventListener("userPointer", (evt) => this.onRemoteUserPosed(evt));
            this.toUser("greet", user.userID, this.localUserName);
            const source = this.audio.createUser(user.userID, user.userName);
            this.dispatchEvent(new UserJoinedEvent(user, source));
        }
    }
    async getRTCConfiguration() {
        return await this.toServer("getRTCConfiguration", this.localUserID);
    }
    onIceError(evt) {
        this.err("icecandidateerror", this.localUserName, evt.user.userName, `${evt.url} [${evt.errorCode}]: ${evt.errorText}`);
    }
    async onIceCandidate(evt) {
        await this.sendIce(evt.user.userID, evt.candidate);
    }
    async onIceReceived(fromUserID, candidateJSON) {
        try {
            const user = this.users.get(fromUserID);
            if (user) {
                const ice = JSON.parse(candidateJSON);
                await user.addIceCandidate(ice);
            }
        }
        catch (exp) {
            this.err("iceReceived", `${exp.message} [${fromUserID}] (${candidateJSON})`);
        }
    }
    async onOfferCreated(evt) {
        await this.sendOffer(evt.user.userID, evt.offer);
    }
    async onOfferReceived(fromUserID, offerJSON) {
        try {
            const user = this.users.get(fromUserID);
            if (user) {
                const offer = JSON.parse(offerJSON);
                await user.acceptOffer(offer);
            }
        }
        catch (exp) {
            this.err("offerReceived", exp.message);
        }
    }
    async onAnswerCreated(evt) {
        await this.sendAnswer(evt.user.userID, evt.answer);
    }
    async onAnswerReceived(fromUserID, answerJSON) {
        try {
            const user = this.users.get(fromUserID);
            if (user) {
                const answer = JSON.parse(answerJSON);
                await user.acceptAnswer(answer);
            }
        }
        catch (exp) {
            this.err("answerReceived", exp.message);
        }
    }
    onStreamNeeded(evt) {
        evt.user.sendStream(this.audio.output.stream);
    }
    onTrackAdded(evt) {
        if (evt.track.kind === "audio") {
            this.audio.setUserStream(evt.user.userID, evt.user.userName, evt.stream);
            this.dispatchEvent(new UserAudioStreamAddedEvent(evt.user, evt.stream));
        }
        else if (evt.track.kind === "video") {
            this.dispatchEvent(new UserVideoStreamAddedEvent(evt.user, evt.stream));
        }
    }
    onTrackMuted(evt) {
        if (evt.track.kind === "audio") {
            this.dispatchEvent(new UserAudioMutedEvent(evt.user, evt.track.muted));
        }
        else if (evt.track.kind === "video") {
            this.dispatchEvent(new UserVideoMutedEvent(evt.user, evt.track.muted));
        }
    }
    onTrackRemoved(evt) {
        if (evt.track.kind === "audio") {
            this.audio.setUserStream(evt.user.userID, evt.user.userName, null);
            this.dispatchEvent(new UserAudioStreamRemovedEvent(evt.user, evt.stream));
        }
        else if (evt.track.kind === "video") {
            this.dispatchEvent(new UserVideoStreamRemovedEvent(evt.user, evt.stream));
        }
    }
    onUserLeft(user) {
        user.dispose();
        this.users.delete(user.userID);
        if (this.users.size === 0) {
            this.remoteGainDecay.stop();
        }
        this.audio.removeUser(user.userID);
        this.dispatchEvent(new UserLeftEvent(user));
    }
    async sendIce(toUserID, candidate) {
        await this.toUser("sendIce", toUserID, JSON.stringify(candidate));
    }
    async sendOffer(toUserID, offer) {
        await this.toUser("sendOffer", toUserID, JSON.stringify(offer));
    }
    async sendAnswer(toUserID, answer) {
        await this.toUser("sendAnswer", toUserID, JSON.stringify(answer));
    }
    async setLocalPose(px, py, pz, fx, fy, fz, ux, uy, uz, height) {
        if (this.autoSetPosition) {
            this.audio.setUserPose(this.localUserID, px, py, pz, fx, fy, fz, ux, uy, uz);
        }
        if (this.conferenceState === ConnectionState.Connected) {
            await Promise.all(Array.from(this.users.values())
                .map(user => user.sendPose(px, py, pz, fx, fy, fz, ux, uy, uz, height)));
        }
    }
    async setLocalPointer(name, px, py, pz, fx, fy, fz, ux, uy, uz) {
        if (this.conferenceState === ConnectionState.Connected) {
            await Promise.all(Array.from(this.users.values())
                .map(user => user.sendPointer(name, px, py, pz, fx, fy, fz, ux, uy, uz)));
        }
    }
    onRemoteUserPosed(evt) {
        const offset = this.audio.getUserOffset(evt.user.userID);
        if (offset) {
            evt.pose.setOffset(offset[0], offset[1], offset[2]);
        }
    }
    ;
    chat(text) {
        if (this.conferenceState === ConnectionState.Connected) {
            this.toRoom("chat", text);
        }
    }
    userExists(id) {
        return this.users.has(id);
    }
    getUserNames() {
        return Array.from(this.users.values())
            .map(u => [u.userID, u.userName]);
    }
    async onAudioInputChanged(evt) {
        const deviceId = evt.audio && evt.audio.deviceId;
        await this.startStream(deviceId, this.audio.useHeadphones);
    }
    async startStream(deviceId, usingHeadphones) {
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
    async restartStream() {
        this.localStream = null;
        await this.startStream(this.audio.devices.preferredAudioOutputID, this.audio.useHeadphones);
    }
    get localStream() {
        return this.localStreamIn && this.localStreamIn.mediaStream || null;
    }
    set localStream(v) {
        if (v !== this.localStream) {
            if (this.localStreamIn) {
                removeVertex(this.localStreamIn);
                this.localStreamIn = null;
            }
            this.audio.devices.currentStream = v;
            if (v) {
                this.localStreamIn = MediaStreamSource("local-mic", this.audio.audioCtx, v, this.audio);
            }
        }
    }
    isMediaMuted(type) {
        for (const track of this.audio.output.stream.getTracks()) {
            if (track.kind === type) {
                return !track.enabled;
            }
        }
        return true;
    }
    setMediaMuted(type, muted) {
        for (const track of this.audio.output.stream.getTracks()) {
            if (track.kind === type) {
                track.enabled = !muted;
            }
        }
    }
    get audioMuted() {
        return this.isMediaMuted(StreamType.Audio);
    }
    set audioMuted(muted) {
        this.setMediaMuted(StreamType.Audio, muted);
    }
    get videoMuted() {
        return this.isMediaMuted(StreamType.Video);
    }
    set videoMuted(muted) {
        this.setMediaMuted(StreamType.Video, muted);
    }
}
