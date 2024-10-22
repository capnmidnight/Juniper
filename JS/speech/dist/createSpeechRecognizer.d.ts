import { ActivityDetector, LocalUserMicrophone } from "@juniper-lib/audio";
import { IFetcher } from "@juniper-lib/fetcher";
import { ISpeechRecognizer } from "./ISpeechRecognizer";
export declare function createSpeechRecognizer(fetcher: IFetcher, activity: ActivityDetector, microphones: LocalUserMicrophone, postPath: string, forceFallback?: boolean): ISpeechRecognizer;
export type SpeechRecognizerFactory = (fetcher: IFetcher, activity: ActivityDetector, microphones: LocalUserMicrophone) => ISpeechRecognizer;
//# sourceMappingURL=createSpeechRecognizer.d.ts.map