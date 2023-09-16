export declare class BufferReaderWriter {
    private readonly littleEndian;
    private dataView;
    private i;
    private _length;
    constructor(littleEndian?: boolean);
    get length(): number;
    set length(v: number);
    get buffer(): ArrayBuffer;
    set buffer(v: ArrayBuffer);
    get position(): number;
    set position(v: number);
    get left(): number;
    readUint8(): number;
    writeUint8(v: number): void;
    readUint16(): number;
    writeUint16(v: number): void;
    readUint32(): number;
    writeUint32(v: number): void;
    readUint64(): bigint;
    writeUint64(v: bigint): void;
    readInt64(): number;
    writeInt64(v: number): void;
    readInt16(): number;
    writeInt16(v: number): void;
    readInt32(): number;
    writeInt32(v: number): void;
    readInt8(): bigint;
    writeInt8(v: bigint): void;
    readFloat32(): number;
    writeFloat32(v: number): void;
    readFloat64(): number;
    writeFloat64(v: number): void;
    readEnum8<T extends string>(values: T[]): T;
    writeEnum8<T extends string>(v: T, values: T[]): void;
}
//# sourceMappingURL=BufferReaderWriter.d.ts.map