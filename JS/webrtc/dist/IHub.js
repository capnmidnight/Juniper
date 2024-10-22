import { TypedEvent } from "@juniper-lib/events/dist/TypedEventTarget";
export class HubCloseEvent extends TypedEvent {
    constructor() {
        super("close");
    }
}
export class HubReconnectingEvent extends TypedEvent {
    constructor(error) {
        super("reconnecting");
        this.error = error;
    }
}
export class HubReconnectedEvent extends TypedEvent {
    constructor() {
        super("reconnected");
    }
}
export class HubUserJoinedEvent extends TypedEvent {
    constructor(fromUserID, fromUserName) {
        super("userJoined");
        this.fromUserID = fromUserID;
        this.fromUserName = fromUserName;
    }
}
export class HubIceReceivedEvent extends TypedEvent {
    constructor(fromUserID, candidateJSON) {
        super("iceReceived");
        this.fromUserID = fromUserID;
        this.candidateJSON = candidateJSON;
    }
}
export class HubOfferReceivedEvent extends TypedEvent {
    constructor(fromUserID, offerJSON) {
        super("offerReceived");
        this.fromUserID = fromUserID;
        this.offerJSON = offerJSON;
    }
}
export class HubAnswerReceivedEvent extends TypedEvent {
    constructor(fromUserID, answerJSON) {
        super("answerReceived");
        this.fromUserID = fromUserID;
        this.answerJSON = answerJSON;
    }
}
export class HubUserLeftEvent extends TypedEvent {
    constructor(fromUserID) {
        super("userLeft");
        this.fromUserID = fromUserID;
    }
}
export class HubUserChatEvent extends TypedEvent {
    constructor() {
        super("chat");
        this.fromUserID = null;
        this.text = null;
    }
    set(fromUserID, text) {
        this.fromUserID = fromUserID;
        this.text = text;
    }
}
//# sourceMappingURL=IHub.js.map