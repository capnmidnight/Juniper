import { AudioManager } from "@juniper-lib/audio/AudioManager";
import { AudioPlayer } from "@juniper-lib/audio/sources/AudioPlayer";
import { CanvasTypes, isHTMLCanvas } from "@juniper-lib/dom/canvas";
import { elementApply } from "@juniper-lib/dom/tags";
import { AssetAudio, BaseAsset, isAsset } from "@juniper-lib/fetcher/Asset";
import { IFetcher } from "@juniper-lib/fetcher/IFetcher";
import { ArtificialHorizon } from "@juniper-lib/graphics2d/ArtificialHorizon";
import { BatteryImage } from "@juniper-lib/graphics2d/BatteryImage";
import { ClockImage } from "@juniper-lib/graphics2d/ClockImage";
import { Audio_Mpeg } from "@juniper-lib/mediatypes";
import { PriorityMap } from "@juniper-lib/tslib/collections/PriorityMap";
import { TypedEvent } from "@juniper-lib/tslib/events/EventBase";
import { hasVR, isDesktop, isMobile, isMobileVR } from "@juniper-lib/tslib/flags";
import { rad2deg } from "@juniper-lib/tslib/math";
import { IProgress } from "@juniper-lib/tslib/progress/IProgress";
import { DEFAULT_LOCAL_USER_ID } from "@juniper-lib/webrtc/constants";
import { InteractionAudio } from "../eventSystem/InteractionAudio";
import { objGraph } from "../objects";
import { ScreenMode } from "../ScreenMode";
import { ScreenUI } from "../ScreenUI";
import { SpaceUI } from "../SpaceUI";
import { VideoPlayer3D } from "../VideoPlayer3D";
import { ButtonFactory } from "../widgets/ButtonFactory";
import { ButtonImageWidget } from "../widgets/ButtonImageWidget";
import { CanvasImageMesh } from "../widgets/CanvasImageMesh";
import { ConfirmationDialog } from "../widgets/ConfirmationDialog";
import { ScreenModeToggleButton } from "../widgets/ScreenModeToggleButton";
import { TextMesh } from "../widgets/TextMesh";
import { ToggleButton } from "../widgets/ToggleButton";
import { widgetSetEnabled } from "../widgets/widgets";
import { ApplicationLoader } from "./ApplicationLoader";
import { BaseEnvironment } from "./BaseEnvironment";
import { DeviceDialog } from "./DeviceDialog";
import { XRTimerTickEvent } from "./XRTimer";

export class EnvironmentRoomJoinedEvent extends TypedEvent<"roomjoined"> {
    constructor(public readonly roomName: string) {
        super("roomjoined");
    }
}

export interface EnvironmentEvents {
    home: TypedEvent<"home">;
    environmentaudiotoggled: TypedEvent<"environmentaudiotoggled">;
    roomjoined: EnvironmentRoomJoinedEvent;
}

export interface EnvironmentOptions {
    DEBUG: boolean;
}

export interface EnvironmentConstructor {
    new(canvas: CanvasTypes,
        fetcher: IFetcher,
        dialogFontFamily: string,
        uiImagePaths: PriorityMap<string, string, string>,
        buttonFillColor: CSSColorValue,
        labelFillColor: CSSColorValue,
        defaultAvatarHeight: number,
        defaultFOV: number,
        enableFullResolution: boolean,
        options?: Partial<EnvironmentOptions>): Environment;
}

export interface EnvironmentModule {
    default: EnvironmentConstructor;
}

export class Environment
    extends BaseEnvironment<EnvironmentEvents> {

    readonly audio: AudioManager;
    readonly interactionAudio: InteractionAudio;

    readonly xrUI: SpaceUI;
    readonly screenUISpace: ScreenUI;
    readonly confirmationDialog: ConfirmationDialog;
    readonly compassImage: ArtificialHorizon;
    readonly clockImage: CanvasImageMesh<ClockImage>;
    readonly batteryImage: CanvasImageMesh<BatteryImage>;
    readonly infoLabel: TextMesh;
    readonly settingsButton: ButtonImageWidget;
    readonly muteMicButton: ToggleButton;
    readonly muteEnvAudioButton: ToggleButton;
    readonly quitButton: ButtonImageWidget;
    readonly lobbyButton: ButtonImageWidget;
    readonly arButton: ScreenModeToggleButton;
    readonly vrButton: ScreenModeToggleButton;
    readonly fullscreenButton: ScreenModeToggleButton;
    readonly devicesDialog: DeviceDialog;
    readonly apps: ApplicationLoader;
    readonly uiButtons: ButtonFactory;
    readonly audioPlayer: AudioPlayer;
    readonly videoPlayer: VideoPlayer3D;

    private readonly envAudioToggleEvt = new TypedEvent("environmentaudiotoggled");

    private _currentRoom: string = null;
    get currentRoom() {
        return this._currentRoom;
    }

    constructor(canvas: CanvasTypes,
        fetcher: IFetcher,
        dialogFontFamily: string,
        uiImagePaths: PriorityMap<string, string, string>,
        buttonFillColor: CSSColorValue,
        labelFillColor: CSSColorValue,
        defaultAvatarHeight: number,
        defaultFOV: number,
        enableFullResolution: boolean,
        options?: Partial<EnvironmentOptions>) {
        super(canvas, fetcher, defaultAvatarHeight, defaultFOV, enableFullResolution, options && options.DEBUG);
        this.screenUISpace = new ScreenUI(buttonFillColor);
        this.compassImage = new ArtificialHorizon();

        this.clockImage = new CanvasImageMesh(this, "Clock", "none", new ClockImage());
        this.clockImage.sizeMode = "fixed-height";
        this.clockImage.mesh.renderOrder = 5;

        this.infoLabel = new TextMesh(this, "InfoLabel", {
            minHeight: 0.1,
            maxHeight: 0.1,
            padding: 0.02,
            scale: 1000,
            bgFillColor: labelFillColor,
            textFillColor: "white"
        });

        options = options || {};

        this.apps = new ApplicationLoader(this);
        this.apps.addEventListener("apploading", (evt) => {
            evt.preLoadTask = this.fadeOut()
                .then(() => {
                    this.clearScene();
                    this.avatar.reset();
                });
        });

        this.apps.addEventListener("apploaded", (evt) => {
            evt.app.addEventListener("joinroom", (evt) => {
                if (evt.roomName !== this._currentRoom) {
                    this._currentRoom = evt.roomName;
                    this.dispatchEvent(new EnvironmentRoomJoinedEvent(evt.roomName));
                }
            });
        });

        this.apps.addEventListener("appshown", async (evt) => {
            this.lobbyButton.visible = evt.appName !== "menu";
            await this.fadeIn();
        });

        this.audio = new AudioManager(DEFAULT_LOCAL_USER_ID);
        this.audio.setAudioProperties(1, 4, "exponential");

        this.audioPlayer = new AudioPlayer(this.audio.audioCtx);

        this.videoPlayer = new VideoPlayer3D(this, this.audio.audioCtx);
        this.videoPlayer.object.visible = false;

        this.interactionAudio = new InteractionAudio(this.audio, this.eventSys);

        this.confirmationDialog = new ConfirmationDialog(this, dialogFontFamily);
        this.devicesDialog = new DeviceDialog(this);

        this.uiButtons = new ButtonFactory(uiImagePaths, 20, buttonFillColor, labelFillColor, this.DEBUG);

        this.settingsButton = new ButtonImageWidget(this.uiButtons, "ui", "settings");
        this.quitButton = new ButtonImageWidget(this.uiButtons, "ui", "quit");
        this.lobbyButton = new ButtonImageWidget(this.uiButtons, "ui", "lobby");
        this.muteMicButton = new ToggleButton(this.uiButtons, "microphone", "mute", "unmute");
        this.muteEnvAudioButton = new ToggleButton(this.uiButtons, "environment-audio", "mute", "unmute");
        this.muteEnvAudioButton.active = true;
        this.audio.ready.then(() => this.muteEnvAudioButton.active = false)

        this.vrButton = new ScreenModeToggleButton(this.uiButtons, ScreenMode.VR);
        this.fullscreenButton = new ScreenModeToggleButton(this.uiButtons, ScreenMode.Fullscreen);
        this.arButton = new ScreenModeToggleButton(this.uiButtons, ScreenMode.AR);

        this.xrUI = new SpaceUI();
        this.xrUI.addItem(this.clockImage, { x: -1, y: 1, height: 0.1 });
        this.xrUI.addItem(this.quitButton, { x: 1, y: 1, scale: 0.5 });
        this.xrUI.addItem(this.confirmationDialog, { x: 0, y: 0, scale: 0.25 });
        this.xrUI.addItem(this.settingsButton, { x: -1, y: -1, scale: 0.5 });
        this.xrUI.addItem(this.muteMicButton, { x: -0.84, y: -1, scale: 0.5 });
        this.xrUI.addItem(this.muteEnvAudioButton, { x: -0.68, y: -1, scale: 0.5 });
        this.xrUI.addItem(this.lobbyButton, { x: -0.473, y: -1, scale: 0.5 });
        this.xrUI.addItem(this.infoLabel, { x: 0.25, y: -1, scale: 0.5 })
        this.xrUI.addItem(this.vrButton, { x: 1, y: -1, scale: 0.5 });
        this.xrUI.addItem(this.fullscreenButton, { x: 1, y: -1, scale: 0.5 });
        this.xrUI.addItem(this.arButton, { x: 1, y: -1, scale: 0.5 });
        
        objGraph(this.worldUISpace, this.xrUI);

        elementApply(this.screenUISpace.topLeft, this.compassImage, this.clockImage);
        elementApply(this.screenUISpace.topRight, this.quitButton);
        elementApply(this.screenUISpace.bottomLeft, this.settingsButton, this.muteMicButton, this.muteEnvAudioButton, this.lobbyButton);
        elementApply(this.screenUISpace.bottomCenter, this.infoLabel);
        elementApply(this.screenUISpace.bottomRight, this.vrButton, this.arButton, this.fullscreenButton);

        if (BatteryImage.isAvailable && isMobile()) {
            this.batteryImage = new CanvasImageMesh(this, "Battery", "none", new BatteryImage());
            this.batteryImage.sizeMode = "fixed-height";
            this.xrUI.addItem(this.batteryImage, { x: 0.75, y: -1, width: 0.2, height: 0.1 });
            elementApply(this.screenUISpace.topRight, this.batteryImage);
        }

        this.vrButton.visible = isDesktop() && hasVR() || isMobileVR();
        this.arButton.visible = false;
        this.lobbyButton.visible = false;
        this.muteMicButton.visible = false;

        this.screenControl.setUI(this.screenUISpace, this.fullscreenButton, this.vrButton, this.arButton);

        this.quitButton.addEventListener("click", () =>
            this.withConfirmation(
                "Confirm quit",
                "Are you sure you want to quit?",
                async () => {
                    if (this.renderer.xr.isPresenting) {
                        this.screenControl.stop();
                    }

                    await this.onQuitting();
                }));

        this.lobbyButton.addEventListener("click", () =>
            this.withConfirmation(
                "Confirm return to lobby",
                "Are you sure you want to return to the lobby?",
                () =>
                    this.dispatchEvent(new TypedEvent("home"))));

        this.settingsButton.addEventListener("click", async () => {
            const mode = this.screenControl.currentMode;
            const wasPresenting = this.renderer.xr.isPresenting;
            if (wasPresenting) {
                await this.screenControl.stop();
            }
            await this.devicesDialog.showDialog();
            if (wasPresenting) {
                await this.screenControl.start(mode);
            }
        });

        this.muteEnvAudioButton.addEventListener("click", () => {
            this.muteEnvAudioButton.active = !this.muteEnvAudioButton.active;
            this.dispatchEvent(this.envAudioToggleEvt);
        });

        this.avatar.addEventListener("avatarmoved", (evt) =>
            this.audio.setUserPose(
                this.audio.localUserID,
                evt.px, evt.py, evt.pz,
                evt.fx, evt.fy, evt.fz,
                evt.ux, evt.uy, evt.uz));
    }

    private _testSpaceLayout = false;
    get testSpaceLayout() {
        return this._testSpaceLayout;
    }

    set testSpaceLayout(v) {
        if (v !== this.testSpaceLayout) {
            this._testSpaceLayout = v;

        }
    }

    private countTick = 0;
    private fpses = new Array<number>();
    private avgFPS = 0;

    override preRender(evt: XRTimerTickEvent) {
        super.preRender(evt);

        this.xrUI.visible = this.renderer.xr.isPresenting
            || this.testSpaceLayout;
        this.clockImage.isVisible = this.xrUI.visible
            || this.DEBUG;

        if (!this.renderer.xr.isPresenting) {
            this.compassImage.setPitchAndHeading(
                rad2deg(this.avatar.worldPitchRadians),
                rad2deg(this.avatar.worldHeadingRadians));
        }

        if (this.DEBUG) {
            const fps = Math.round(evt.fps);
            this.avgFPS += fps / 100;
            this.fpses.push(fps);
            if (this.fpses.length > 100) {
                const fps = this.fpses.shift();
                this.avgFPS -= fps / 100;
            }

            if ((++this.countTick) % 100 === 0) {
                this.clockImage.image.setStats(
                    this.avgFPS,
                    this.renderer.info.render.calls,
                    this.renderer.info.render.triangles);
            }
        }

        this.audio.update();
        this.confirmationDialog.update(evt.dt);
    }

    get environmentAudioMuted() {
        return this.muteEnvAudioButton.active;
    }

    async withConfirmation(title: string, msg: string, act: () => void) {
        this.onConfirmationShowing(true);
        if (await this.confirmationDialog.prompt(title, msg)) {
            act();
        }
        this.onConfirmationShowing(false);
    }

    protected onConfirmationShowing(showing: boolean) {
        widgetSetEnabled(this.quitButton, !showing);
        widgetSetEnabled(this.lobbyButton, !showing);
    }

    override async load(prog: IProgress, ...assets: BaseAsset[]): Promise<void>;
    override async load(...assets: BaseAsset[]): Promise<void>;
    override async load(progOrAsset: IProgress | BaseAsset, ...assets: BaseAsset[]): Promise<void> {
        let prog: IProgress = null;
        if (isAsset(progOrAsset)) {
            assets.push(progOrAsset);
            prog = this.loadingBar;
        }
        else {
            prog = progOrAsset
        }

        if (isHTMLCanvas(this.renderer.domElement)) {
            elementApply(
                this.renderer.domElement.parentElement,
                this.screenUISpace,
                this.renderer.domElement);
        }

        const footsteps = new AssetAudio("/audio/footsteps.mp3", Audio_Mpeg, !this.DEBUG);
        const enter = new AssetAudio("/audio/basic_enter.mp3", Audio_Mpeg, !this.DEBUG);
        const exit = new AssetAudio("/audio/basic_exit.mp3", Audio_Mpeg, !this.DEBUG);
        const error = new AssetAudio("/audio/basic_error.mp3", Audio_Mpeg, !this.DEBUG);
        const click = new AssetAudio("/audio/vintage_radio_button_pressed.mp3", Audio_Mpeg, !this.DEBUG);

        assets.push(...this.uiButtons.assets, footsteps, enter, exit, error, click);

        await super.load(prog, ...assets);

        this.audio.createBasicClip("footsteps", footsteps.result, 0.5);
        this.interactionAudio.create("enter", enter.result, 0.25);
        this.interactionAudio.create("exit", exit.result, 0.25);
        this.interactionAudio.create("error", error.result, 0.25);
        this.interactionAudio.create("click", click.result, 1);
    }
}
