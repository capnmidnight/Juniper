import { ActivityDetector } from "@juniper-lib/audio/ActivityDetector";
import { LocalUserMicrophone } from "@juniper-lib/audio/LocalUserMicrophone";
import { IFetcher } from "@juniper-lib/fetcher/IFetcher";
import { ISpeechRecognizer } from "./ISpeechRecognizer";
export declare function createSpeechRecognizer(fetcher: IFetcher, activity: ActivityDetector, microphones: LocalUserMicrophone, postPath: string, forceFallback?: boolean): ISpeechRecognizer;
export type SpeechRecognizerFactory = (fetcher: IFetcher, activity: ActivityDetector, microphones: LocalUserMicrophone) => ISpeechRecognizer;
//# sourceMappingURL=createSpeechRecognizer.d.ts.map