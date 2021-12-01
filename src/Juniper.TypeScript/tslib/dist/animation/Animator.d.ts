export declare class Animator {
    private animations;
    update(dt: number): void;
    clear(): void;
    start(delay: number, duration: number, update: (t: number) => void): Promise<void>;
}
