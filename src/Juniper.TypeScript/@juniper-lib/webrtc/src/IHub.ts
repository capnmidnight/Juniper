import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
import { ConnectionState } from "./ConnectionState";

export class HubCloseEvent extends TypedEvent<"close">{
    constructor() {
        super("close");
    }
}

export class HubReconnectingEvent extends TypedEvent<"reconnecting">{
    constructor(public readonly error: Error) {
        super("reconnecting");
    }
}

export class HubReconnectedEvent extends TypedEvent<"reconnected"> {
    constructor() {
        super("reconnected");
    }
}

export class HubUserJoinedEvent extends TypedEvent<"userJoined"> {
    constructor(public readonly fromUserID: string, public readonly fromUserName: string) {
        super("userJoined");
    }
}

export class HubIceReceivedEvent extends TypedEvent<"iceReceived">{
    constructor(public readonly fromUserID: string, public readonly candidateJSON: string) {
        super("iceReceived");
    }
}

export class HubOfferReceivedEvent extends TypedEvent<"offerReceived">{
    constructor(public readonly fromUserID: string, public readonly offerJSON: string) {
        super("offerReceived");
    }
}

export class HubAnswerReceivedEvent extends TypedEvent<"answerReceived">{
    constructor(public readonly fromUserID: string, public readonly answerJSON: string) {
        super("answerReceived");
    }
}

export class HubUserLeftEvent extends TypedEvent<"userLeft">{
    constructor(public readonly fromUserID: string) {
        super("userLeft");
    }
}

export class HubUserChatEvent extends TypedEvent<"chat">{
    public fromUserID: string = null;
    public text: string = null;
    constructor() {
        super("chat");
    }

    set(fromUserID: string, text: string) {
        this.fromUserID = fromUserID;
        this.text = text;
    }
}

export type IHubEvents = {
    close: HubCloseEvent;
    reconnecting: HubReconnectingEvent;
    reconnected: HubReconnectedEvent;
    userJoined: HubUserJoinedEvent;
    iceReceived: HubIceReceivedEvent;
    offerReceived: HubOfferReceivedEvent;
    answerReceived: HubAnswerReceivedEvent;
    userLeft: HubUserLeftEvent;
    chat: HubUserChatEvent;
}


export interface IHub extends TypedEventTarget<IHubEvents> {
    readonly connectionState: ConnectionState;
    start(): Promise<void>;
    stop(): Promise<void>;
    invoke<T>(methodName: string, ...params: any[]): Promise<T>;
}
