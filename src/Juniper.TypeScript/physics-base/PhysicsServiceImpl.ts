import { Body, Vec3, World } from "cannon-es";
import { ITimer, RequestAnimationFrameTimer } from "juniper-timers";
import { IPhysicsService } from "./IPhysicsService";

export class PhysicsServiceImpl implements IPhysicsService {
    private readonly world: World;
    private readonly timer: ITimer;

    private idCounter = 0;
    private readonly bodyToID = new Map<Body, number>();
    private readonly idToBody = new Map<number, Body>();

    constructor() {
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
        this.bodyToID.set(body, id);
        this.idToBody.set(id, body);
        this.world.addBody(body);
        return Promise.resolve(id);
    }
}
