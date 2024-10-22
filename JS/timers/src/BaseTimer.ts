import { arrayRemove } from "@juniper-lib/util";
import { ITimer, TickHandler, TimerTickEvent } from "./ITimer";

export abstract class BaseTimer<CallbackT extends Function> implements ITimer {
    protected onTick: CallbackT;
    protected lt = -1;
    protected tickEvt = new TimerTickEvent();;
    protected tickHandlers = new Array<TickHandler>();

    addTickHandler(onTick: TickHandler): void {
        this.tickHandlers.push(onTick);
    }

    removeTickHandler(onTick: TickHandler): void {
        arrayRemove(this.tickHandlers, onTick);
    }

    restart() {
        this.stop();
        this.start();
    }

    abstract get isRunning(): boolean;

    abstract start(): void;

    stop() {
        this.lt = -1;
    }
}

