import { AudioRecordingNode } from "@juniper-lib/audio/AudioRecordingNode";
import { JuniperSpeechRecognizer } from "./JuniperSpeechRecognizer";
import { WebSpeechRecognizer } from "./WebSpeechRecognizer";
export function createSpeechRecognizer(fetcher, activity, microphones, postPath, forceFallback = false) {
    if (WebSpeechRecognizer.isAvailable && !forceFallback) {
        return new WebSpeechRecognizer();
    }
    console.log("Using fallback speech recognizer");
    const recorder = new AudioRecordingNode(activity.context, activity);
    microphones.connect(recorder);
    return new JuniperSpeechRecognizer(fetcher, postPath, recorder);
}
//# sourceMappingURL=createSpeechRecognizer.js.map