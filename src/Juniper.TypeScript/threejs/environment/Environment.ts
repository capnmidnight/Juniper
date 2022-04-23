import { ArtificialHorizon } from "@juniper/2d/ArtificialHorizon";
import { BatteryImage } from "@juniper/2d/BatteryImage";
import { ClockImage } from "@juniper/2d/ClockImage";
import { AudioManager } from "@juniper/audio/AudioManager";
import { AudioPlayer } from "@juniper/audio/sources/AudioPlayer";
import type { CanvasTypes } from "@juniper/dom/canvas";
import { elementApply } from "@juniper/dom/tags";
import type { IFetcher } from "@juniper/fetcher";
import { hasVR, IProgress, isDesktop, isMobile, isMobileVR, PriorityMap, progressTasks, rad2deg, TimerTickEvent, TypedEvent } from "@juniper/tslib";
import { DEFAULT_LOCAL_USER_ID } from "@juniper/webrtc/constants";
import { ButtonFactory } from "../ButtonFactory";
import { ConfirmationDialog } from "../ConfirmationDialog";
import { InteractionAudio } from "../eventSystem/InteractionAudio";
import { ScreenMode } from "../ScreenMode";
import { ScreenUI } from "../ScreenUI";
import { SpaceUI } from "../SpaceUI";
import { VideoPlayer3D } from "../VideoPlayer3D";
import { ButtonImageWidget } from "../widgets/ButtonImageWidget";
import { CanvasImageMesh } from "../widgets/CanvasImageMesh";
import { ScreenModeToggleButton } from "../widgets/ScreenModeToggleButton";
import { ToggleButton } from "../widgets/ToggleButton";
import { widgetSetEnabled } from "../widgets/widgets";
import { ApplicationLoader } from "./ApplicationLoader";
import { BaseEnvironment } from "./BaseEnvironment";
import { DeviceDialog } from "./DeviceDialog";

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
    JS_EXT: string;
    DEBUG: boolean;
}

export class Environment
    extends BaseEnvironment<EnvironmentEvents> {

    readonly audio: AudioManager;
    readonly interactionAudio: InteractionAudio;

    readonly xrUI: SpaceUI;
    readonly screenUISpace = new ScreenUI();
    readonly confirmationDialog: ConfirmationDialog;
    readonly compassImage: CanvasImageMesh<ArtificialHorizon>;
    readonly clockImage: CanvasImageMesh<ClockImage>;
    readonly batteryImage: CanvasImageMesh<BatteryImage>;
    readonly settingsButton: ButtonImageWidget;
    readonly muteMicButton: ToggleButton;
    readonly muteEnvAudioButton: ToggleButton;
    readonly quitButton: ButtonImageWidget;
    readonly lobbyButton: ButtonImageWidget;
    //readonly arButton: ScreenModeToggleButton;
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

    readonly DEBUG: boolean;

    constructor(canvas: CanvasTypes,
        fetcher: IFetcher,
        dialogFontFamily: string,
        uiImagePaths: PriorityMap<string, string, string>,
        defaultAvatarHeight: number,
        enableFullResolution: boolean,
        options?: Partial<EnvironmentOptions>) {
        super(canvas, fetcher, defaultAvatarHeight, enableFullResolution);

        this.compassImage = new CanvasImageMesh(this, "Horizon", new ArtificialHorizon());
        this.compassImage.mesh.renderOrder = 5;

        this.clockImage = new CanvasImageMesh(this, "Clock", new ClockImage());
        this.clockImage.mesh.renderOrder = 5;

        options = options || {};
        const JS_EXT = options.JS_EXT || ".js";
        this.DEBUG = options.DEBUG || false;

        this.apps = new ApplicationLoader(this, JS_EXT);
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

        this.interactionAudio = new InteractionAudio(this.audio, this.eventSystem);

        this.confirmationDialog = new ConfirmationDialog(this, dialogFontFamily);
        this.devicesDialog = new DeviceDialog(this);

        elementApply(
            this.renderer.domElement.parentElement,
            this.screenUISpace,
            this.confirmationDialog,
            this.devicesDialog);

        this.uiButtons = new ButtonFactory(this.fetcher, uiImagePaths, 20);

        this.settingsButton = new ButtonImageWidget(this.uiButtons, "ui", "settings");
        this.quitButton = new ButtonImageWidget(this.uiButtons, "ui", "quit");
        this.lobbyButton = new ButtonImageWidget(this.uiButtons, "ui", "lobby");
        this.muteMicButton = new ToggleButton(this.uiButtons, "microphone", "mute", "unmute");
        this.muteEnvAudioButton = new ToggleButton(this.uiButtons, "environment-audio", "mute", "unmute");

        this.vrButton = new ScreenModeToggleButton(this.uiButtons, ScreenMode.VR);
        this.fullscreenButton = new ScreenModeToggleButton(this.uiButtons, ScreenMode.Fullscreen);
        //this.arButton = new ScreenModeToggleButton(this.uiButtons, ScreenMode.AR);

        this.xrUI = new SpaceUI();
        this.xrUI.addItem(this.clockImage, { x: -1, y: 1, scale: 1 });
        this.xrUI.addItem(this.quitButton, { x: 1, y: 1, scale: 0.5 });
        this.xrUI.addItem(this.confirmationDialog, { x: 0, y: 0, scale: 0.25 });
        this.xrUI.addItem(this.settingsButton, { x: -1, y: -1, scale: 0.5 });
        this.xrUI.addItem(this.muteMicButton, { x: -0.84, y: -1, scale: 0.5 });
        this.xrUI.addItem(this.muteEnvAudioButton, { x: -0.68, y: -1, scale: 0.5 });
        this.xrUI.addItem(this.lobbyButton, { x: -0.473, y: -1, scale: 0.5 });
        this.xrUI.addItem(this.vrButton, { x: 1, y: -1, scale: 0.5 });
        this.xrUI.addItem(this.fullscreenButton, { x: 1, y: -1, scale: 0.5 });
        //this.xrUI.addItem(this.arButton, { x: 1, y: -1, scale: 0.5 });

        this.worldUISpace.add(this.xrUI);

        elementApply(this.screenUISpace.topRowLeft, this.compassImage, this.clockImage);
        elementApply(this.screenUISpace.topRowRight, this.quitButton);
        elementApply(this.screenUISpace.bottomRowLeft, this.settingsButton, this.muteMicButton, this.muteEnvAudioButton, this.lobbyButton);
        elementApply(this.screenUISpace.bottomRowRight, this.fullscreenButton, this.vrButton); //, this.arButton);

        if (BatteryImage.isAvailable && isMobile()) {
            this.batteryImage = new CanvasImageMesh(this, "Battery", new BatteryImage());
            this.xrUI.addItem(this.batteryImage, { x: 0.75, y: -1, scale: 1 });
            elementApply(this.screenUISpace.topRowRight, this.batteryImage);
        }

        this.vrButton.visible = isDesktop() && hasVR() || isMobileVR();
        this.lobbyButton.visible = false;
        this.muteMicButton.visible = false;

        this.screenControl.setUI(this.screenUISpace, this.fullscreenButton, this.vrButton); //, this.arButton);

        this.refreshSpaceUI();

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

        const onSessionChange = () => this.refreshSpaceUI();
        this.screenControl.addEventListener("sessionstarted", onSessionChange);
        this.screenControl.addEventListener("sessionstopped", onSessionChange);

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

    private refreshSpaceUI() {
        this.xrUI.visible = this.renderer.xr.isPresenting
            || this.testSpaceLayout;
        this.clockImage.isVisible = this.xrUI.visible
            || this.DEBUG;
    }

    private _testSpaceLayout = false;
    get testSpaceLayout() {
        return this._testSpaceLayout;
    }

    set testSpaceLayout(v) {
        if (v !== this.testSpaceLayout) {
            this._testSpaceLayout = v;
            this.refreshSpaceUI();
        }
    }

    private countTick = 0;
    private fpses = new Array<number>();
    private avgFPS = 0;

    override preRender(evt: TimerTickEvent) {
        super.preRender(evt);

        this.audio.update();
        this.videoPlayer.update(evt.dt, evt.frame);

        this.compassImage.image.setPitchAndHeading(
            rad2deg(this.avatar.worldPitch),
            rad2deg(this.avatar.worldHeading));

        if (this.DEBUG) {
            const fps = Math.round(evt.fps);
            this.avgFPS += fps / 100;
            this.fpses.push(fps);
            if (this.fpses.length > 100) {
                const fps = this.fpses.shift();
                this.avgFPS -= fps / 100;
            }

            if ((++this.countTick) % 100 === 0) {
                this.clockImage.image.fps = this.avgFPS;
            }
        }


        this.confirmationDialog.update(evt.dt);

        for (const app of this.apps) {
            app.update(evt);
        }
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
        widgetSetEnabled(this.quitButton, !showing, "primary");
        widgetSetEnabled(this.lobbyButton, !showing, "primary");
    }

    override async load(prog?: IProgress) {
        await progressTasks(prog,
            (prog) => super.load(prog),
            (prog) => this.uiButtons.load(prog),
            (prog) => this.audio.loadBasicClip("footsteps", "/audio/TransitionFootstepAudio.mp3", 0.5, prog),
            (prog) => this.interactionAudio.load("enter", "/audio/basic_enter.mp3", 0.25, prog),
            (prog) => this.interactionAudio.load("exit", "/audio/basic_exit.mp3", 0.25, prog),
            (prog) => this.interactionAudio.load("error", "/audio/basic_error.mp3", 0.25, prog),
            (prog) => this.interactionAudio.load("click", "/audio/vintage_radio_button_pressed.mp3", 1, prog));
    }
}
