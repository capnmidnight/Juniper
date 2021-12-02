import { arrayClear, arrayRemove, arrayScan, isArrayBuffer, TypedEvent, TypedEventBase } from "juniper-tslib";
import { UserLeftEvent, UserPointerEvent, UserPosedEvent } from "./ConferenceEvents";
class Locker {
    locks = new Set();
    isLocked(name) {
        return this.locks.has(name);
    }
    isUnlocked(name) {
        return !this.locks.has(name);
    }
    lock(name) {
        this.locks.add(name);
    }
    unlock(name) {
        this.locks.delete(name);
    }
    async withSkipLock(name, act) {
        if (this.isUnlocked(name)) {
            this.lock(name);
            try {
                await act();
            }
            finally {
                this.unlock(name);
            }
        }
    }
}
class RemoteUserEvent extends TypedEvent {
    user;
    constructor(type, user) {
        super(type);
        this.user = user;
    }
}
export class RemoteUserIceErrorEvent extends RemoteUserEvent {
    address;
    errorCode;
    errorText;
    port;
    url;
    constructor(user, evt) {
        super("iceError", user);
        this.address = evt.address;
        this.errorCode = evt.errorCode;
        this.errorText = evt.errorText;
        this.port = evt.port;
        this.url = evt.url;
        Object.seal(this);
    }
}
export class RemoteUserIceCandidateEvent extends RemoteUserEvent {
    candidate;
    constructor(user, candidate) {
        super("iceCandidate", user);
        this.candidate = candidate;
    }
}
export class RemoteUserOfferEvent extends RemoteUserEvent {
    offer;
    constructor(user, offer) {
        super("offer", user);
        this.offer = offer;
    }
}
export class RemoteUserAnswerEvent extends RemoteUserEvent {
    answer;
    constructor(user, answer) {
        super("answer", user);
        this.answer = answer;
    }
}
export class RemoteUserStreamNeededEvent extends RemoteUserEvent {
    constructor(user) {
        super("streamNeeded", user);
    }
}
export class RemoteUserTrackAddedEvent extends RemoteUserEvent {
    track;
    stream;
    constructor(user, track, stream) {
        super("trackAdded", user);
        this.track = track;
        this.stream = stream;
    }
}
export class RemoteUserTrackMutedEvent extends RemoteUserEvent {
    track;
    constructor(user, track) {
        super("trackMuted", user);
        this.track = track;
    }
}
export class RemoteUserTrackRemovedEvent extends RemoteUserEvent {
    track;
    stream;
    constructor(user, track, stream) {
        super("trackRemoved", user);
        this.track = track;
        this.stream = stream;
    }
}
const seenUsers = new Set();
var Message;
(function (Message) {
    Message[Message["Pointer"] = 1] = "Pointer";
    Message[Message["Pose"] = 2] = "Pose";
    Message[Message["InvocationComplete"] = 3] = "InvocationComplete";
})(Message || (Message = {}));
export class RemoteUser extends TypedEventBase {
    userID;
    userName;
    confirmReceipt;
    userPosedEvt = new UserPosedEvent(this);
    userPointerEvt = new UserPointerEvent(this);
    sendPoseBuffer = new Float32Array(12);
    sendPointerBuffer = new Float32Array(12);
    sendInvocationCompleteBuffer = new Float32Array(2);
    transceivers = new Array();
    invocationCount = 0;
    invocations = new Map();
    locks = new Locker();
    connection;
    channel = null;
    gotOffer = false;
    disposed = false;
    trackSent = false;
    constructor(userID, userName, rtcConfig, confirmReceipt) {
        super();
        this.userID = userID;
        this.userName = userName;
        this.confirmReceipt = confirmReceipt;
        this.sendPoseBuffer[0] = Message.Pose;
        this.sendPointerBuffer[0] = Message.Pointer;
        this.sendInvocationCompleteBuffer[0] = Message.InvocationComplete;
        this.connection = new RTCPeerConnection(rtcConfig);
        this.connection.addEventListener("icecandidateerror", (evt) => {
            this.dispatchEvent(new RemoteUserIceErrorEvent(this, evt));
        });
        this.connection.addEventListener("icecandidate", async (evt) => {
            if (evt.candidate) {
                this.dispatchEvent(new RemoteUserIceCandidateEvent(this, evt.candidate));
            }
        });
        this.connection.addEventListener("negotiationneeded", async () => {
            if (this.trackSent || this.gotOffer) {
                const iceRestart = seenUsers.has(this.userID);
                const offer = await this.connection.createOffer({
                    iceRestart
                });
                if (!iceRestart) {
                    seenUsers.add(this.userID);
                }
                await this.connection.setLocalDescription(offer);
                this.dispatchEvent(new RemoteUserOfferEvent(this, this.connection.localDescription.toJSON()));
            }
        });
        this.connection.addEventListener("datachannel", (evt) => {
            this.setChannel(evt.channel);
        });
        this.connection.addEventListener("track", async (evt) => {
            const transceiver = evt.transceiver;
            const track = evt.track;
            const stream = arrayScan(evt.streams, (s) => {
                const tracks = s.getTracks();
                for (const t of tracks) {
                    if (t === track) {
                        return true;
                    }
                }
                return false;
            });
            if (this.transceivers.indexOf(transceiver) === -1) {
                this.transceivers.push(transceiver);
            }
            const onMute = () => {
                this.dispatchEvent(new RemoteUserTrackMutedEvent(this, track));
            };
            const onEnd = () => {
                track.removeEventListener("ended", onEnd);
                track.removeEventListener("mute", onMute);
                arrayRemove(this.transceivers, transceiver);
                this.dispatchEvent(new RemoteUserTrackRemovedEvent(this, track, stream));
            };
            track.addEventListener("ended", onEnd);
            track.addEventListener("mute", onMute);
            this.dispatchEvent(new RemoteUserTrackAddedEvent(this, track, stream));
            if (!this.trackSent) {
                this.start();
            }
            else {
                this.setChannel(this.connection.createDataChannel("poses", {
                    ordered: false,
                    maxRetransmits: 0
                }));
            }
        });
        let wasConnected = false;
        this.connection.addEventListener("connectionstatechange", () => {
            if (this.connection.connectionState === "failed" && wasConnected) {
                this.dispatchEvent(new UserLeftEvent(this));
            }
            else if (this.connection.connectionState === "connected") {
                wasConnected = true;
            }
        });
        Object.seal(this);
    }
    dispose() {
        if (!this.disposed) {
            if (this.channel) {
                this.channel.close();
                this.channel = null;
            }
            for (const transceiver of this.transceivers) {
                transceiver.stop();
            }
            arrayClear(this.transceivers);
            this.connection.close();
            this.disposed = true;
        }
    }
    setChannel(channel) {
        this.channel = channel;
        this.channel.binaryType = "arraybuffer";
        this.channel.addEventListener("message", (evt) => {
            if (isArrayBuffer(evt.data)) {
                const data = new Float32Array(evt.data);
                const msg = data[0];
                const invocationID = data[1];
                if (msg === Message.InvocationComplete) {
                    const resolve = this.invocations.get(invocationID);
                    if (resolve) {
                        resolve();
                    }
                }
                else if (this.channel) {
                    if (msg === Message.Pose) {
                        this.recvPose(data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11]);
                    }
                    else {
                        this.recvPointer(data[11], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10]);
                    }
                    if (invocationID >= 0) {
                        this.sendInvocationCompleteBuffer[1] = invocationID;
                        this.channel.send(this.sendInvocationCompleteBuffer);
                    }
                }
            }
        });
    }
    recvPose(px, py, pz, fx, fy, fz, ux, uy, uz, height) {
        this.userPosedEvt.pose.set(px, py, pz, fx, fy, fz, ux, uy, uz);
        this.userPosedEvt.height = height;
        this.dispatchEvent(this.userPosedEvt);
    }
    sendPose(px, py, pz, fx, fy, fz, ux, uy, uz, height) {
        return this.sendMessage(Message.Pose, px, py, pz, fx, fy, fz, ux, uy, uz, height);
    }
    recvPointer(pointerName, px, py, pz, fx, fy, fz, ux, uy, uz) {
        this.userPointerEvt.name = pointerName;
        this.userPointerEvt.pose.set(px, py, pz, fx, fy, fz, ux, uy, uz);
        this.dispatchEvent(this.userPointerEvt);
    }
    sendPointer(pointerName, px, py, pz, fx, fy, fz, ux, uy, uz) {
        return this.sendMessage(Message.Pointer, px, py, pz, fx, fy, fz, ux, uy, uz, pointerName);
    }
    async sendMessage(msgName, px, py, pz, fx, fy, fz, ux, uy, uz, pointerNameOrHeight) {
        if (this.channel) {
            this.locks.withSkipLock(msgName, async () => {
                const buffer = msgName === Message.Pose ? this.sendPoseBuffer : this.sendPointerBuffer;
                const invocationID = ++this.invocationCount;
                buffer[1] = invocationID;
                buffer[2] = px;
                buffer[3] = py;
                buffer[4] = pz;
                buffer[5] = fx;
                buffer[6] = fy;
                buffer[7] = fz;
                buffer[8] = ux;
                buffer[9] = uy;
                buffer[10] = uz;
                buffer[11] = pointerNameOrHeight;
                if (!this.confirmReceipt) {
                    this.channel.send(buffer);
                }
                else {
                    const task = new Promise((resolve, reject) => {
                        const timeout = setTimeout(reject, 100, "timeout");
                        const onComplete = () => {
                            clearTimeout(timeout);
                            resolve();
                        };
                        this.invocations.set(invocationID, onComplete);
                    });
                    this.channel.send(buffer);
                    try {
                        await task;
                    }
                    catch (exp) {
                        if (exp !== "timeout") {
                            console.error(exp);
                        }
                    }
                    finally {
                        this.invocations.delete(invocationID);
                    }
                }
            });
        }
    }
    async addIceCandidate(ice) {
        await this.connection.addIceCandidate(ice);
    }
    async acceptOffer(offer) {
        await this.connection.setRemoteDescription(offer);
        this.gotOffer = true;
        const answer = await this.connection.createAnswer();
        await this.connection.setLocalDescription(answer);
        this.dispatchEvent(new RemoteUserAnswerEvent(this, this.connection.localDescription.toJSON()));
    }
    async acceptAnswer(answer) {
        await this.connection.setRemoteDescription(answer);
    }
    start() {
        if (!this.trackSent) {
            this.trackSent = true;
            this.dispatchEvent(new RemoteUserStreamNeededEvent(this));
        }
    }
    sendStream(stream) {
        for (const track of stream.getTracks()) {
            if (this.trackSent) {
                this.connection.addTrack(track, stream);
            }
            else {
                this.transceivers.push(this.connection.addTransceiver(track, {
                    streams: [stream]
                }));
            }
        }
    }
}
