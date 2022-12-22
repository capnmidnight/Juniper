import { AudioManager } from "@juniper-lib/audio/AudioManager";
import { AudioPlayer } from "@juniper-lib/audio/sources/AudioPlayer";
import { id } from "@juniper-lib/dom/attrs";
import { CanvasTypes, isHTMLCanvas } from "@juniper-lib/dom/canvas";
import { display, em, flexDirection, gap } from "@juniper-lib/dom/css";
import { isModifierless } from "@juniper-lib/dom/evts";
import { Div, elementApply } from "@juniper-lib/dom/tags";
import { AssetFile, AssetStyleSheet, BaseAsset, isAsset } from "@juniper-lib/fetcher/Asset";
import { IFetcher } from "@juniper-lib/fetcher/IFetcher";
import { ArtificialHorizon } from "@juniper-lib/graphics2d/ArtificialHorizon";
import { AudioGraphDialog } from "@juniper-lib/graphics2d/AudioGraphDialog";
import { BatteryImage } from "@juniper-lib/graphics2d/BatteryImage";
import { ClockImage } from "@juniper-lib/graphics2d/ClockImage";
import { StatsImage } from "@juniper-lib/graphics2d/StatsImage";
import { Audio_Mpeg } from "@juniper-lib/mediatypes";
import { PriorityMap } from "@juniper-lib/tslib/collections/PriorityMap";
import { all } from "@juniper-lib/tslib/events/all";
import { TypedEvent } from "@juniper-lib/tslib/events/EventBase";
import { hasVR, isDesktop, isMobile, isMobileVR } from "@juniper-lib/tslib/flags";
import { rad2deg } from "@juniper-lib/tslib/math";
import { IProgress } from "@juniper-lib/tslib/progress/IProgress";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { DEFAULT_LOCAL_USER_ID } from "@juniper-lib/webrtc/constants";
import { InteractionAudio } from "../../eventSystem/InteractionAudio";
import { obj, objGraph } from "../../objects";
import { ScreenMode } from "../../ScreenMode";
import { ScreenUI } from "../../ScreenUI";
import { SpaceUI } from "../../SpaceUI";
import { VideoPlayer3D } from "../../VideoPlayer3D";
import { Watch } from "../../Watch";
import { ButtonFactory } from "../../widgets/ButtonFactory";
import { ButtonImageWidget } from "../../widgets/ButtonImageWidget";
import { CanvasImageMesh } from "../../widgets/CanvasImageMesh";
import { ConfirmationDialog } from "../../widgets/ConfirmationDialog";
import { ScreenModeToggleButton } from "../../widgets/ScreenModeToggleButton";
import { TextMesh } from "../../widgets/TextMesh";
import { ToggleButton } from "../../widgets/ToggleButton";
import { Widget, widgetApply, widgetSetEnabled } from "../../widgets/widgets";
import { ApplicationLoader } from "./../ApplicationLoader";
import { BaseEnvironment } from "./../BaseEnvironment";
import { DeviceDialog } from "./../DeviceDialog";
import { XRTimerTickEvent } from "./../XRTimer";


import "./style.css";

export class EnvironmentRoomJoinedEvent extends TypedEvent<"roomjoined"> {
    constructor(public readonly roomName: string) {
        super("roomjoined");
    }
}

export class DialogShowingEvent extends TypedEvent<"dialogshowing"> {
    constructor(public readonly showing: boolean) {
        super("dialogshowing");
    }
}

export interface EnvironmentEvents {
    dialogshowing: DialogShowingEvent;
    environmentaudiotoggled: TypedEvent<"environmentaudiotoggled">;
    roomjoined: EnvironmentRoomJoinedEvent;
}

export interface EnvironmentOptions {
    DEBUG: boolean;
    watchModelPath: string;
    styleSheetPath: string;
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
    readonly statsImage: CanvasImageMesh<StatsImage>;
    readonly watch: Watch = null;
    readonly batteryImage: CanvasImageMesh<BatteryImage>;
    readonly infoLabel: TextMesh;
    readonly menuButton: ButtonImageWidget;
    readonly subMenu: Widget;
    readonly settingsButton: ButtonImageWidget;
    readonly muteMicButton: ToggleButton;
    readonly muteEnvAudioButton: ToggleButton;
    readonly quitButton: ButtonImageWidget;
    readonly arButton: ScreenModeToggleButton;
    readonly vrButton: ScreenModeToggleButton;
    readonly fullscreenButton: ScreenModeToggleButton;
    readonly devicesDialog: DeviceDialog;
    readonly apps: ApplicationLoader;
    readonly uiButtons: ButtonFactory;
    readonly audioPlayer: AudioPlayer;
    readonly videoPlayer: VideoPlayer3D;
    readonly graph: AudioGraphDialog;

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
        private readonly options?: Partial<EnvironmentOptions>) {

        options = options || {};

        super(canvas, fetcher, defaultAvatarHeight, defaultFOV, enableFullResolution, options && options.DEBUG);


        this.screenUISpace = new ScreenUI(buttonFillColor);
        this.compassImage = new ArtificialHorizon();

        this.clockImage = new CanvasImageMesh(this, "Clock", "none", new ClockImage());
        this.clockImage.sizeMode = "fixed-height";
        this.clockImage.mesh.renderOrder = 5;

        this.statsImage = new CanvasImageMesh(this, "Stats", "none", new StatsImage());
        this.statsImage.sizeMode = "fixed-height";
        this.statsImage.mesh.renderOrder = 5;

        this.infoLabel = new TextMesh(this, "InfoLabel", {
            minHeight: 0.1,
            maxHeight: 0.1,
            padding: 0.02,
            scale: 1000,
            bgFillColor: labelFillColor,
            textFillColor: "white"
        });

        this.apps = new ApplicationLoader(this);

        this.apps.addEventListener("apploaded", (evt) => {
            evt.app.addEventListener("joinroom", (evt) => {
                console.log("Join", evt.roomName, this._currentRoom);
                if (evt.roomName !== this._currentRoom) {
                    this._currentRoom = evt.roomName;
                    this.dispatchEvent(new EnvironmentRoomJoinedEvent(evt.roomName));
                }
            });
        });

        this.audio = new AudioManager(this.fetcher, DEFAULT_LOCAL_USER_ID);

        this.graph = new AudioGraphDialog(this.audio.context);
        if (isHTMLCanvas(canvas)) {
            canvas.addEventListener("keypress", (evt) => {
                if (isModifierless(evt) && evt.key === "`") {
                    this.graph.showDialog();
                }
            });
        }

        this.audioPlayer = new AudioPlayer(this.audio.context, this.audio.createSpatializer(false, false));

        this.videoPlayer = new VideoPlayer3D(this, this.audio.context, this.audio.createSpatializer(false, false));
        this.videoPlayer.object.visible = false;

        this.interactionAudio = new InteractionAudio(this.audio, this.eventSys);

        this.confirmationDialog = new ConfirmationDialog(this, dialogFontFamily);
        this.devicesDialog = new DeviceDialog(this);

        this.uiButtons = new ButtonFactory(uiImagePaths, 20, buttonFillColor, labelFillColor, this.DEBUG);

        this.menuButton = new ButtonImageWidget(this.uiButtons, "ui", "menu");
        this.settingsButton = new ButtonImageWidget(this.uiButtons, "ui", "settings");
        this.quitButton = new ButtonImageWidget(this.uiButtons, "ui", "quit");
        this.muteMicButton = new ToggleButton(this.uiButtons, "microphone", "mute", "unmute");
        this.muteEnvAudioButton = new ToggleButton(this.uiButtons, "environment-audio", "mute", "unmute");
        this.muteEnvAudioButton.active = true;
        this.audio.ready.then(() => this.muteEnvAudioButton.active = false)

        this.vrButton = new ScreenModeToggleButton(this.uiButtons, ScreenMode.VR);
        this.fullscreenButton = new ScreenModeToggleButton(this.uiButtons, ScreenMode.Fullscreen);
        this.arButton = new ScreenModeToggleButton(this.uiButtons, ScreenMode.AR);

        if (BatteryImage.isAvailable && isMobile()) {
            this.batteryImage = new CanvasImageMesh(this, "Battery", "none", new BatteryImage());
            this.batteryImage.sizeMode = "fixed-height";
        }

        this.xrUI = new SpaceUI();

        this.subMenu = new Widget(
            Div(id("juniperSubMenu")),
            obj("sub-menu"),
            "flex"
        );

        this.createMenu();

        this.screenControl.setUI(this.screenUISpace, this.fullscreenButton, this.vrButton, this.arButton);

        this.avatar.addEventListener("avatarmoved", (evt) =>
            this.audio.setUserPose(
                this.audio.localUserID,
                evt.px, evt.py, evt.pz,
                evt.fx, evt.fy, evt.fz,
                evt.ux, evt.uy, evt.uz));

        if (isDefined(this.screenControl)) {
            this.screenControl.addEventListener("sessionstarted", (evt) => {
                if (evt.mode === ScreenMode.Fullscreen && this.confirmationDialog.element.parentElement !== this.screenControl.fullscreenElement) {
                    elementApply(this.screenControl.fullscreenElement, this.devicesDialog, this.confirmationDialog);
                }
            });
        }

        if (isDefined(options.watchModelPath)) {
            this.watch = new Watch(this, options.watchModelPath);
        }
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

    private createMenu() {

        if (isDefined(this.batteryImage)) {
            this.xrUI.addItem(this.batteryImage, { x: 0.75, y: -1, width: 0.2, height: 0.1 });
            elementApply(this.screenUISpace.topRight, this.batteryImage);
        }

        this.xrUI.addItem(this.clockImage, { x: -1, y: 1, height: 0.1 });
        this.xrUI.addItem(this.statsImage, { x: -1, y: 0.95, height: 0.1 });
        this.xrUI.addItem(this.confirmationDialog, { x: 0, y: 0, scale: 0.25 });
        this.xrUI.addItem(this.menuButton, { x: -1, y: -1, scale: 0.5 });
        this.xrUI.addItem(this.infoLabel, { x: 0, y: -1.125, scale: 0.5 })
        this.xrUI.addItem(this.vrButton, { x: 1, y: -1, scale: 0.5 });
        this.xrUI.addItem(this.arButton, { x: 1, y: -1, scale: 0.5 });

        objGraph(this.menuButton, this.subMenu);

        objGraph(this.worldUISpace, this.xrUI);

        elementApply(this.screenUISpace.topLeft, this.compassImage, this.statsImage);
        elementApply(this.screenUISpace.bottomCenter, this.infoLabel);
        elementApply(this.screenUISpace.bottomRight, this.vrButton, this.arButton, this.fullscreenButton);
        elementApply(this.screenUISpace.bottomLeft,
            Div(this.menuButton,
                display("flex"),
                flexDirection("column-reverse"),
                gap(em(.25)),
                this.subMenu
            )
        );

        widgetApply(this.subMenu,
            this.settingsButton,
            this.muteMicButton,
            this.muteEnvAudioButton,
            this.quitButton
        );

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

        this.menuButton.addEventListener("click", () =>
            this.subMenu.visible = !this.subMenu.visible);

        [
            this.settingsButton,
            this.muteMicButton,
            this.muteEnvAudioButton,
            this.quitButton
        ].forEach(btn =>
            btn.addEventListener("click", () =>
                this.subMenu.visible = false));

        this.subMenu.visible = false;

        this.vrButton.visible = isDesktop() && hasVR() || isMobileVR();
        this.arButton.visible = false;
        this.muteMicButton.visible = false;
    }

    private layoutMenu() {
        let curCount = 0;
        for (const child of this.subMenu.object.children) {
            if (child.visible) {
                child.position.set(0, ++curCount * 0.25, 0);
            }
        }
    }

    private countTick = 0;
    private fpses = new Array<number>();
    private avgFPS = 0;

    override preRender(evt: XRTimerTickEvent) {
        super.preRender(evt);

        this.xrUI.visible = this.renderer.xr.isPresenting
            || this.testSpaceLayout;
        this.statsImage.isVisible = this.xrUI.visible
            && this.DEBUG;

        if (!this.renderer.xr.isPresenting) {
            this.compassImage.setPitchAndHeading(
                rad2deg(this.avatar.worldPitchRadians),
                rad2deg(this.avatar.worldHeadingRadians));
        }

        this.layoutMenu();


        if (this.DEBUG) {
            const fps = Math.round(evt.fps);
            this.avgFPS += fps / 100;
            this.fpses.push(fps);
            if (this.fpses.length > 100) {
                const fps = this.fpses.shift();
                this.avgFPS -= fps / 100;
            }

            if ((++this.countTick) % 100 === 0) {
                this.statsImage.image.setStats(
                    this.avgFPS,
                    this.renderer.info.render.calls,
                    this.renderer.info.render.triangles);
            }
        }

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
        this.dispatchEvent(new DialogShowingEvent(showing));
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

        const footsteps = new AssetFile("/audio/footsteps.mp3", Audio_Mpeg, !this.DEBUG);
        const enter = new AssetFile("/audio/basic_enter.mp3", Audio_Mpeg, !this.DEBUG);
        const exit = new AssetFile("/audio/basic_exit.mp3", Audio_Mpeg, !this.DEBUG);
        const error = new AssetFile("/audio/basic_error.mp3", Audio_Mpeg, !this.DEBUG);
        const click = new AssetFile("/audio/vintage_radio_button_pressed.mp3", Audio_Mpeg, !this.DEBUG);

        assets.push(...this.uiButtons.assets, footsteps, enter, exit, error, click);
        if (isDefined(this.watch)) {
            assets.push(this.watch.asset);
        }

        if (isDefined(this.options.styleSheetPath)) {
            assets.push(new AssetStyleSheet(this.options.styleSheetPath, !this.DEBUG));
        }

        await super.load(prog, ...assets);

        await all(
            this.audio.createBasicClip("footsteps", footsteps, 0.5),
            this.interactionAudio.create("enter", enter, 0.25),
            this.interactionAudio.create("exit", exit, 0.25),
            this.interactionAudio.create("error", error, 0.25),
            this.interactionAudio.create("click", click, 1)
        );

        this.screenUISpace.show();
    }
}
