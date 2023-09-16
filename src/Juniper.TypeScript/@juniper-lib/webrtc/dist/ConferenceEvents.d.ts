import { Pose } from "@juniper-lib/audio/dist/Pose";
import { AudioStreamSource } from "@juniper-lib/audio/dist/sources/AudioStreamSource";
import { RemoteUser, RemoteUserTrackAddedEvent, RemoteUserTrackRemovedEvent } from "./RemoteUser";
export type ConferenceEventTypes = "error" | "info" | "serverConnected" | "serverDisconnected" | "serverFailed" | "roomJoined" | "roomLeft" | "userJoined" | "userLeft" | "userNameChanged" | "audioMuteStatusChanged" | "videoMuteStatusChanged" | "audioAdded" | "audioRemoved" | "videoAdded" | "videoRemoved" | "userState" | "chat";
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
export declare class UserStateEvent extends RemoteUserEvent<"userState"> {
    buffer: ArrayBuffer;
    constructor(user: RemoteUser);
}
export declare class UserChatEvent extends RemoteUserEvent<"chat"> {
    text: string;
    constructor(user: RemoteUser);
}
export type ConferenceEvents = {
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
    trackAdded: RemoteUserTrackAddedEvent;
    trackRemoved: RemoteUserTrackRemovedEvent;
};
//# sourceMappingURL=ConferenceEvents.d.ts.map