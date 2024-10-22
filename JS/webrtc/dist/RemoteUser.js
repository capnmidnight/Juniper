import { arrayClear, arrayRemove, arrayScan, dispose, isArrayBuffer } from "@juniper-lib/util";
import { Task, TypedEvent, TypedEventTarget } from "@juniper-lib/events";
import { UserChatEvent, UserLeftEvent, UserStateEvent } from "./ConferenceEvents";
class Locker {
    constructor() {
        this.locks = new Set();
    }
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
    constructor(type, user) {
        super(type);
        this.user = user;
    }
}
export class RemoteUserIceErrorEvent extends RemoteUserEvent {
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
    constructor(user, candidate) {
        super("iceCandidate", user);
        this.candidate = candidate;
    }
}
export class RemoteUserOfferEvent extends RemoteUserEvent {
    constructor(user, offer) {
        super("offer", user);
        this.offer = offer;
    }
}
export class RemoteUserAnswerEvent extends RemoteUserEvent {
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
    constructor(user, track, stream) {
        super("trackAdded", user);
        this.track = track;
        this.stream = stream;
    }
}
export class RemoteUserTrackMutedEvent extends RemoteUserEvent {
    constructor(user, track) {
        super("trackMuted", user);
        this.track = track;
    }
}
export class RemoteUserTrackRemovedEvent extends RemoteUserEvent {
    constructor(user, track, stream) {
        super("trackRemoved", user);
        this.track = track;
        this.stream = stream;
    }
}
const seenUsers = new Set();
export class RemoteUser extends TypedEventTarget {
    constructor(userID, userName, rtcConfig) {
        super();
        this.userID = userID;
        this.userName = userName;
        this.userStateEvt = new UserStateEvent(this);
        this.userChatEvt = new UserChatEvent(this);
        this.transceivers = new Array();
        this.tasks = new Map();
        this.locks = new Locker();
        this.channel = null;
        this.gotOffer = false;
        this.disposed = false;
        this.trackSent = false;
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
                dispose(this.channel);
                this.channel = null;
            }
            for (const transceiver of this.transceivers) {
                transceiver.stop();
            }
            arrayClear(this.transceivers);
            dispose(this.connection);
            this.disposed = true;
        }
    }
    setChannel(channel) {
        this.channel = channel;
        this.channel.binaryType = "arraybuffer";
        this.channel.addEventListener("message", (evt) => {
            if (isArrayBuffer(evt.data)) {
                this.recvUserState(evt);
            }
        });
    }
    recvChat(text) {
        this.userChatEvt.text = text;
        this.dispatchEvent(this.userChatEvt);
    }
    async sendUserState(buffer) {
        if (this.channel && this.channel.readyState === "open") {
            const lockName = "sendUserState";
            if (!this.tasks.has(lockName)) {
                this.tasks.set(lockName, new Task());
            }
            await this.locks.withSkipLock(lockName, async () => {
                this.channel.send(buffer);
            });
        }
    }
    recvUserState(evt) {
        this.userStateEvt.buffer = evt.data;
        this.dispatchEvent(this.userStateEvt);
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
            this.dispatchEvent(new RemoteUserStreamNeededEvent(this));
        }
    }
    removeStream(stream) {
        const senders = this.connection.getSenders();
        const sendersByTrack = new Map(senders.map(s => [s.track, s]));
        for (const track of stream.getTracks()) {
            if (sendersByTrack.has(track)) {
                const sender = sendersByTrack.get(track);
                this.connection.removeTrack(sender);
            }
        }
    }
    sendStream(...streams) {
        for (const stream of streams) {
            if (stream) {
                for (const track of stream.getTracks()) {
                    if (this.trackSent) {
                        this.connection.addTrack(track, stream);
                    }
                    else {
                        this.trackSent = true;
                        this.transceivers.push(this.connection.addTransceiver(track, {
                            streams: [stream]
                        }));
                    }
                }
            }
        }
    }
}
//# sourceMappingURL=RemoteUser.js.map