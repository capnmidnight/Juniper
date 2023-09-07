import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import { ConnectionState } from "./ConnectionState";
export declare class HubCloseEvent extends TypedEvent<"close"> {
    constructor();
}
export declare class HubReconnectingEvent extends TypedEvent<"reconnecting"> {
    readonly error: Error;
    constructor(error: Error);
}
export declare class HubReconnectedEvent extends TypedEvent<"reconnected"> {
    constructor();
}
export declare class HubUserJoinedEvent extends TypedEvent<"userJoined"> {
    readonly fromUserID: string;
    readonly fromUserName: string;
    constructor(fromUserID: string, fromUserName: string);
}
export declare class HubIceReceivedEvent extends TypedEvent<"iceReceived"> {
    readonly fromUserID: string;
    readonly candidateJSON: string;
    constructor(fromUserID: string, candidateJSON: string);
}
export declare class HubOfferReceivedEvent extends TypedEvent<"offerReceived"> {
    readonly fromUserID: string;
    readonly offerJSON: string;
    constructor(fromUserID: string, offerJSON: string);
}
export declare class HubAnswerReceivedEvent extends TypedEvent<"answerReceived"> {
    readonly fromUserID: string;
    readonly answerJSON: string;
    constructor(fromUserID: string, answerJSON: string);
}
export declare class HubUserLeftEvent extends TypedEvent<"userLeft"> {
    readonly fromUserID: string;
    constructor(fromUserID: string);
}
export declare class HubUserChatEvent extends TypedEvent<"chat"> {
    fromUserID: string;
    text: string;
    constructor();
    set(fromUserID: string, text: string): void;
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
};
export interface IHub extends TypedEventTarget<IHubEvents> {
    readonly connectionState: ConnectionState;
    start(): Promise<void>;
    stop(): Promise<void>;
    invoke<T>(methodName: string, ...params: any[]): Promise<T>;
}
//# sourceMappingURL=IHub.d.ts.map