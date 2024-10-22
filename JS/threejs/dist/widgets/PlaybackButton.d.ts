import { asyncCallback, IDisposable } from "@juniper-lib/util";
import { FullAudioRecord, IPlayer } from "@juniper-lib/audio";
import { TypedEvent } from "@juniper-lib/events";
import { BaseProgress } from "@juniper-lib/progress";
import { Object3D } from "three";
import { BaseEnvironment } from "../environment/BaseEnvironment";
import { ErsatzObject } from "../objects";
import { ButtonFactory } from "./ButtonFactory";
type PlaybackButtonEvents = {
    play: TypedEvent<"play">;
    stop: TypedEvent<"stop">;
};
export declare class PlaybackButton<T extends FullAudioRecord> extends BaseProgress<PlaybackButtonEvents> implements ErsatzObject, IDisposable {
    private readonly data;
    volume: number;
    private readonly player;
    readonly content3d: Object3D;
    private readonly textLabel;
    private readonly progressBar;
    private playButton;
    private pauseButton;
    private stopButton;
    private replayButton;
    readonly clickPlay: asyncCallback;
    constructor(env: BaseEnvironment, buttonFactory: ButtonFactory, data: T | string, name: string, label: string, volume: number, player: IPlayer<T>);
    private disposed;
    dispose(): void;
    private repositionLabel;
    private load;
    get label(): string;
    set label(v: string);
    report(soFar: number, total: number, msg?: string, est?: number): void;
}
export {};
//# sourceMappingURL=PlaybackButton.d.ts.map