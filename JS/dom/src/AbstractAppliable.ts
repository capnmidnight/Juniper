export abstract class AbstractAppliable<TargetT extends EventTarget = EventTarget> {
    abstract apply(tag: TargetT): void;
}
