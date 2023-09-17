import { AudioManager } from "@juniper-lib/audio/dist/AudioManager";
import { LocalUserMicrophone } from "@juniper-lib/audio/dist/LocalUserMicrophone";
import { TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
import { IDisposable } from "@juniper-lib/tslib/dist/using";
import { LocalUserWebcam } from "@juniper-lib/video";
import "webrtc-adapter";
import { ConferenceEvents } from "./ConferenceEvents";
import { ConnectionState } from "./ConnectionState";
import { GainDecayer } from "./GainDecayer";
import { IHub } from "./IHub";
export declare enum ClientState {
    InConference = "in-conference",
    JoiningConference = "joining-conference",
    Connected = "connected",
    Connecting = "connecting",
    Prepaired = "prepaired",
    Prepairing = "prepairing",
    Unprepared = "unprepaired"
}
export declare class TeleconferenceManager extends TypedEventTarget<ConferenceEvents> implements IDisposable {
    private readonly audio;
    private readonly microphones;
    private readonly webcams;
    private readonly hub;
    private _isAudioMuted;
    get isAudioMuted(): boolean;
    private _isVideoMuted;
    get isVideoMuted(): boolean;
    private _localUserID;
    get localUserID(): string;
    private _localUserName;
    get localUserName(): string;
    private _roomName;
    get roomName(): string;
    private _conferenceState;
    private _hasAudioPermission;
    get hasAudioPermission(): boolean;
    private _hasVideoPermission;
    get hasVideoPermission(): boolean;
    private lastRoom;
    private lastUserID;
    private users;
    private readonly remoteGainDecay;
    private readonly windowQuitter;
    private heartbeatTimer;
    constructor(audio: AudioManager, microphones: LocalUserMicrophone, webcams: LocalUserWebcam, hub: IHub);
    get connectionState(): ConnectionState;
    get echoControl(): GainDecayer;
    get isConnected(): boolean;
    get conferenceState(): ConnectionState;
    get isConferenced(): boolean;
    protected setConferenceState(state: ConnectionState): void;
    private disposed;
    dispose(): void;
    private err;
    private toServer;
    private toRoom;
    private toUser;
    connect(): Promise<void>;
    join(roomName: string): Promise<void>;
    identify(userName: string): Promise<void>;
    leave(): Promise<void>;
    disconnect(): Promise<void>;
    private onClose;
    private onReconnecting;
    private onReconnected;
    private onUserJoined;
    private getRTCConfiguration;
    private onIceError;
    private onIceCandidate;
    private onIceReceived;
    private onOfferCreated;
    private onOfferReceived;
    private onAnswerCreated;
    private onAnswerReceived;
    private onStreamNeeded;
    private onTrackAdded;
    private onTrackMuted;
    private onTrackRemoved;
    private onUserLeft;
    private removeUser;
    private onChat;
    sendChat(text: string): void;
    private sendIce;
    private sendOffer;
    private sendAnswer;
    private forEachUser;
    sendUserState(buffer: ArrayBuffer): Promise<void>;
    userExists(id: string): boolean;
    getUserNames(): [string, string][];
}
//# sourceMappingURL=TeleconferenceManager.d.ts.map