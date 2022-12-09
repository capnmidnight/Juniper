import { TypedEvent } from "@juniper-lib/tslib/events/EventBase";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { JuniperAudioContext } from "./context/JuniperAudioContext";
import { JuniperAudioNode } from "./context/JuniperAudioNode";
import { JuniperBiquadFilterNode } from "./context/JuniperBiquadFilterNode";
import { JuniperDynamicsCompressorNode } from "./context/JuniperDynamicsCompressorNode";
import { JuniperGainNode } from "./context/JuniperGainNode";
import { JuniperMediaStreamAudioDestinationNode } from "./context/JuniperMediaStreamAudioDestinationNode";
import { JuniperMediaStreamAudioSourceNode } from "./context/JuniperMediaStreamAudioSourceNode";


export class LocalUserMicrophone extends JuniperAudioNode<{
    started: TypedEvent<"started">;
    stopped: TypedEvent<"stopped">;
}> {

    private localStream: JuniperMediaStreamAudioSourceNode;
    private readonly localVolume: JuniperGainNode;
    private readonly localAutoControlledGain: JuniperGainNode;
    private readonly localOutput: JuniperMediaStreamAudioDestinationNode;

    constructor(context: JuniperAudioContext) {
        const localVolume = context.createGain();
        localVolume.name = "local-mic-user-gain";

        const localAutoControlledGain = new JuniperGainNode(context);
        localAutoControlledGain.name = "local-mic-auto-gain";

        const localFilter = new JuniperBiquadFilterNode(context, {
            type: "bandpass",
            frequency: 1500,
            Q: 0.25,
        });
        localFilter.name = "local-mic-filter";

        const localCompressor = new JuniperDynamicsCompressorNode(context, {
            threshold: -15,
            knee: 40,
            ratio: 17
        });
        localCompressor.name = "local-mic-compressor";

        const localOutput = new JuniperMediaStreamAudioDestinationNode(context);
        localOutput.name = "local-mic-destination";

        super("local-user-microphone", context, [], [localCompressor], [
            localVolume,
            localAutoControlledGain,
            localFilter,
            localOutput
        ]);

        this.localVolume = localVolume;
        this.localOutput = localOutput;
        this.localAutoControlledGain = localAutoControlledGain;

        localVolume
            .connect(localAutoControlledGain)
            .connect(localFilter)
            .connect(localCompressor)
            .connect(localOutput);
    }

    get gain() {
        return this.localVolume.gain;
    }

    get inStream() {
        if (isDefined(this.localStream)) {
            return this.localStream.mediaStream;
        }
        else {
            return null;
        }
    }

    set inStream(mediaStream) {
        if (mediaStream !== this.inStream) {
            if (isDefined(this.localStream)) {
                this.remove(this.localStream);
                this.localStream.dispose();
                this.localStream = null;
            }

            if (isDefined(mediaStream)) {
                this.localStream = new JuniperMediaStreamAudioSourceNode(this.context, {
                    mediaStream
                });
                this.add(this.localStream);
                this.localStream.connect(this.localVolume);
            }
        }
    }

    get outStream() {
        return this.localOutput.stream;
    }
}