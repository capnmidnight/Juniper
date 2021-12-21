import type { AudioManager, DeviceManagerAudioInputChangedEvent } from "juniper-audio";
import { PointerName } from "juniper-dom";
import type { IDisposable } from "juniper-tslib";
import { TypedEventBase } from "juniper-tslib";
import { ConferenceEvents } from "./ConferenceEvents";
import { ConnectionState } from "./ConnectionState";
import { DecayingGain } from "./DecayingGain";
export declare const DEFAULT_LOCAL_USER_ID = "local-user";
export declare enum ClientState {
    InConference = "in-conference",
    JoiningConference = "joining-conference",
    Connected = "connected",
    Connecting = "connecting",
    Prepaired = "prepaired",
    Prepairing = "prepairing",
    Unprepared = "unprepaired"
}
export declare class TeleconferenceManager extends TypedEventBase<ConferenceEvents> implements IDisposable {
    readonly audio: AudioManager;
    private readonly signalRPath;
    private readonly autoSetPosition;
    readonly needsVideoDevice: boolean;
    toggleLogging(): void;
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
    private localStreamIn;
    private readonly hub;
    private readonly remoteGainDecay;
    private _ready;
    get ready(): Promise<void>;
    constructor(audio: AudioManager, signalRPath: string, autoSetPosition?: boolean, needsVideoDevice?: boolean);
    private startInternal;
    get connectionState(): ConnectionState;
    get echoControl(): DecayingGain;
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
    private sendIce;
    private sendOffer;
    private sendAnswer;
    setLocalPose(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number, height: number): Promise<void>;
    setLocalPointer(name: PointerName, px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): Promise<void>;
    private onRemoteUserPosed;
    chat(text: string): void;
    userExists(id: string): boolean;
    getUserNames(): [string, string][];
    protected onAudioInputChanged(evt: DeviceManagerAudioInputChangedEvent): Promise<void>;
    private startStream;
    private restartStream;
    get localStream(): MediaStream;
    set localStream(v: MediaStream);
    private isMediaMuted;
    private setMediaMuted;
    get audioMuted(): boolean;
    set audioMuted(muted: boolean);
    get videoMuted(): boolean;
    set videoMuted(muted: boolean);
}
