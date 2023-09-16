import { Object3D } from "three";
import { AvatarRemote } from "../../AvatarRemote";
import { BufferReaderWriter } from "../../BufferReaderWriter";
import type { BaseEnvironment } from "../../environment/BaseEnvironment";
import { IXRHandModel } from "../../examples/webxr/XRHandModelFactory";
import { ErsatzObject } from "../../objects";
import { PointerID, PointerType } from "../Pointers";
import { BasePointer } from "./BasePointer";
export declare class PointerRemote extends BasePointer implements ErsatzObject {
    private readonly avatar;
    private readonly remoteID;
    readonly object: Object3D;
    private readonly laser;
    private readonly D;
    private readonly P;
    private readonly visualOffset;
    private readonly M;
    private readonly MW;
    private readonly originTarget;
    private readonly directionTarget;
    private readonly upTarget;
    private readonly visualOffsetTarget;
    private readonly handCube;
    readonly remoteType: PointerType;
    private _hand;
    constructor(avatar: AvatarRemote, env: BaseEnvironment, remoteID: PointerID);
    get hand(): IXRHandModel;
    set hand(v: IXRHandModel);
    readState(buffer: BufferReaderWriter): void;
    private killTimeout;
    deferExecution(killTime: number, killAction: () => void): void;
    writeState(_buffer: BufferReaderWriter): void;
    protected onUpdate(): void;
    animate(dt: number): void;
    updatePointerOrientation(): void;
    vibrate(): void;
}
//# sourceMappingURL=PointerRemote.d.ts.map