export class ConferenceEvent extends Event {
    constructor(eventType) {
        super(eventType);
        this.eventType = eventType;
    }
}
export class ConferenceErrorEvent extends ConferenceEvent {
    constructor(error) {
        super("error");
        this.error = error;
    }
}
export class ConferenceInfoEvent extends ConferenceEvent {
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
    constructor(type, userID) {
        super(type);
        this.userID = userID;
    }
}
export class RoomJoinedEvent extends LocalUserEvent {
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
    constructor(type, user) {
        super(type);
        this.user = user;
    }
}
export class UserJoinedEvent extends RemoteUserEvent {
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
    constructor(user, newUserName) {
        super("userNameChanged", user);
        this.newUserName = newUserName;
    }
}
export class UserMutedEvent extends RemoteUserEvent {
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
export class UserStateEvent extends RemoteUserEvent {
    constructor(user) {
        super("userState", user);
        this.buffer = null;
    }
}
export class UserChatEvent extends RemoteUserEvent {
    constructor(user) {
        super("chat", user);
        this.text = null;
    }
}
//# sourceMappingURL=ConferenceEvents.js.map