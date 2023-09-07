import { keycapDigits } from "@juniper-lib/emoji/numbers";
import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
import { all } from "@juniper-lib/events/all";
import { BaseProgress } from "@juniper-lib/progress/BaseProgress";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { Cube } from "../Cube";
import { cleanup } from "../cleanup";
import { solidWhite } from "../materials";
import { obj, objGraph } from "../objects";
import { TextMesh } from "./TextMesh";
const playEvt = new TypedEvent("play");
const stopEvt = new TypedEvent("stop");
const size = 0.1;
const translations = new Map(keycapDigits.alts.map((m, i) => [m.value, i.toString()]));
export class PlaybackButton extends BaseProgress {
    constructor(env, buttonFactory, data, name, label, volume, player) {
        super();
        this.data = data;
        this.volume = volume;
        this.player = player;
        this.playButton = null;
        this.pauseButton = null;
        this.stopButton = null;
        this.replayButton = null;
        this.clickPlay = null;
        this.disposed = false;
        label = translations.get(label) || label || "";
        this.object = obj(`playback-${name}`);
        this.textLabel = new TextMesh(env, `playback-${name}-label`, "none", {
            minHeight: size,
            minWidth: 4 * size,
            maxWidth: 4 * size,
            padding: 0.02,
            scale: 1000,
            bgFillColor: buttonFactory.labelFillColor,
            textFillColor: "white"
        });
        this.textLabel.image.value = label;
        this.textLabel.image.addEventListener("redrawn", () => this.repositionLabel());
        this.progressBar = new Cube(4 * size, 0.025, 0.01, solidWhite);
        this.progressBar.visible = false;
        this.clickPlay = async () => {
            if (this.player.data !== this.data) {
                await this.player.load(this.data, this);
            }
            this.player.volume = this.volume;
            await this.player.play();
        };
        this.load(buttonFactory, player);
    }
    dispose() {
        if (!this.disposed) {
            if (this.data === this.player.data) {
                this.player.clear();
            }
            cleanup(this.object);
            this.disposed = true;
        }
    }
    repositionLabel() {
        this.textLabel.position.y = -(size + this.textLabel.objectHeight) / 2;
    }
    async load(buttonFactory, player) {
        const [play, pause, stop, replay] = await all(buttonFactory.getMeshButton("media", "play", size), buttonFactory.getMeshButton("media", "pause", size), buttonFactory.getMeshButton("media", "stop", size), buttonFactory.getMeshButton("media", "replay", size));
        objGraph(this, this.playButton = play, this.pauseButton = pause, this.stopButton = stop, this.replayButton = replay, this.progressBar, this.textLabel);
        this.playButton.object.position.x = -1.5 * size;
        this.pauseButton.object.position.x = -0.5 * size;
        this.stopButton.object.position.x = 0.5 * size;
        this.replayButton.object.position.x = 1.5 * size;
        this.progressBar.position.y = -size / 2;
        this.progressBar.position.z = 0.01;
        this.repositionLabel();
        const refresh = () => {
            const hasMyData = player.data === this.data;
            this.playButton.disabled = hasMyData
                && (player.playbackState === "loading"
                    || player.playbackState === "playing"
                    || player.playbackState === "errored")
                || !hasMyData
                    && player.playbackState === "loading";
            this.pauseButton.disabled = !hasMyData
                || player.playbackState === "loading"
                || player.playbackState !== "playing";
            this.replayButton.disabled
                = this.stopButton.disabled
                    = !hasMyData
                        || player.playbackState === "loading"
                        || player.playbackState === "stopped"
                        || player.playbackState === "errored";
            if (!hasMyData
                || player.playbackState === "loading"
                || player.playbackState === "stopped") {
                this.progressBar.visible = false;
            }
        };
        refresh();
        const local = (callback) => (evt) => {
            if (evt.source.data === this.data) {
                callback(evt);
            }
        };
        const localRefresh = local(refresh);
        player.addEventListener("loading", refresh);
        player.addEventListener("loaded", localRefresh);
        player.addEventListener("played", localRefresh);
        player.addEventListener("paused", localRefresh);
        player.addEventListener("stopped", refresh);
        player.addEventListener("progress", local((evt) => this.report(evt.value, evt.total)));
        player.addEventListener("played", local(() => this.dispatchEvent(playEvt)));
        player.addEventListener("stopped", local(() => this.dispatchEvent(stopEvt)));
        const onClick = (btn, callback) => {
            btn.addEventListener("click", callback);
        };
        onClick(this.playButton, this.clickPlay);
        onClick(this.pauseButton, () => player.pause());
        onClick(this.stopButton, () => player.stop());
        onClick(this.replayButton, () => player.restart());
    }
    get label() {
        if (isDefined(this.textLabel.image)) {
            return this.textLabel.image.value;
        }
        return null;
    }
    set label(v) {
        v = translations.get(v) || v;
        this.textLabel.image.value = v;
    }
    report(soFar, total, msg, est) {
        super.report(soFar, total, msg, est);
        const width = this.p * 4 * size;
        this.progressBar.position.x = 0.5 * (width - 4 * size);
        this.progressBar.scale.x = width;
        this.progressBar.visible = soFar > 0;
    }
}
//# sourceMappingURL=PlaybackButton.js.map