import { BaseTimer } from "./BaseTimer";


export class AsyncCallbackTimer extends BaseTimer<() => Promise<void>> {

    #isRunning: boolean;
    get isRunning() { return this.#isRunning; }

    constructor() {
        super();

        let dt = 0;
        let t = 0;
        this.onTick = async () => {
            t = performance.now();
            if (this.lt >= 0) {
                dt = t - this.lt;
                this.tickEvt.set(t, dt);
                await Promise.all(this.tickHandlers.map(t => t(this.tickEvt)));
            }

            this.lt = t;

            if (this.#isRunning) {
                this.onTick();
            }
        };
    }

    start(): void {
        if (!this.#isRunning) {
            this.#isRunning = true;
            this.onTick();
        }
    }

    override stop() {
        this.#isRunning = false;
        super.stop();
    }
}
