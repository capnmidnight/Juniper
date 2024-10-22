import { AudioStreamSource, Pose } from "@juniper-lib/audio";
import { RemoteUser, RemoteUserTrackAddedEvent, RemoteUserTrackRemovedEvent } from "./RemoteUser";

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
    | "userState"
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

export class UserStateEvent extends RemoteUserEvent<"userState"> {
    public buffer: ArrayBuffer = null;
    constructor(user: RemoteUser) {
        super("userState", user);
    }
}

export class UserChatEvent extends RemoteUserEvent<"chat"> {
    public text: string = null;
    constructor(user: RemoteUser) {
        super("chat", user);
    }
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
}