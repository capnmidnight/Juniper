export interface IPhysicsService {
    addBody(): Promise<number>;
    removeBody(id: number): Promise<void>;
}