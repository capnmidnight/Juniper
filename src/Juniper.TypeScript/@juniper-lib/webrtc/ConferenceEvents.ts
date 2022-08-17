import { Pose } from "@juniper-lib/audio/Pose";
import { AudioStreamSource } from "@juniper-lib/audio/sources/AudioStreamSource";
import { PointerID } from "@juniper-lib/tslib/events/Pointers";
import { RemoteUser } from "./RemoteUser";

export type ConferenceEventTypes = "error"
    | "info"
    | "serverConnected"
    | "serverDisconnected"
    | "serverFailed"
    | "roomJoined"
    | "roomLeft"
    | "userJoined"
    | "userLeft"
    | "userNameChanged"
    | "audioMuteStatusChanged"
    | "videoMuteStatusChanged"
    | "audioAdded"
    | "audioRemoved"
    | "videoAdded"
    | "videoRemoved"
    | "userPosed"
    | "userPointer"
    | "chat";

export class ConferenceEvent<T extends ConferenceEventTypes> extends Event {
    constructor(public eventType: T) {
        super(eventType);
    }
}

export class ConferenceErrorEvent
    extends ConferenceEvent<"error"> {
    constructor(public readonly error: Error) {
        super("error");
    }
}

export class ConferenceInfoEvent
    extends ConferenceEvent<"info"> {
    constructor(public readonly message: string) {
        super("info");
    }
}

export class ConferenceServerConnectedEvent
    extends ConferenceEvent<"serverConnected"> {
    constructor() {
        super("serverConnected");
    }
}

export class ConferenceServerDisconnectedEvent
    extends ConferenceEvent<"serverDisconnected"> {
    constructor() {
        super("serverDisconnected");
    }
}

export class ConferenceServerFailedEvent
    extends ConferenceEvent<"serverFailed"> {
    constructor() {
        super("serverFailed");
    }
}

export class LocalUserEvent<T extends ConferenceEventTypes> extends ConferenceEvent<T> {
    constructor(type: T, public readonly userID: string) {
        super(type);
    }
}

export class RoomJoinedEvent extends LocalUserEvent<"roomJoined"> {
    constructor(userID: string, public pose: Pose) {
        super("roomJoined", userID);
    }
}

export class RoomLeftEvent extends LocalUserEvent<"roomLeft"> {
    constructor(userID: string) {
        super("roomLeft", userID);
    }
}

export class RemoteUserEvent<T extends ConferenceEventTypes> extends ConferenceEvent<T> {
    constructor(type: T, public user: RemoteUser) {
        super(type);
    }
}

export class UserJoinedEvent extends RemoteUserEvent<"userJoined"> {
    constructor(user: RemoteUser, public readonly source: AudioStreamSource) {
        super("userJoined", user);
    }
}

export class UserLeftEvent extends RemoteUserEvent<"userLeft"> {
    constructor(user: RemoteUser) {
        super("userLeft", user);
    }
}

export class UserNameChangedEvent extends RemoteUserEvent<"userNameChanged"> {
    constructor(user: RemoteUser, public readonly newUserName: string) {
        super("userNameChanged", user);
    }
}

export class UserMutedEvent<T extends ConferenceEventTypes> extends RemoteUserEvent<T> {
    constructor(type: T, user: RemoteUser, public readonly muted: boolean) {
        super(type, user);
    }
}

export class UserAudioMutedEvent extends UserMutedEvent<"audioMuteStatusChanged"> {
    constructor(user: RemoteUser, muted: boolean) {
        super("audioMuteStatusChanged", user, muted);
    }
}

export class UserVideoMutedEvent extends UserMutedEvent<"videoMuteStatusChanged"> {
    constructor(user: RemoteUser, muted: boolean) {
        super("videoMuteStatusChanged", user, muted);
    }
}

export enum StreamType {
    Audio = "audio",
    Video = "video"
}

export enum StreamOpType {
    Added = "added",
    Removed = "removed",
    Changed = "changed"
}

export class UserStreamEvent<T extends ConferenceEventTypes> extends RemoteUserEvent<T> {
    constructor(type: T, public kind: StreamType, public op: StreamOpType, user: RemoteUser, public stream: MediaStream) {
        super(type, user);
    }
}

export class UserStreamAddedEvent<T extends ConferenceEventTypes> extends UserStreamEvent<T> {
    constructor(type: T, kind: StreamType, user: RemoteUser, stream: MediaStream) {
        super(type, kind, StreamOpType.Added, user, stream);
    }
}

export class UserStreamRemovedEvent<T extends ConferenceEventTypes> extends UserStreamEvent<T> {
    constructor(type: T, kind: StreamType, user: RemoteUser, stream: MediaStream) {
        super(type, kind, StreamOpType.Removed, user, stream);
    }
}

export class UserAudioStreamAddedEvent extends UserStreamAddedEvent<"audioAdded"> {
    constructor(user: RemoteUser, stream: MediaStream) {
        super("audioAdded", StreamType.Audio, user, stream);
    }
}

export class UserAudioStreamRemovedEvent extends UserStreamRemovedEvent<"audioRemoved"> {
    constructor(user: RemoteUser, stream: MediaStream) {
        super("audioRemoved", StreamType.Audio, user, stream);
    }
}

export class UserVideoStreamAddedEvent extends UserStreamAddedEvent<"videoAdded"> {
    constructor(user: RemoteUser, stream: MediaStream) {
        super("videoAdded", StreamType.Video, user, stream);
    }
}

export class UserVideoStreamRemovedEvent extends UserStreamRemovedEvent<"videoRemoved"> {
    constructor(user: RemoteUser, stream: MediaStream) {
        super("videoRemoved", StreamType.Video, user, stream);
    }
}

export class UserPoseEvent<T extends ConferenceEventTypes> extends RemoteUserEvent<T> {

    public readonly pose = new Pose();

    constructor(type: T, user: RemoteUser) {
        super(type, user);
    }
}

export class UserPosedEvent extends UserPoseEvent<"userPosed"> {

    public height = 0;

    constructor(user: RemoteUser) {
        super("userPosed", user);
    }
}

export class UserPointerEvent extends UserPoseEvent<"userPointer"> {

    public pointerID = PointerID.RemoteUser;

    constructor(user: RemoteUser) {
        super("userPointer", user);
    }
}

export class UserChatEvent extends RemoteUserEvent<"chat"> {
    constructor(user: RemoteUser, public text: string) {
        super("chat", user);
    }
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