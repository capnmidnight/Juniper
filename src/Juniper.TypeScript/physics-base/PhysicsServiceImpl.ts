import { TypedEventBase } from "@juniper/events";
import { ITimer, RequestAnimationFrameTimer } from "@juniper/timers";
import { Body, Vec3, World } from "cannon-es";
import { IPhysicsService } from "./IPhysicsService";

export type PhysicsServiceEvents = void;

export class PhysicsServiceImpl
    extends TypedEventBase<PhysicsServiceEvents>
    implements IPhysicsService {
    private readonly world: World;
    private readonly timer: ITimer;

    private idCounter = 0;
    private readonly bodyToID = new Map<Body, number>();
    private readonly idToBody = new Map<number, Body>();

    constructor() {
        super();

        this.world = new World({
            allowSleep: true,
            gravity: new Vec3(0, -9.82, 0)
        });

        this.timer = new RequestAnimationFrameTimer();
        this.timer.addTickHandler((evt) => {
            this.world.fixedStep(evt.dt);
        });
        this.timer.start();
    }

    addBody(): Promise<number> {
        const id = ++this.idCounter;
        const body = new Body();
        body.addEventListener("sleep", () =>
            console.log(id, "sleep"));
        body.addEventListener("sleepy", () =>
            console.log(id, "sleepy"));
        body.addEventListener("wakeup", () =>
            console.log(id, "wakeup"));

        this.bodyToID.set(body, id);
        this.idToBody.set(id, body);
        this.world.addBody(body);
        return Promise.resolve(id);
    }

    removeBody(id: number): Promise<void> {
        const body = this.idToBody.get(id);
        this.world.removeBody(body);
        this.idToBody.delete(id);
        this.bodyToID.delete(body);
        return Promise.resolve();
    }
}
