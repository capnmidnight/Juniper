import { FullAudioRecord } from "@juniper-lib/audio/data";
import { IPlayer } from "@juniper-lib/audio/sources/IPlayer";
import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
import { BaseProgress } from "@juniper-lib/progress/BaseProgress";
import { AsyncCallback } from "@juniper-lib/tslib/identity";
import { IDisposable } from "@juniper-lib/tslib/using";
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
    readonly object: Object3D;
    private readonly textLabel;
    private readonly progressBar;
    private playButton;
    private pauseButton;
    private stopButton;
    private replayButton;
    readonly clickPlay: AsyncCallback;
    constructor(env: BaseEnvironment, buttonFactory: ButtonFactory, data: T | string, name: string, label: string, volume: number, player: IPlayer);
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