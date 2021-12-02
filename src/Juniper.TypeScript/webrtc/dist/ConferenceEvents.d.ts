import { AudioStreamSource, Pose } from "juniper-audio";
import { PointerName } from "juniper-tslib";
import { RemoteUser } from "./RemoteUser";
export declare type ConferenceEventTypes = "error" | "info" | "serverConnected" | "serverDisconnected" | "serverFailed" | "roomJoined" | "roomLeft" | "userJoined" | "userLeft" | "userNameChanged" | "audioMuteStatusChanged" | "videoMuteStatusChanged" | "audioAdded" | "audioRemoved" | "videoAdded" | "videoRemoved" | "userPosed" | "userPointer" | "chat";
export declare class ConferenceEvent<T extends ConferenceEventTypes> extends Event {
    eventType: T;
    constructor(eventType: T);
}
export declare class ConferenceErrorEvent extends ConferenceEvent<"error"> {
    readonly error: Error;
    constructor(error: Error);
}
export declare class ConferenceInfoEvent extends ConferenceEvent<"info"> {
    readonly message: string;
    constructor(message: string);
}
export declare class ConferenceServerConnectedEvent extends ConferenceEvent<"serverConnected"> {
    constructor();
}
export declare class ConferenceServerDisconnectedEvent extends ConferenceEvent<"serverDisconnected"> {
    constructor();
}
export declare class ConferenceServerFailedEvent extends ConferenceEvent<"serverFailed"> {
    constructor();
}
export declare class LocalUserEvent<T extends ConferenceEventTypes> extends ConferenceEvent<T> {
    readonly userID: string;
    constructor(type: T, userID: string);
}
export declare class RoomJoinedEvent extends LocalUserEvent<"roomJoined"> {
    pose: Pose;
    constructor(userID: string, pose: Pose);
}
export declare class RoomLeftEvent extends LocalUserEvent<"roomLeft"> {
    constructor(userID: string);
}
export declare class RemoteUserEvent<T extends ConferenceEventTypes> extends ConferenceEvent<T> {
    user: RemoteUser;
    constructor(type: T, user: RemoteUser);
}
export declare class UserJoinedEvent extends RemoteUserEvent<"userJoined"> {
    readonly source: AudioStreamSource;
    constructor(user: RemoteUser, source: AudioStreamSource);
}
export declare class UserLeftEvent extends RemoteUserEvent<"userLeft"> {
    constructor(user: RemoteUser);
}
export declare class UserNameChangedEvent extends RemoteUserEvent<"userNameChanged"> {
    readonly newUserName: string;
    constructor(user: RemoteUser, newUserName: string);
}
export declare class UserMutedEvent<T extends ConferenceEventTypes> extends RemoteUserEvent<T> {
    readonly muted: boolean;
    constructor(type: T, user: RemoteUser, muted: boolean);
}
export declare class UserAudioMutedEvent extends UserMutedEvent<"audioMuteStatusChanged"> {
    constructor(user: RemoteUser, muted: boolean);
}
export declare class UserVideoMutedEvent extends UserMutedEvent<"videoMuteStatusChanged"> {
    constructor(user: RemoteUser, muted: boolean);
}
export declare enum StreamType {
    Audio = "audio",
    Video = "video"
}
export declare enum StreamOpType {
    Added = "added",
    Removed = "removed",
    Changed = "changed"
}
export declare class UserStreamEvent<T extends ConferenceEventTypes> extends RemoteUserEvent<T> {
    kind: StreamType;
    op: StreamOpType;
    stream: MediaStream;
    constructor(type: T, kind: StreamType, op: StreamOpType, user: RemoteUser, stream: MediaStream);
}
export declare class UserStreamAddedEvent<T extends ConferenceEventTypes> extends UserStreamEvent<T> {
    constructor(type: T, kind: StreamType, user: RemoteUser, stream: MediaStream);
}
export declare class UserStreamRemovedEvent<T extends ConferenceEventTypes> extends UserStreamEvent<T> {
    constructor(type: T, kind: StreamType, user: RemoteUser, stream: MediaStream);
}
export declare class UserAudioStreamAddedEvent extends UserStreamAddedEvent<"audioAdded"> {
    constructor(user: RemoteUser, stream: MediaStream);
}
export declare class UserAudioStreamRemovedEvent extends UserStreamRemovedEvent<"audioRemoved"> {
    constructor(user: RemoteUser, stream: MediaStream);
}
export declare class UserVideoStreamAddedEvent extends UserStreamAddedEvent<"videoAdded"> {
    constructor(user: RemoteUser, stream: MediaStream);
}
export declare class UserVideoStreamRemovedEvent extends UserStreamRemovedEvent<"videoRemoved"> {
    constructor(user: RemoteUser, stream: MediaStream);
}
export declare class UserPoseEvent<T extends ConferenceEventTypes> extends RemoteUserEvent<T> {
    readonly pose: Pose;
    constructor(type: T, user: RemoteUser);
}
export declare class UserPosedEvent extends UserPoseEvent<"userPosed"> {
    height: number;
    constructor(user: RemoteUser);
}
export declare class UserPointerEvent extends UserPoseEvent<"userPointer"> {
    name: PointerName;
    constructor(user: RemoteUser);
}
export declare class UserChatEvent extends RemoteUserEvent<"chat"> {
    text: string;
    constructor(user: RemoteUser, text: string);
}
export interface ConferenceEvents {
    error: ConferenceErrorEvent;
    info: ConferenceInfoEvent;
    serverConnected: ConferenceServerConnectedEvent;
    serverDisconnected: ConferenceServerDisconnectedEvent;
    serverFailed: ConferenceServerFailedEvent;
    audioMuteStatusChanged: UserAudioMutedEvent;
    videoMuteStatusChanged: UserVideoMutedEvent;
    roomJoined: RoomJoinedEvent;
    roomLeft: RoomLeftEvent;
    userJoined: UserJoinedEvent;
    userLeft: UserLeftEvent;
    userNameChanged: UserNameChangedEvent;
    audioAdded: UserAudioStreamAddedEvent;
    videoAdded: UserVideoStreamAddedEvent;
    audioRemoved: UserAudioStreamRemovedEvent;
    videoRemoved: UserVideoStreamRemovedEvent;
    chat: UserChatEvent;
}
