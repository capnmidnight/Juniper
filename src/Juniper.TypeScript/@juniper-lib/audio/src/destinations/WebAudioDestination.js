import { AutoPlay, Controls, ID, SrcObject } from "@juniper-lib/dom/attrs";
import { display } from "@juniper-lib/dom/css";
import { onPlay } from "@juniper-lib/dom/evts";
import { onUserGesture } from "@juniper-lib/dom/onUserGesture";
import { Audio } from "@juniper-lib/dom/tags";
import { Task } from "@juniper-lib/events/Task";
import { BaseNodeCluster } from "../BaseNodeCluster";
import { Pose } from "../Pose";
import { JuniperGainNode } from "../context/JuniperGainNode";
import { JuniperMediaStreamAudioDestinationNode } from "../context/JuniperMediaStreamAudioDestinationNode";
import { WebAudioListenerNew } from "../listeners/WebAudioListenerNew";
import { WebAudioListenerOld } from "../listeners/WebAudioListenerOld";
import { WebAudioPannerNew } from "../spatializers/WebAudioPannerNew";
import { WebAudioPannerOld } from "../spatializers/WebAudioPannerOld";
import { hasNewAudioListener } from "../util";
export class WebAudioDestination extends BaseNodeCluster {
    get ready() { return this._ready; }
    get isReady() { return this._ready.finished && this._ready.resolved; }
    constructor(context) {
        const listener = hasNewAudioListener
            ? new WebAudioListenerNew(context)
            : new WebAudioListenerOld(context);
        const remoteUserInput = new JuniperGainNode(context);
        remoteUserInput.name = "remote-user-input";
        const spatializedInput = new JuniperGainNode(context);
        spatializedInput.name = "spatialized-input";
        const nonSpatializedInput = new JuniperGainNode(context);
        nonSpatializedInput.name = "non-spatialized-input";
        const destination = new JuniperMediaStreamAudioDestinationNode(context);
        const ready = new Task();
        const element = Audio(ID("Audio-Device-Manager"), display("none"), AutoPlay(true), Controls(true), SrcObject(destination.stream), onPlay(() => ready.resolve()));
        onUserGesture(() => element.play());
        super("web-audio-destination", context, [nonSpatializedInput, spatializedInput, remoteUserInput], [], [destination]);
        this.pose = new Pose();
        this._ready = ready;
        this.listener = listener;
        this.remoteUserInput = remoteUserInput;
        this.spatializedInput = spatializedInput;
        this.nonSpatializedInput = nonSpatializedInput;
        this.volumeControl = nonSpatializedInput;
        this.destination = destination;
        this.audioElement = element;
        remoteUserInput
            .connect(spatializedInput)
            .connect(this.volumeControl)
            .connect(destination);
    }
    createSpatializer(isRemoteStream) {
        const destination = isRemoteStream
            ? this.remoteUserInput
            : this.spatializedInput;
        const spatializer = hasNewAudioListener
            ? new WebAudioPannerNew(this.context)
            : new WebAudioPannerOld(this.context);
        spatializer.connect(destination);
        return spatializer;
    }
    setPosition(px, py, pz) {
        this.pose.setPosition(px, py, pz);
        this.listener.readPose(this.pose);
    }
    setOrientation(qx, qy, qz, qw) {
        this.pose.setOrientation(qx, qy, qz, qw);
        this.listener.readPose(this.pose);
    }
    set(px, py, pz, qx, qy, qz, qw) {
        this.pose.set(px, py, pz, qx, qy, qz, qw);
        this.listener.readPose(this.pose);
    }
    get stream() {
        return this.destination.stream;
    }
    get volume() {
        return this.volumeControl.gain.value;
    }
    set volume(v) {
        this.volumeControl.gain.value = v;
    }
}
//# sourceMappingURL=WebAudioDestination.js.map