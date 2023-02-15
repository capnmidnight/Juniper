import { ActivityDetector } from "@juniper-lib/audio/ActivityDetector";
import { AudioRecordingNode } from "@juniper-lib/audio/AudioRecordingNode";
import { LocalUserMicrophone } from "@juniper-lib/audio/LocalUserMicrophone";
import { IFetcher } from "@juniper-lib/fetcher/IFetcher";
import { ISpeechRecognizer } from "@juniper-lib/speech/ISpeechRecognizer";
import { JuniperSpeechRecognizer } from "@juniper-lib/speech/JuniperSpeechRecognizer";
import { WebSpeechRecognizer } from "@juniper-lib/speech/WebSpeechRecognizer";

export function createSpeechRecognizer(fetcher: IFetcher, activity: ActivityDetector, microphones: LocalUserMicrophone, postPath: string): ISpeechRecognizer {
    if (WebSpeechRecognizer.isAvailable) {
        return new WebSpeechRecognizer();
    }

    console.log("Using fallback speech recognizer");
    const recorder = new AudioRecordingNode(activity.context, activity);
    microphones.connect(recorder);

    return new JuniperSpeechRecognizer(fetcher, postPath, recorder)
}

