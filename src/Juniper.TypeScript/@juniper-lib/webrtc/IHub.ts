import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { PointerID } from "@juniper-lib/tslib/events/Pointers";
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

export class HubUserPosedEvent extends TypedEvent<"userPosed">{
    public fromUserID: string = null;
    public px: number = null;
    public py: number = null;
    public pz: number = null;
    public fx: number = null;
    public fy: number = null;
    public fz: number = null;
    public ux: number = null;
    public uy: number = null;
    public uz: number = null;
    public height: number = null;

    constructor() {
        super("userPosed");
    }

    set(fromUserID: string,
        px: number, py: number, pz: number,
        fx: number, fy: number, fz: number,
        ux: number, uy: number, uz: number,
        height: number) {
        this.fromUserID = fromUserID;
        this.px = px;
        this.py = py;
        this.pz = pz;
        this.fx = fx;
        this.fy = fy;
        this.fz = fz;
        this.ux = ux;
        this.uy = uy;
        this.uz = uz;
        this.height = height;
    }
}

export class HubUserPointerEvent extends TypedEvent<"userPointer">{
    public fromUserID: string = null;
    public pointerID: PointerID = null;
    public px: number = null;
    public py: number = null;
    public pz: number = null;
    public fx: number = null;
    public fy: number = null;
    public fz: number = null;
    public ux: number = null;
    public uy: number = null;
    public uz: number = null;

    constructor() {
        super("userPointer");
    }

    set(fromUserID: string,
        pointerID: PointerID,
        px: number, py: number, pz: number,
        fx: number, fy: number, fz: number,
        ux: number, uy: number, uz: number) {
        this.fromUserID = fromUserID;
        this.pointerID = pointerID;
        this.px = px;
        this.py = py;
        this.pz = pz;
        this.fx = fx;
        this.fy = fy;
        this.fz = fz;
        this.ux = ux;
        this.uy = uy;
        this.uz = uz;
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

export interface IHubEvents {
    close: HubCloseEvent;
    reconnecting: HubReconnectingEvent;
    reconnected: HubReconnectedEvent;
    userJoined: HubUserJoinedEvent;
    iceReceived: HubIceReceivedEvent;
    offerReceived: HubOfferReceivedEvent;
    answerReceived: HubAnswerReceivedEvent;
    userLeft: HubUserLeftEvent;
    userPosed: HubUserPosedEvent;
    userPointer: HubUserPointerEvent;
    chat: HubUserChatEvent;
}


export interface IHub extends TypedEventBase<IHubEvents> {
    readonly connectionState: ConnectionState;
    start(): Promise<void>;
    stop(): Promise<void>;
    invoke<T>(methodName: string, ...params: any[]): Promise<T>;
}
