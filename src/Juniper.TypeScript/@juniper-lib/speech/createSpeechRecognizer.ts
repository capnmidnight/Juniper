import { ActivityDetector } from "@juniper-lib/audio/ActivityDetector";
import { AudioRecordingNode } from "@juniper-lib/audio/AudioRecordingNode";
import { LocalUserMicrophone } from "@juniper-lib/audio/LocalUserMicrophone";
import { IFetcher } from "@juniper-lib/fetcher/IFetcher";
import { ISpeechRecognizer } from "./ISpeechRecognizer";
import { JuniperSpeechRecognizer } from "./JuniperSpeechRecognizer";
import { WebSpeechRecognizer } from "./WebSpeechRecognizer";

export function createSpeechRecognizer(fetcher: IFetcher, activity: ActivityDetector, microphones: LocalUserMicrophone, postPath: string, forceFallback = false): ISpeechRecognizer {
    if (WebSpeechRecognizer.isAvailable && !forceFallback) {
        return new WebSpeechRecognizer();
    }

    console.log("Using fallback speech recognizer");
    const recorder = new AudioRecordingNode(activity.context, activity);
    microphones.connect(recorder);

    return new JuniperSpeechRecognizer(fetcher, postPath, recorder);
}

export type SpeechRecognizerFactory = (fetcher: IFetcher, activity: ActivityDetector, microphones: LocalUserMicrophone) => ISpeechRecognizer;