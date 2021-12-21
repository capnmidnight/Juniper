import { Pose } from "juniper-audio";
import { PointerName } from "juniper-dom";
export class ConferenceEvent extends Event {
    eventType;
    constructor(eventType) {
        super(eventType);
        this.eventType = eventType;
    }
}
export class ConferenceErrorEvent extends ConferenceEvent {
    error;
    constructor(error) {
        super("error");
        this.error = error;
    }
}
export class ConferenceInfoEvent extends ConferenceEvent {
    message;
    constructor(message) {
        super("info");
        this.message = message;
    }
}
export class ConferenceServerConnectedEvent extends ConferenceEvent {
    constructor() {
        super("serverConnected");
    }
}
export class ConferenceServerDisconnectedEvent extends ConferenceEvent {
    constructor() {
        super("serverDisconnected");
    }
}
export class ConferenceServerFailedEvent extends ConferenceEvent {
    constructor() {
        super("serverFailed");
    }
}
export class LocalUserEvent extends ConferenceEvent {
    userID;
    constructor(type, userID) {
        super(type);
        this.userID = userID;
    }
}
export class RoomJoinedEvent extends LocalUserEvent {
    pose;
    constructor(userID, pose) {
        super("roomJoined", userID);
        this.pose = pose;
    }
}
export class RoomLeftEvent extends LocalUserEvent {
    constructor(userID) {
        super("roomLeft", userID);
    }
}
export class RemoteUserEvent extends ConferenceEvent {
    user;
    constructor(type, user) {
        super(type);
        this.user = user;
    }
}
export class UserJoinedEvent extends RemoteUserEvent {
    source;
    constructor(user, source) {
        super("userJoined", user);
        this.source = source;
    }
}
export class UserLeftEvent extends RemoteUserEvent {
    constructor(user) {
        super("userLeft", user);
    }
}
export class UserNameChangedEvent extends RemoteUserEvent {
    newUserName;
    constructor(user, newUserName) {
        super("userNameChanged", user);
        this.newUserName = newUserName;
    }
}
export class UserMutedEvent extends RemoteUserEvent {
    muted;
    constructor(type, user, muted) {
        super(type, user);
        this.muted = muted;
    }
}
export class UserAudioMutedEvent extends UserMutedEvent {
    constructor(user, muted) {
        super("audioMuteStatusChanged", user, muted);
    }
}
export class UserVideoMutedEvent extends UserMutedEvent {
    constructor(user, muted) {
        super("videoMuteStatusChanged", user, muted);
    }
}
export var StreamType;
(function (StreamType) {
    StreamType["Audio"] = "audio";
    StreamType["Video"] = "video";
})(StreamType || (StreamType = {}));
export var StreamOpType;
(function (StreamOpType) {
    StreamOpType["Added"] = "added";
    StreamOpType["Removed"] = "removed";
    StreamOpType["Changed"] = "changed";
})(StreamOpType || (StreamOpType = {}));
export class UserStreamEvent extends RemoteUserEvent {
    kind;
    op;
    stream;
    constructor(type, kind, op, user, stream) {
        super(type, user);
        this.kind = kind;
        this.op = op;
        this.stream = stream;
    }
}
export class UserStreamAddedEvent extends UserStreamEvent {
    constructor(type, kind, user, stream) {
        super(type, kind, StreamOpType.Added, user, stream);
    }
}
export class UserStreamRemovedEvent extends UserStreamEvent {
    constructor(type, kind, user, stream) {
        super(type, kind, StreamOpType.Removed, user, stream);
    }
}
export class UserAudioStreamAddedEvent extends UserStreamAddedEvent {
    constructor(user, stream) {
        super("audioAdded", StreamType.Audio, user, stream);
    }
}
export class UserAudioStreamRemovedEvent extends UserStreamRemovedEvent {
    constructor(user, stream) {
        super("audioRemoved", StreamType.Audio, user, stream);
    }
}
export class UserVideoStreamAddedEvent extends UserStreamAddedEvent {
    constructor(user, stream) {
        super("videoAdded", StreamType.Video, user, stream);
    }
}
export class UserVideoStreamRemovedEvent extends UserStreamRemovedEvent {
    constructor(user, stream) {
        super("videoRemoved", StreamType.Video, user, stream);
    }
}
export class UserPoseEvent extends RemoteUserEvent {
    pose = new Pose();
    constructor(type, user) {
        super(type, user);
    }
}
export class UserPosedEvent extends UserPoseEvent {
    height = 0;
    constructor(user) {
        super("userPosed", user);
    }
}
export class UserPointerEvent extends UserPoseEvent {
    name = PointerName.RemoteUser;
    constructor(user) {
        super("userPointer", user);
    }
}
export class UserChatEvent extends RemoteUserEvent {
    text;
    constructor(user, text) {
        super("chat", user);
        this.text = text;
    }
}
