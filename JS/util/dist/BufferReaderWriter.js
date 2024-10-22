export class BufferReaderWriter {
    #dataView;
    #i = 0;
    #length = -1;
    #littleEndian;
    constructor(littleEndian = false) {
        this.#littleEndian = littleEndian;
        this.length = 0;
    }
    get length() {
        return this.#length;
    }
    set length(v) {
        if (v !== this.length) {
            this.#length = v;
            this.#dataView = new DataView(new ArrayBuffer(this.length));
            this.#i = 0;
        }
    }
    get buffer() {
        return this.#dataView.buffer;
    }
    set buffer(v) {
        if (v !== this.buffer) {
            this.#length = v.byteLength;
            this.#dataView = new DataView(v);
            this.#i = 0;
        }
    }
    get position() {
        return this.#i;
    }
    set position(v) {
        this.#i = v;
    }
    get left() {
        return this.length - this.#i;
    }
    readUint8() {
        const v = this.#dataView.getUint8(this.#i);
        this.#i += 1;
        return v;
    }
    writeUint8(v) {
        this.#dataView.setUint8(this.#i, v);
        this.#i += 1;
    }
    readUint16() {
        const v = this.#dataView.getUint16(this.#i, this.#littleEndian);
        this.#i += 2;
        return v;
    }
    writeUint16(v) {
        this.#dataView.setUint16(this.#i, v, this.#littleEndian);
        this.#i += 2;
    }
    readUint32() {
        const v = this.#dataView.getUint32(this.#i, this.#littleEndian);
        this.#i += 4;
        return v;
    }
    writeUint32(v) {
        this.#dataView.setUint32(this.#i, v, this.#littleEndian);
        this.#i += 4;
    }
    readUint64() {
        const v = this.#dataView.getBigUint64(this.#i, this.#littleEndian);
        this.#i += 8;
        return v;
    }
    writeUint64(v) {
        this.#dataView.setBigUint64(this.#i, v, this.#littleEndian);
        this.#i += 8;
    }
    readInt64() {
        const v = this.#dataView.getInt8(this.#i);
        this.#i += 1;
        return v;
    }
    writeInt64(v) {
        this.#dataView.setInt8(this.#i, v);
        this.#i += 1;
    }
    readInt16() {
        const v = this.#dataView.getInt16(this.#i, this.#littleEndian);
        this.#i += 2;
        return v;
    }
    writeInt16(v) {
        this.#dataView.setInt16(this.#i, v, this.#littleEndian);
        this.#i += 2;
    }
    readInt32() {
        const v = this.#dataView.getInt32(this.#i, this.#littleEndian);
        this.#i += 4;
        return v;
    }
    writeInt32(v) {
        this.#dataView.setInt32(this.#i, v, this.#littleEndian);
        this.#i += 4;
    }
    readInt8() {
        const v = this.#dataView.getBigInt64(this.#i, this.#littleEndian);
        this.#i += 8;
        return v;
    }
    writeInt8(v) {
        this.#dataView.setBigInt64(this.#i, v, this.#littleEndian);
        this.#i += 8;
    }
    readFloat32() {
        const v = this.#dataView.getFloat32(this.#i, this.#littleEndian);
        this.#i += 4;
        return v;
    }
    writeFloat32(v) {
        this.#dataView.setFloat32(this.#i, v, this.#littleEndian);
        this.#i += 4;
    }
    readFloat64() {
        const v = this.#dataView.getFloat64(this.#i, this.#littleEndian);
        this.#i += 8;
        return v;
    }
    writeFloat64(v) {
        this.#dataView.setFloat64(this.#i, v, this.#littleEndian);
        this.#i += 8;
    }
    readEnum8(values) {
        const idx = this.readUint8();
        return values[idx];
    }
    writeEnum8(v, values) {
        const idx = values.indexOf(v);
        this.writeUint8(idx);
    }
}
//# sourceMappingURL=BufferReaderWriter.js.map