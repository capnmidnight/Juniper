export class BufferReaderWriter {
    private dataView: DataView;
    private i = 0;
    private _length = -1;

    constructor(private readonly littleEndian = false) {
        this.length = 0;
    }

    get length() {
        return this._length;
    }

    set length(v) {
        if (v !== this.length) {
            this._length = v;
            this.dataView = new DataView(new ArrayBuffer(this.length));
            this.i = 0;
        }
    }

    get buffer(): ArrayBuffer {
        return this.dataView.buffer;
    }

    set buffer(v: ArrayBuffer) {
        if (v !== this.buffer) {
            this._length = v.byteLength;
            this.dataView = new DataView(v);
            this.i = 0;
        }
    }

    get position(): number {
        return this.i;
    }

    set position(v: number) {
        this.i = v;
    }

    get left() {
        return this.length - this.i;
    }


    readUint8(): number {
        const v = this.dataView.getUint8(this.i);
        this.i += 1;
        return v;
    }

    writeUint8(v: number): void {
        this.dataView.setUint8(this.i, v);
        this.i += 1;
    }


    readUint16(): number {
        const v = this.dataView.getUint16(this.i, this.littleEndian);
        this.i += 2;
        return v;
    }

    writeUint16(v: number): void {
        this.dataView.setUint16(this.i, v, this.littleEndian);
        this.i += 2;
    }


    readUint32(): number {
        const v = this.dataView.getUint32(this.i, this.littleEndian);
        this.i += 4;
        return v;
    }

    writeUint32(v: number): void {
        this.dataView.setUint32(this.i, v, this.littleEndian);
        this.i += 4;
    }


    readUint64(): bigint {
        const v = this.dataView.getBigUint64(this.i, this.littleEndian);
        this.i += 8;
        return v;
    }

    writeUint64(v: bigint): void {
        this.dataView.setBigUint64(this.i, v, this.littleEndian);
        this.i += 8;
    }


    readInt64(): number {
        const v = this.dataView.getInt8(this.i);
        this.i += 1;
        return v;
    }

    writeInt64(v: number): void {
        this.dataView.setInt8(this.i, v);
        this.i += 1;
    }


    readInt16(): number {
        const v = this.dataView.getInt16(this.i, this.littleEndian);
        this.i += 2;
        return v;
    }

    writeInt16(v: number): void {
        this.dataView.setInt16(this.i, v, this.littleEndian);
        this.i += 2;
    }


    readInt32(): number {
        const v = this.dataView.getInt32(this.i, this.littleEndian);
        this.i += 4;
        return v;
    }

    writeInt32(v: number): void {
        this.dataView.setInt32(this.i, v, this.littleEndian);
        this.i += 4;
    }


    readInt8(): bigint {
        const v = this.dataView.getBigInt64(this.i, this.littleEndian);
        this.i += 8;
        return v;
    }

    writeInt8(v: bigint): void {
        this.dataView.setBigInt64(this.i, v, this.littleEndian);
        this.i += 8;
    }


    readFloat32(): number {
        const v = this.dataView.getFloat32(this.i, this.littleEndian);
        this.i += 4;
        return v;
    }

    writeFloat32(v: number): void {
        this.dataView.setFloat32(this.i, v, this.littleEndian);
        this.i += 4;
    }


    readFloat64(): number {
        const v = this.dataView.getFloat64(this.i, this.littleEndian);
        this.i += 8;
        return v;
    }

    writeFloat64(v: number): void {
        this.dataView.setFloat64(this.i, v, this.littleEndian);
        this.i += 8;
    }


    readEnum8<T extends string>(values: T[]): T {
        const idx = this.readUint8();
        return values[idx];
    }

    writeEnum8<T extends string>(v: T, values: T[]): void {
        const idx = values.indexOf(v);
        this.writeUint8(idx);
    }
}
