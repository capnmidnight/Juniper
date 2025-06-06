import { isDefined, isFunction, isNullOrUndefined, rad2deg } from "@juniper-lib/util";
import { AudioManager, AudioPlayer, DeviceManager, LocalUserMicrophone } from "@juniper-lib/audio";
import { display, Div, em, flexDirection, gap, hasVR, HtmlRender, ID, isDesktop, isHTMLCanvas, isMobile, isMobileVR, perc, pointerEvents, rule, SingletonStyleBlob, transform, width } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
import { AssetFile, isAsset } from "@juniper-lib/fetcher";
import { ArtificialHorizon, BatteryImage, ClockImage, StatsImage } from "@juniper-lib/graphics2d";
import { Audio_Mpeg } from "@juniper-lib/mediatypes";
import { LocalUserWebcam } from "@juniper-lib/video";
import { DEFAULT_LOCAL_USER_ID } from "@juniper-lib/webrtc";
import { ScreenMode } from "../../ScreenMode";
import { ScreenUI } from "../../ScreenUI";
import { SpaceUI } from "../../SpaceUI";
import { VideoPlayer3D } from "../../VideoPlayer3D";
import { Watch } from "../../Watch";
import { InteractionAudio } from "../../eventSystem/InteractionAudio";
import { obj, objGraph } from "../../objects";
import { ButtonFactory } from "../../widgets/ButtonFactory";
import { ButtonImageWidget } from "../../widgets/ButtonImageWidget";
import { CanvasImageMesh } from "../../widgets/CanvasImageMesh";
import { ConfirmationDialog, EnvironmentAttr } from "../../widgets/ConfirmationDialog";
import { ScreenModeToggleButton } from "../../widgets/ScreenModeToggleButton";
import { TextMesh } from "../../widgets/TextMesh";
import { ToggleButton } from "../../widgets/ToggleButton";
import { Widget, widgetApply, widgetSetEnabled } from "../../widgets/widgets";
import { ApplicationLoader } from "./../ApplicationLoader";
import { BaseEnvironment } from "./../BaseEnvironment";
import { DeviceDialog } from "./../DeviceDialog";
export class EnvironmentRoomJoinedEvent extends TypedEvent {
    constructor(roomName) {
        super("roomjoined");
        this.roomName = roomName;
    }
}
export class DialogShowingEvent extends TypedEvent {
    constructor(showing) {
        super("dialogshowing");
        this.showing = showing;
    }
}
export class Environment extends BaseEnvironment {
    get currentRoom() {
        return this._currentRoom;
    }
    constructor(options) {
        if (isNullOrUndefined(options)) {
            throw new Error("Options are now required");
        }
        if (isNullOrUndefined(options.canvas)) {
            throw new Error("options.canvas is required");
        }
        if (isNullOrUndefined(options.fetcher)) {
            throw new Error("options.fetcher is required");
        }
        if (isNullOrUndefined(options.dialogFontFamily)) {
            throw new Error("options.dialogFontFamily is required");
        }
        if (isNullOrUndefined(options.getAppUrl)) {
            throw new Error("options.getAppUrl is required");
        }
        if (isNullOrUndefined(options.uiImagePaths)) {
            throw new Error("options.uiImagePaths is required");
        }
        if (isNullOrUndefined(options.buttonFillColor)) {
            throw new Error("options.buttonFillColor is required");
        }
        if (isNullOrUndefined(options.labelFillColor)) {
            throw new Error("options.labelFillColor is required");
        }
        super(options.canvas, options.styleSheetPath, options.fetcher, options.enableFullResolution, options.enableAnaglyph, options.DEBUG, options.defaultAvatarHeight, options.defaultFOV);
        this.envAudioToggleEvt = new TypedEvent("environmentaudiotoggled");
        this._currentRoom = null;
        this.countTick = 0;
        this.fpses = new Array();
        this.avgFPS = 0;
        SingletonStyleBlob("Juniper::ThreeJS::Environment", () => rule("#juniperSubMenu", display("none"), flexDirection("column-reverse"), gap(em(.25))));
        this.screenUISpace = new ScreenUI(options.buttonFillColor);
        this.compassImage = new ArtificialHorizon();
        this.clockImage = new CanvasImageMesh(this, "Clock", "none", new ClockImage());
        this.clockImage.sizeMode = "fixed-height";
        this.clockImage.mesh.renderOrder = 5;
        this.statsImage = new CanvasImageMesh(this, "Stats", "none", new StatsImage());
        this.statsImage.sizeMode = "fixed-height";
        this.statsImage.mesh.renderOrder = 5;
        this.infoLabel = new TextMesh(this, "InfoLabel", "none", {
            minHeight: 0.1,
            maxHeight: 0.1,
            padding: 0.02,
            scale: 1000,
            bgFillColor: options.labelFillColor,
            textFillColor: "white"
        });
        this.apps = new ApplicationLoader(this, options.getAppUrl);
        this.apps.addEventListener("apploaded", (evt) => {
            evt.app.addEventListener("joinroom", (evt) => {
                if (evt.roomName !== this._currentRoom) {
                    this._currentRoom = evt.roomName;
                    this.dispatchEvent(new EnvironmentRoomJoinedEvent(evt.roomName));
                }
            });
        });
        this.audio = new AudioManager(options.fetcher, DEFAULT_LOCAL_USER_ID);
        this.audioPlayer = new AudioPlayer(this.audio.context, this.audio.createSpatializer(false, false));
        this.videoPlayer = new VideoPlayer3D(this, this.audio.context, this.audio.createSpatializer(false, false));
        this.videoPlayer.content3d.visible = false;
        this.interactionAudio = new InteractionAudio(this.audio, this.eventSys);
        this.microphones = new LocalUserMicrophone(this.audio.context);
        this.webcams = LocalUserWebcam();
        this.devices = new DeviceManager(this.microphones, this.webcams);
        this.confirmationDialog = ConfirmationDialog(EnvironmentAttr(this), options.dialogFontFamily);
        this.devicesDialog = new DeviceDialog(this.fetcher, this.devices, this.audio, this.microphones, this.webcams, this.DEBUG);
        this.uiButtons = new ButtonFactory(options.uiImagePaths, 20, options.buttonFillColor, options.labelFillColor, this.DEBUG);
        this.menuButton = new ButtonImageWidget(this.uiButtons, "ui", "menu");
        this.settingsButton = new ButtonImageWidget(this.uiButtons, "ui", "settings");
        this.quitButton = new ButtonImageWidget(this.uiButtons, "ui", "quit");
        this.muteMicButton = new ToggleButton(this.uiButtons, "microphone", "mute", "unmute");
        this.muteCamButton = new ToggleButton(this.uiButtons, "media", "play", "pause");
        this.muteEnvAudioButton = new ToggleButton(this.uiButtons, "environment-audio", "mute", "unmute");
        this.muteEnvAudioButton.active = true;
        this.audio.ready.then(() => this.muteEnvAudioButton.active = false);
        this.fullscreenButton = new ScreenModeToggleButton(this.uiButtons, ScreenMode.Fullscreen);
        this.anaglyphButton = new ScreenModeToggleButton(this.uiButtons, ScreenMode.Anaglyph);
        this.vrButton = new ScreenModeToggleButton(this.uiButtons, ScreenMode.VR);
        this.arButton = new ScreenModeToggleButton(this.uiButtons, ScreenMode.AR);
        if (BatteryImage.isAvailable && isMobile()) {
            this.batteryImage = new CanvasImageMesh(this, "Battery", "none", new BatteryImage());
            this.batteryImage.sizeMode = "fixed-height";
        }
        this.xrUI = new SpaceUI();
        this.subMenu = new Widget(Div(ID("juniperSubMenu")), obj("sub-menu"), "flex");
        this.createMenu();
        this.screenControl.setUI(this.screenUISpace, this.anaglyphButton, this.fullscreenButton, this.vrButton, this.arButton);
        if (isDefined(this.screenControl)) {
            this.screenControl.addEventListener("sessionstarted", (evt) => {
                if (evt.mode === ScreenMode.Fullscreen && this.confirmationDialog.parentElement !== this.screenControl.fullscreenElement) {
                    this.screenControl.fullscreenElement.append(this.devicesDialog, this.confirmationDialog);
                }
            });
        }
        if (isDefined(options.watchModelPath)) {
            this.watch = new Watch(this, options.watchModelPath);
        }
        this.microphones.addEventListener("devicesettingschanged", () => {
            this.muteMicButton.active = this.microphones.enabled && !this.microphones.muted;
        });
        this.webcams.addEventListener("devicesettingschanged", () => {
            this.muteCamButton.active = this.webcams.enabled;
        });
        this.muteMicButton.content.addEventListener("click", () => {
            this.microphones.muted = this.microphones.enabled && !this.microphones.muted;
            this.muteMicButton.active = !this.microphones.muted;
        });
        this.muteCamButton.content.addEventListener("click", () => {
            this.webcams.enabled = !this.webcams.enabled;
            this.muteCamButton.active = this.webcams.enabled;
        });
        this.muteMicButton.active = this.microphones.enabled && !this.microphones.muted;
        this.muteCamButton.active = this.webcams.enabled;
        if (isFunction(options.createSpeechRecognizer)) {
            this.speech = options.createSpeechRecognizer(this.fetcher, this.devicesDialog.activity, this.microphones);
            this.speech.continuous = true;
        }
    }
    get canvas() {
        return this.statsImage.image.canvas;
    }
    createMenu() {
        if (isDefined(this.batteryImage)) {
            this.xrUI.addItem(this.batteryImage, { x: 0.75, y: -1, width: 0.2, height: 0.1 });
            this.screenUISpace.topRight.append(this.batteryImage.content);
        }
        this.xrUI.addItem(this.clockImage, { x: -1, y: 1, height: 0.1 });
        this.xrUI.addItem(this.statsImage, { x: -1, y: 0.95, height: 0.1 });
        this.xrUI.addItem(this.confirmationDialog, { x: 0, y: 0, scale: 0.25 });
        this.xrUI.addItem(this.menuButton, { x: -1, y: -1, scale: 0.5 });
        this.xrUI.addItem(this.infoLabel, { x: 0, y: -1.125, scale: 0.5 });
        this.xrUI.addItem(this.vrButton, { x: 1, y: -1, scale: 0.5 });
        this.xrUI.addItem(this.arButton, { x: 1, y: -1, scale: 0.5 });
        objGraph(this.menuButton, this.subMenu);
        objGraph(this.worldUISpace, this.xrUI);
        HtmlRender(this.screenUISpace.topLeft, pointerEvents("none"), HtmlRender(this.webcams, display("inline-block"), width(perc(50)), transform("scaleX(-1)")));
        this.screenUISpace.topRight.append(...[this.compassImage, this.statsImage]
            .map(v => v.canvas)
            .filter(isHTMLCanvas)
            .map(v => v));
        this.screenUISpace.bottomCenter.append(this.infoLabel.content);
        this.screenUISpace.bottomRight.append(this.vrButton.content, this.arButton.content, this.fullscreenButton.content, this.anaglyphButton.content);
        this.screenUISpace.bottomLeft.append(Div(this.menuButton.content, display("flex"), flexDirection("column-reverse"), gap(em(.25)), this.subMenu.content));
        widgetApply(this.subMenu, this.settingsButton, this.muteCamButton, this.muteMicButton, this.muteEnvAudioButton, this.quitButton);
        this.settingsButton.content.addEventListener("click", async () => {
            const mode = this.screenControl.currentMode;
            const wasPresenting = this.renderer.xr.isPresenting;
            if (wasPresenting) {
                await this.screenControl.stop();
            }
            await this.devicesDialog.showModal();
            if (wasPresenting) {
                await this.screenControl.start(mode);
            }
        });
        this.muteEnvAudioButton.content.addEventListener("click", () => {
            this.muteEnvAudioButton.active = !this.muteEnvAudioButton.active;
            this.dispatchEvent(this.envAudioToggleEvt);
        });
        this.quitButton.content.addEventListener("click", () => this.withConfirmation("Confirm quit", "Are you sure you want to quit?", async () => {
            if (this.renderer.xr.isPresenting) {
                this.screenControl.stop();
            }
            await this.onQuitting();
        }));
        this.menuButton.content.addEventListener("click", () => this.subMenu.visible = !this.subMenu.visible);
        [
            this.settingsButton,
            this.muteCamButton,
            this.muteMicButton,
            this.muteEnvAudioButton,
            this.quitButton
        ].forEach(btn => btn.content.addEventListener("click", () => this.subMenu.visible = false));
        this.subMenu.visible = false;
        this.vrButton.visible = isDesktop() && hasVR() || isMobileVR();
        this.arButton.visible = false;
        this.muteCamButton.visible = false;
        this.muteMicButton.visible = false;
    }
    layoutMenu() {
        let curCount = 0;
        for (const child of this.subMenu.content3d.children) {
            if (child.visible) {
                child.position.set(0, ++curCount * 0.25, 0);
            }
        }
    }
    preRender(evt) {
        super.preRender(evt);
        const { worldPos, worldQuat } = this.avatar;
        this.audio.setUserPose(this.audio.localUserID, worldPos.x, worldPos.y, worldPos.z, worldQuat.x, worldQuat.y, worldQuat.z, worldQuat.w);
        this.xrUI.visible = this.renderer.xr.isPresenting
            || this.testSpaceLayout;
        this.statsImage.isVisible = this.xrUI.visible
            && this.DEBUG;
        if (!this.renderer.xr.isPresenting) {
            this.compassImage.setPitchAndHeading(rad2deg(this.avatar.worldPitchRadians), rad2deg(this.avatar.worldHeadingRadians));
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
                this.statsImage.image.setStats(this.avgFPS, this.renderer.info.render.calls, this.renderer.info.render.triangles);
            }
        }
        this.confirmationDialog.update(evt.dt);
    }
    get environmentAudioMuted() {
        return this.muteEnvAudioButton.active;
    }
    async withConfirmation(title, msg, act) {
        this.onConfirmationShowing(true);
        if (await this.confirmationDialog.prompt(title, msg)) {
            act();
        }
        this.onConfirmationShowing(false);
    }
    onConfirmationShowing(showing) {
        widgetSetEnabled(this.quitButton, !showing);
        this.dispatchEvent(new DialogShowingEvent(showing));
    }
    async load(progOrAsset, ...assets) {
        let prog = null;
        if (isAsset(progOrAsset)) {
            assets.push(progOrAsset);
            prog = this.loadingBar;
        }
        else {
            prog = progOrAsset;
        }
        if (isHTMLCanvas(this.renderer.domElement)) {
            HtmlRender(this.renderer.domElement.parentElement, this.renderer.domElement, ...this.screenUISpace.elements);
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
        await super.load(prog, ...assets);
        await Promise.all([
            this.audio.createBasicClip("footsteps", footsteps, 0.5),
            this.interactionAudio.create("enter", enter, 0.25),
            this.interactionAudio.create("exit", exit, 0.25),
            this.interactionAudio.create("error", error, 0.25),
            this.interactionAudio.create("click", click, 1)
        ]);
        this.screenUISpace.show();
    }
}
//# sourceMappingURL=index.js.map