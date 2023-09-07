import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import { IDisposable } from "@juniper-lib/tslib/using";
import { UserChatEvent, UserLeftEvent, UserStateEvent } from "./ConferenceEvents";
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
};
export declare class RemoteUser extends TypedEventTarget<RemoteUserEvents> implements IDisposable {
    readonly userID: string;
    userName: string;
    private readonly userStateEvt;
    private readonly userChatEvt;
    private readonly transceivers;
    private readonly tasks;
    private readonly locks;
    private readonly connection;
    private channel;
    private gotOffer;
    private disposed;
    private trackSent;
    constructor(userID: string, userName: string, rtcConfig: RTCConfiguration);
    dispose(): void;
    private setChannel;
    recvChat(text: string): void;
    sendUserState(buffer: ArrayBuffer): Promise<void>;
    private recvUserState;
    addIceCandidate(ice: RTCIceCandidate): Promise<void>;
    acceptOffer(offer: RTCSessionDescription): Promise<void>;
    acceptAnswer(answer: RTCSessionDescription): Promise<void>;
    start(): void;
    removeStream(stream: MediaStream): void;
    sendStream(...streams: MediaStream[]): void;
}
export {};
//# sourceMappingURL=RemoteUser.d.ts.map