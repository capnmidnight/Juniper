import { arrayClear, arrayRemove, arrayScan, IDisposable, isArrayBuffer, Task, TypedEvent, TypedEventBase } from "@juniper-lib/tslib";
import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import { UserLeftEvent, UserPointerEvent, UserPosedEvent } from "./ConferenceEvents";

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

interface RemoteUserEvents {
    iceError: RemoteUserIceErrorEvent;
    iceCandidate: RemoteUserIceCandidateEvent;
    offer: RemoteUserOfferEvent;
    answer: RemoteUserAnswerEvent;
    streamNeeded: RemoteUserStreamNeededEvent;
    trackAdded: RemoteUserTrackAddedEvent;
    trackMuted: RemoteUserTrackMutedEvent;
    trackRemoved: RemoteUserTrackRemovedEvent;
    userPosed: UserPosedEvent;
    userPointer: UserPointerEvent;
    userLeft: UserLeftEvent;
}

const seenUsers = new Set<string>();

enum Message {
    Pointer = 1,
    Pose,
    InvocationComplete
}

export class RemoteUser extends TypedEventBase<RemoteUserEvents> implements IDisposable {

    private readonly userPosedEvt = new UserPosedEvent(this);
    private readonly userPointerEvt = new UserPointerEvent(this);

    private readonly sendPoseBuffer = new Float32Array(12);
    private readonly sendPointerBuffer = new Float32Array(12);
    private readonly sendInvocationCompleteBuffer = new Float32Array(2);

    private readonly transceivers = new Array<RTCRtpTransceiver>();

    private invocationCount = 0;
    private readonly invocations = new Map<number, () => void>();
    private readonly tasks = new Map<string, Task>();
    private readonly locks = new Locker<string>();

    private readonly connection: RTCPeerConnection;
    private channel: RTCDataChannel = null;

    private gotOffer = false;
    private disposed = false;
    private trackSent = false;

    constructor(public readonly userID: string, public userName: string, rtcConfig: RTCConfiguration, private readonly confirmReceipt: boolean) {
        super();

        this.sendPoseBuffer[0] = Message.Pose;
        this.sendPointerBuffer[0] = Message.Pointer;
        this.sendInvocationCompleteBuffer[0] = Message.InvocationComplete;

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

    private setChannel(channel: RTCDataChannel) {
        this.channel = channel;
        this.channel.binaryType = "arraybuffer";
        this.channel.addEventListener("message", (evt: MessageEvent<any>) => {
            if (isArrayBuffer(evt.data)) {
                const data = new Float32Array(evt.data);
                const msg = data[0] as Message;
                const invocationID = data[1];
                if (msg === Message.InvocationComplete) {
                    const resolve = this.invocations.get(invocationID);
                    if (resolve) {
                        resolve();
                    }
                }
                else if (this.channel && this.channel.readyState === "open") {
                    if (msg === Message.Pose) {
                        this.recvPose(
                            data[2], data[3], data[4],
                            data[5], data[6], data[7],
                            data[8], data[9], data[10],
                            data[11]);
                    }
                    else {
                        this.recvPointer(
                            data[11],
                            data[2], data[3], data[4],
                            data[5], data[6], data[7],
                            data[8], data[9], data[10]);
                    }

                    if (invocationID >= 0) {
                        this.sendInvocationCompleteBuffer[1] = invocationID;
                        this.channel.send(this.sendInvocationCompleteBuffer);
                    }
                }
            }
        });
    }

    recvPose(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number, height: number) {
        this.userPosedEvt.pose.set(px, py, pz, fx, fy, fz, ux, uy, uz);
        this.userPosedEvt.height = height;
        this.dispatchEvent(this.userPosedEvt);
    }

    sendPose(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number, height: number) {
        return this.sendMessage(Message.Pose, px, py, pz, fx, fy, fz, ux, uy, uz, height);
    }

    recvPointer(pointerName: PointerName, px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number) {
        this.userPointerEvt.name = pointerName;
        this.userPointerEvt.pose.set(px, py, pz, fx, fy, fz, ux, uy, uz);
        this.dispatchEvent(this.userPointerEvt);
    }

    sendPointer(pointerName: PointerName, px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number) {
        return this.sendMessage(Message.Pointer, px, py, pz, fx, fy, fz, ux, uy, uz, pointerName);
    }

    private async sendMessage(msgName: Message.Pose, px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number, height: number): Promise<void>;
    private async sendMessage(msgName: Message.Pointer, px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number, pointerName: PointerName): Promise<void>;
    private async sendMessage(msgName: Message, px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number, pointerNameOrHeight: number | PointerName): Promise<void> {
        if (this.channel && this.channel.readyState === "open") {
            let lockName = msgName === Message.Pose
                ? msgName.toString()
                : `${msgName}${pointerNameOrHeight}`;
            if (!this.tasks.has(lockName)) {
                this.tasks.set(lockName, new Task());
            }
            this.locks.withSkipLock(lockName, async () => {
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
                    const task = this.tasks.get(lockName);
                    task.reset();

                    const timeout = setTimeout(task.reject, 100, "timeout");
                    const onComplete = () => {
                        clearTimeout(timeout);
                        task.resolve();
                    };
                    this.invocations.set(invocationID, onComplete);

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
            this.trackSent = true;
            this.dispatchEvent(new RemoteUserStreamNeededEvent(this));
        }
    }

    sendStream(stream: MediaStream): void {
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
