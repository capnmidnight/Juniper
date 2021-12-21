import { PointerName } from "juniper-dom";
import { IDisposable, TypedEvent, TypedEventBase } from "juniper-tslib";
import { UserLeftEvent, UserPointerEvent, UserPosedEvent } from "./ConferenceEvents";
declare class RemoteUserEvent<T extends string> extends TypedEvent<T> {
    readonly user: RemoteUser;
    constructor(type: T, user: RemoteUser);
}
export declare class RemoteUserIceErrorEvent extends RemoteUserEvent<"iceError"> {
    readonly address: string;
    readonly errorCode: number;
    readonly errorText: string;
    readonly port: number;
    readonly url: string;
    constructor(user: RemoteUser, evt: RTCPeerConnectionIceErrorEvent);
}
export declare class RemoteUserIceCandidateEvent extends RemoteUserEvent<"iceCandidate"> {
    readonly candidate: RTCIceCandidate;
    constructor(user: RemoteUser, candidate: RTCIceCandidate);
}
export declare class RemoteUserOfferEvent extends RemoteUserEvent<"offer"> {
    readonly offer: RTCSessionDescription;
    constructor(user: RemoteUser, offer: RTCSessionDescription);
}
export declare class RemoteUserAnswerEvent extends RemoteUserEvent<"answer"> {
    readonly answer: RTCSessionDescription;
    constructor(user: RemoteUser, answer: RTCSessionDescription);
}
export declare class RemoteUserStreamNeededEvent extends RemoteUserEvent<"streamNeeded"> {
    constructor(user: RemoteUser);
}
export declare class RemoteUserTrackAddedEvent extends RemoteUserEvent<"trackAdded"> {
    readonly track: MediaStreamTrack;
    readonly stream: MediaStream;
    constructor(user: RemoteUser, track: MediaStreamTrack, stream: MediaStream);
}
export declare class RemoteUserTrackMutedEvent extends RemoteUserEvent<"trackMuted"> {
    readonly track: MediaStreamTrack;
    constructor(user: RemoteUser, track: MediaStreamTrack);
}
export declare class RemoteUserTrackRemovedEvent extends RemoteUserEvent<"trackRemoved"> {
    readonly track: MediaStreamTrack;
    readonly stream: MediaStream;
    constructor(user: RemoteUser, track: MediaStreamTrack, stream: MediaStream);
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
export declare class RemoteUser extends TypedEventBase<RemoteUserEvents> implements IDisposable {
    readonly userID: string;
    userName: string;
    private readonly confirmReceipt;
    private readonly userPosedEvt;
    private readonly userPointerEvt;
    private readonly sendPoseBuffer;
    private readonly sendPointerBuffer;
    private readonly sendInvocationCompleteBuffer;
    private readonly transceivers;
    private invocationCount;
    private readonly invocations;
    private readonly locks;
    private readonly connection;
    private channel;
    private gotOffer;
    private disposed;
    private trackSent;
    constructor(userID: string, userName: string, rtcConfig: RTCConfiguration, confirmReceipt: boolean);
    dispose(): void;
    private setChannel;
    recvPose(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number, height: number): void;
    sendPose(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number, height: number): Promise<void>;
    recvPointer(pointerName: PointerName, px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;
    sendPointer(pointerName: PointerName, px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): Promise<void>;
    private sendMessage;
    addIceCandidate(ice: RTCIceCandidate): Promise<void>;
    acceptOffer(offer: RTCSessionDescription): Promise<void>;
    acceptAnswer(answer: RTCSessionDescription): Promise<void>;
    start(): void;
    sendStream(stream: MediaStream): void;
}
export {};
