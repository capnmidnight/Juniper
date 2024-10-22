import { AudioManager, AudioPlayer, DeviceManager, LocalUserMicrophone } from "@juniper-lib/audio";
import { PriorityMap } from "@juniper-lib/collections";
import { CanvasTypes, CssColorValue } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
import { BaseAsset, IFetcher } from "@juniper-lib/fetcher";
import { ArtificialHorizon, BatteryImage, ClockImage, StatsImage } from "@juniper-lib/graphics2d";
import { IProgress } from "@juniper-lib/progress";
import { ISpeechRecognizer, SpeechRecognizerFactory } from "@juniper-lib/speech";
import { LocalUserWebcamElement } from "@juniper-lib/video";
import { ScreenUI } from "../../ScreenUI";
import { SpaceUI } from "../../SpaceUI";
import { VideoPlayer3D } from "../../VideoPlayer3D";
import { Watch } from "../../Watch";
import { InteractionAudio } from "../../eventSystem/InteractionAudio";
import { ButtonFactory } from "../../widgets/ButtonFactory";
import { ButtonImageWidget } from "../../widgets/ButtonImageWidget";
import { CanvasImageMesh } from "../../widgets/CanvasImageMesh";
import { ConfirmationDialogElement } from "../../widgets/ConfirmationDialog";
import { ScreenModeToggleButton } from "../../widgets/ScreenModeToggleButton";
import { TextMesh } from "../../widgets/TextMesh";
import { ToggleButton } from "../../widgets/ToggleButton";
import { Widget } from "../../widgets/widgets";
import { ApplicationLoader } from "./../ApplicationLoader";
import { BaseEnvironment } from "./../BaseEnvironment";
import { DeviceDialog } from "./../DeviceDialog";
import { XRTimerTickEvent } from "./../XRTimer";
export declare class EnvironmentRoomJoinedEvent extends TypedEvent<"roomjoined"> {
    readonly roomName: string;
    constructor(roomName: string);
}
export declare class DialogShowingEvent extends TypedEvent<"dialogshowing"> {
    readonly showing: boolean;
    constructor(showing: boolean);
}
export type EnvironmentEvents = {
    dialogshowing: DialogShowingEvent;
    environmentaudiotoggled: TypedEvent<"environmentaudiotoggled">;
    roomjoined: EnvironmentRoomJoinedEvent;
};
export interface EnvironmentOptions {
    canvas: CanvasTypes;
    fetcher: IFetcher;
    dialogFontFamily: string;
    getAppUrl: (name: string) => string;
    uiImagePaths: PriorityMap<string, string, string>;
    buttonFillColor: CssColorValue;
    labelFillColor: CssColorValue;
    defaultAvatarHeight?: number;
    defaultFOV?: number;
    enableFullResolution?: boolean;
    enableAnaglyph?: boolean;
    DEBUG?: boolean;
    watchModelPath?: string;
    styleSheetPath?: string;
    createSpeechRecognizer?: SpeechRecognizerFactory;
}
export interface EnvironmentConstructor {
    new (options: EnvironmentOptions): Environment;
}
export interface EnvironmentModule {
    default: EnvironmentConstructor;
}
export declare class Environment extends BaseEnvironment<EnvironmentEvents> {
    readonly audio: AudioManager;
    readonly interactionAudio: InteractionAudio;
    readonly microphones: LocalUserMicrophone;
    readonly webcams: LocalUserWebcamElement;
    readonly devices: DeviceManager;
    readonly speech: ISpeechRecognizer;
    readonly xrUI: SpaceUI;
    readonly screenUISpace: ScreenUI;
    readonly confirmationDialog: ConfirmationDialogElement;
    readonly compassImage: ArtificialHorizon;
    readonly clockImage: CanvasImageMesh<ClockImage>;
    readonly statsImage: CanvasImageMesh<StatsImage>;
    readonly watch: Watch;
    readonly batteryImage: CanvasImageMesh<BatteryImage>;
    readonly infoLabel: TextMesh;
    readonly menuButton: ButtonImageWidget;
    readonly subMenu: Widget;
    readonly settingsButton: ButtonImageWidget;
    readonly muteCamButton: ToggleButton;
    readonly muteMicButton: ToggleButton;
    readonly muteEnvAudioButton: ToggleButton;
    readonly quitButton: ButtonImageWidget;
    readonly arButton: ScreenModeToggleButton;
    readonly vrButton: ScreenModeToggleButton;
    readonly fullscreenButton: ScreenModeToggleButton;
    readonly anaglyphButton: ScreenModeToggleButton;
    readonly devicesDialog: DeviceDialog;
    readonly apps: ApplicationLoader;
    readonly uiButtons: ButtonFactory;
    readonly audioPlayer: AudioPlayer;
    readonly videoPlayer: VideoPlayer3D;
    private readonly envAudioToggleEvt;
    private _currentRoom;
    get currentRoom(): string;
    constructor(options: EnvironmentOptions);
    get canvas(): CanvasTypes;
    private createMenu;
    private layoutMenu;
    private countTick;
    private fpses;
    private avgFPS;
    preRender(evt: XRTimerTickEvent): void;
    get environmentAudioMuted(): boolean;
    withConfirmation(title: string, msg: string, act: () => void): Promise<void>;
    protected onConfirmationShowing(showing: boolean): void;
    load(prog: IProgress, ...assets: BaseAsset[]): Promise<void>;
    load(...assets: BaseAsset[]): Promise<void>;
}
//# sourceMappingURL=index.d.ts.map