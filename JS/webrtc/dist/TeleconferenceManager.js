import { TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
import { WindowQuitEventer } from "@juniper-lib/events/dist/WindowQuitEventer";
import { singleton } from "@juniper-lib/tslib/dist/singleton";
import { isDefined } from "@juniper-lib/tslib/dist/typeChecks";
import { dispose } from "@juniper-lib/tslib/dist/using";
import "webrtc-adapter";
import { ConferenceErrorEvent, ConferenceServerConnectedEvent, ConferenceServerDisconnectedEvent, RoomJoinedEvent, RoomLeftEvent, UserJoinedEvent, UserLeftEvent } from "./ConferenceEvents";
import { ConnectionState, settleConnected, whenDisconnected } from "./ConnectionState";
import { GainDecayer } from "./GainDecayer";
import { RemoteUser } from "./RemoteUser";
import { DEFAULT_LOCAL_USER_ID } from "./constants";
import { makeErrorMessage } from "../../tslib/src/makeErrorMessage";
const sockets = singleton("Juniper:Sockets", () => new Array());
function fakeSocket(...args) {
    const socket = new WebSocket(...args);
    sockets.push(socket);
    return socket;
}
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
            sockets.forEach(dispose);
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
export class TeleconferenceManager extends TypedEventTarget {
    get isAudioMuted() { return this._isAudioMuted; }
    get isVideoMuted() { return this._isVideoMuted; }
    get localUserID() { return this._localUserID; }
    get localUserName() { return this._localUserName; }
    get roomName() { return this._roomName; }
    get hasAudioPermission() { return this._hasAudioPermission; }
    get hasVideoPermission() { return this._hasVideoPermission; }
    constructor(audio, microphones, webcams, hub) {
        super();
        this.audio = audio;
        this.microphones = microphones;
        this.webcams = webcams;
        this.hub = hub;
        this._isAudioMuted = null;
        this._isVideoMuted = null;
        this._localUserID = DEFAULT_LOCAL_USER_ID;
        this._localUserName = null;
        this._roomName = null;
        this._conferenceState = ConnectionState.Disconnected;
        this._hasAudioPermission = false;
        this._hasVideoPermission = false;
        this.lastRoom = null;
        this.lastUserID = null;
        this.users = new Map();
        this.windowQuitter = new WindowQuitEventer();
        this.heartbeatTimer = null;
        this.disposed = false;
        const onStreamChanged = (evt) => {
            for (const user of this.users.values()) {
                if (evt.oldStream) {
                    user.removeStream(evt.oldStream);
                }
                if (evt.newStream) {
                    user.sendStream(evt.newStream);
                }
            }
        };
        this.microphones.addEventListener("streamchanged", onStreamChanged);
        this.webcams.addEventListener("streamchanged", onStreamChanged);
        this.hub.addEventListener("close", this.onClose.bind(this));
        this.hub.addEventListener("reconnecting", this.onReconnecting.bind(this));
        this.hub.addEventListener("reconnected", this.onReconnected.bind(this));
        this.hub.addEventListener("userJoined", this.onUserJoined.bind(this));
        this.hub.addEventListener("iceReceived", this.onIceReceived.bind(this));
        this.hub.addEventListener("offerReceived", this.onOfferReceived.bind(this));
        this.hub.addEventListener("answerReceived", this.onAnswerReceived.bind(this));
        this.hub.addEventListener("userLeft", this.onUserLeft.bind(this));
        this.hub.addEventListener("chat", this.onChat.bind(this));
        this.remoteGainDecay = new GainDecayer(this.audio.context, this.microphones.autoGainNode, 0.05, 1, 0.25, 0, 250, 0.25, 1000, 250);
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
    dispose() {
        if (!this.disposed) {
            this.leave();
            this.disconnect();
            this.windowQuitter.removeScope(this);
            dispose(this.remoteGainDecay);
            this.disposed = true;
        }
    }
    err(source, messageOrError, maybeError) {
        console.warn(source, makeErrorMessage(messageOrError, maybeError));
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
        await whenDisconnected("Connecting", () => this.connectionState, async () => {
            await this.hub.start();
            this.heartbeatTimer = setInterval(() => {
                if (this.hub.connectionState === ConnectionState.Connected) {
                    this.hub.invoke("heartbeat", this.localUserID);
                }
            }, 2500);
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
            this.removeUser(user);
        }
        this._roomName
            = this.lastRoom
                = null;
        this.setConferenceState(ConnectionState.Disconnected);
        this.dispatchEvent(new RoomLeftEvent(this.localUserID));
    }
    async disconnect() {
        if (isDefined(this.heartbeatTimer)) {
            clearInterval(this.heartbeatTimer);
            this.heartbeatTimer = null;
        }
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
    onClose() {
        this.lastRoom = null;
        this.lastUserID = null;
        this.setConferenceState(ConnectionState.Disconnected);
    }
    onReconnecting(evt) {
        this.dispatchEvent(new ConferenceErrorEvent(evt.error));
    }
    async onReconnected() {
        this.lastRoom = null;
        this.lastUserID = null;
        this.setConferenceState(ConnectionState.Disconnected);
        await this.identify(this.localUserName);
        await this.join(this.roomName);
    }
    async onUserJoined(evt) {
        if (this.users.has(evt.fromUserID)) {
            const user = this.users.get(evt.fromUserID);
            user.start();
        }
        else {
            if (this.users.size === 0) {
                this.remoteGainDecay.start();
            }
            const rtcConfig = await this.getRTCConfiguration();
            const user = new RemoteUser(evt.fromUserID, evt.fromUserName, rtcConfig);
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
    async getRTCConfiguration() {
        return await this.toServer("getRTCConfiguration", this.localUserID);
    }
    onIceError(evt) {
        this.err("icecandidateerror", `${this.localUserName} ${evt.user.userName} ${evt.url} [${evt.errorCode}]: ${evt.errorText}`);
    }
    async onIceCandidate(evt) {
        await this.sendIce(evt.user.userID, evt.candidate);
    }
    async onIceReceived(evt) {
        try {
            const user = this.users.get(evt.fromUserID);
            if (user) {
                const ice = JSON.parse(evt.candidateJSON);
                await user.addIceCandidate(ice);
            }
        }
        catch (exp) {
            this.err("iceReceived", `$1 [${evt.fromUserID}] (${evt.candidateJSON})`, exp);
        }
    }
    async onOfferCreated(evt) {
        await this.sendOffer(evt.user.userID, evt.offer);
    }
    async onOfferReceived(evt) {
        try {
            const user = this.users.get(evt.fromUserID);
            if (user) {
                const offer = JSON.parse(evt.offerJSON);
                await user.acceptOffer(offer);
            }
        }
        catch (exp) {
            this.err("offerReceived", exp);
        }
    }
    async onAnswerCreated(evt) {
        await this.sendAnswer(evt.user.userID, evt.answer);
    }
    async onAnswerReceived(evt) {
        try {
            const user = this.users.get(evt.fromUserID);
            if (user) {
                const answer = JSON.parse(evt.answerJSON);
                await user.acceptAnswer(answer);
            }
        }
        catch (exp) {
            this.err("answerReceived", exp);
        }
    }
    onStreamNeeded(evt) {
        evt.user.sendStream(this.microphones.outStream, this.webcams.outStream);
    }
    onTrackAdded(evt) {
        this.dispatchEvent(evt);
    }
    onTrackMuted(evt) {
        this.dispatchEvent(evt);
    }
    onTrackRemoved(evt) {
        this.dispatchEvent(evt);
    }
    onUserLeft(evt) {
        this.removeUser(this.users.get(evt.fromUserID));
    }
    removeUser(user) {
        if (isDefined(user)) {
            dispose(user);
            this.users.delete(user.userID);
            if (this.users.size === 0) {
                this.remoteGainDecay.stop();
            }
            this.audio.removeUser(user.userID);
            this.dispatchEvent(new UserLeftEvent(user));
        }
    }
    onChat(evt) {
        const user = this.users.get(evt.fromUserID);
        if (user) {
            user.recvChat(evt.text);
        }
    }
    sendChat(text) {
        if (this.conferenceState === ConnectionState.Connected) {
            this.toRoom("chat", text);
        }
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
    async forEachUser(callback) {
        if (this.conferenceState === ConnectionState.Connected) {
            await Promise.all(Array.from(this.users.values())
                .map(callback));
        }
    }
    async sendUserState(buffer) {
        await this.forEachUser((user) => user.sendUserState(buffer));
    }
    userExists(id) {
        return this.users.has(id);
    }
    getUserNames() {
        return Array.from(this.users.values())
            .map((u) => [u.userID, u.userName]);
    }
}
//# sourceMappingURL=TeleconferenceManager.js.map