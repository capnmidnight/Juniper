import { arrayClear, arrayRemove, arrayScan } from "@juniper-lib/collections/arrays";
import { TypedEvent, TypedEventBase } from "@juniper-lib/events/TypedEventBase";
import { Task } from "@juniper-lib/events/Task";
import { isArrayBuffer } from "@juniper-lib/tslib/typeChecks";
import { IDisposable, dispose } from "@juniper-lib/tslib/using";
import { UserChatEvent, UserLeftEvent, UserStateEvent } from "./ConferenceEvents";

class Locker<T> {
    private locks = new Set<T>();

    isLocked(name: T) {
        return this.locks.has(name);
    }

    isUnlocked(name: T) {
        return !this.locks.has(name);
    }

    lock(name: T) {
        this.locks.add(name);
    }

    unlock(name: T) {
        this.locks.delete(name);
    }

    async withSkipLock(name: T, act: () => any): Promise<void> {
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

class RemoteUserEvent<T extends string> extends TypedEvent<T>{
    constructor(type: T, public readonly user: RemoteUser) {
        super(type);
    }
}

export class RemoteUserIceErrorEvent extends RemoteUserEvent<"iceError"> {
    readonly address: string;
    readonly errorCode: number;
    readonly errorText: string;
    readonly port: number;
    readonly url: string;

    constructor(user: RemoteUser, evt: RTCPeerConnectionIceErrorEvent) {
        super("iceError", user);

        this.address = evt.address;
        this.errorCode = evt.errorCode;
        this.errorText = evt.errorText;
        this.port = evt.port;
        this.url = evt.url;

        Object.seal(this);
    }
}

export class RemoteUserIceCandidateEvent extends RemoteUserEvent<"iceCandidate"> {
    constructor(user: RemoteUser, public readonly candidate: RTCIceCandidate) {
        super("iceCandidate", user);
    }
}

export class RemoteUserOfferEvent extends RemoteUserEvent<"offer"> {
    constructor(user: RemoteUser, public readonly offer: RTCSessionDescription) {
        super("offer", user);
    }
}

export class RemoteUserAnswerEvent extends RemoteUserEvent<"answer"> {
    constructor(user: RemoteUser, public readonly answer: RTCSessionDescription) {
        super("answer", user);
    }
}

export class RemoteUserStreamNeededEvent extends RemoteUserEvent<"streamNeeded"> {
    constructor(user: RemoteUser) {
        super("streamNeeded", user);
    }
}

export class RemoteUserTrackAddedEvent extends RemoteUserEvent<"trackAdded"> {
    constructor(user: RemoteUser, public readonly track: MediaStreamTrack, public readonly stream: MediaStream) {
        super("trackAdded", user);
    }
}

export class RemoteUserTrackMutedEvent extends RemoteUserEvent<"trackMuted"> {
    constructor(user: RemoteUser, public readonly track: MediaStreamTrack) {
        super("trackMuted", user);
    }
}

export class RemoteUserTrackRemovedEvent extends RemoteUserEvent<"trackRemoved"> {
    constructor(user: RemoteUser, public readonly track: MediaStreamTrack, public readonly stream: MediaStream) {
        super("trackRemoved", user);
    }
}

type RemoteUserEvents = {
    iceError: RemoteUserIceErrorEvent;
    iceCandidate: RemoteUserIceCandidateEvent;
    offer: RemoteUserOfferEvent;
    answer: RemoteUserAnswerEvent;
    streamNeeded: RemoteUserStreamNeededEvent;
    trackAdded: RemoteUserTrackAddedEvent;
    trackMuted: RemoteUserTrackMutedEvent;
    trackRemoved: RemoteUserTrackRemovedEvent;
    userState: UserStateEvent;
    userLeft: UserLeftEvent;
    chat: UserChatEvent;
}

const seenUsers = new Set<string>();

export class RemoteUser extends TypedEventBase<RemoteUserEvents> implements IDisposable {

    private readonly userStateEvt = new UserStateEvent(this);
    private readonly userChatEvt = new UserChatEvent(this);

    private readonly transceivers = new Array<RTCRtpTransceiver>();

    private readonly tasks = new Map<string, Task>();
    private readonly locks = new Locker<string>();

    private readonly connection: RTCPeerConnection;
    private channel: RTCDataChannel = null;

    private gotOffer = false;
    private disposed = false;
    private trackSent = false;

    constructor(public readonly userID: string, public userName: string, rtcConfig: RTCConfiguration) {
        super();

        this.connection = new RTCPeerConnection(rtcConfig);
        this.connection.addEventListener("icecandidateerror", (evt: Event) => {
            this.dispatchEvent(new RemoteUserIceErrorEvent(this, evt as RTCPeerConnectionIceErrorEvent));
        });

        this.connection.addEventListener("icecandidate", async (evt: RTCPeerConnectionIceEvent) => {
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

        this.connection.addEventListener("datachannel", (evt: RTCDataChannelEvent) => {
            this.setChannel(evt.channel);
        });

        this.connection.addEventListener("track", async (evt: RTCTrackEvent) => {
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

    private setChannel(channel: RTCDataChannel) {
        this.channel = channel;
        this.channel.binaryType = "arraybuffer";
        this.channel.addEventListener("message", (evt: MessageEvent<any>) => {
            if (isArrayBuffer(evt.data)) {
                this.recvUserState(evt);
            }
        });
    }

    recvChat(text: string) {
        this.userChatEvt.text = text;
        this.dispatchEvent(this.userChatEvt);
    }

    async sendUserState(buffer: ArrayBuffer): Promise<void> {
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

    private recvUserState(evt: MessageEvent<any>) {
        this.userStateEvt.buffer = evt.data;
        this.dispatchEvent(this.userStateEvt);
    }

    async addIceCandidate(ice: RTCIceCandidate): Promise<void> {
        await this.connection.addIceCandidate(ice);
    }

    async acceptOffer(offer: RTCSessionDescription): Promise<void> {
        await this.connection.setRemoteDescription(offer);
        this.gotOffer = true;
        const answer = await this.connection.createAnswer();
        await this.connection.setLocalDescription(answer);
        this.dispatchEvent(new RemoteUserAnswerEvent(this, this.connection.localDescription.toJSON()));
    }

    async acceptAnswer(answer: RTCSessionDescription): Promise<void> {
        await this.connection.setRemoteDescription(answer);
    }

    start() {
        if (!this.trackSent) {
            this.dispatchEvent(new RemoteUserStreamNeededEvent(this));
        }
    }

    removeStream(stream: MediaStream): void {
        const senders = this.connection.getSenders();
        const sendersByTrack = new Map<MediaStreamTrack, RTCRtpSender>(senders.map(s => [s.track, s]));
        for (const track of stream.getTracks()) {
            if (sendersByTrack.has(track)) {
                const sender = sendersByTrack.get(track);
                this.connection.removeTrack(sender);
            }
        }
    }

    sendStream(...streams: MediaStream[]): void {
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
