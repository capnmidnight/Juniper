import { BufferReaderWriter as BaseBufferReaderWriter } from "@juniper-lib/util";
import { Matrix4, Vector3 } from "three";

export class BufferReaderWriter extends BaseBufferReaderWriter {

    readVector48(vector: Vector3) {
        vector.set(
            this.readFloat32(),
            this.readFloat32(),
            this.readFloat32()
        );
    }

    writeVector48(vector: Vector3) {
        this.writeFloat32(vector.x);
        this.writeFloat32(vector.y);
        this.writeFloat32(vector.z);
    }


    readMatrix512(matrix: Matrix4) {
        matrix.elements[0] = this.readFloat32();
        matrix.elements[1] = this.readFloat32();
        matrix.elements[2] = this.readFloat32();
        matrix.elements[3] = this.readFloat32();
        matrix.elements[4] = this.readFloat32();
        matrix.elements[5] = this.readFloat32();
        matrix.elements[6] = this.readFloat32();
        matrix.elements[7] = this.readFloat32();
        matrix.elements[8] = this.readFloat32();
        matrix.elements[9] = this.readFloat32();
        matrix.elements[10] = this.readFloat32();
        matrix.elements[11] = this.readFloat32();
        matrix.elements[12] = this.readFloat32();
        matrix.elements[13] = this.readFloat32();
        matrix.elements[14] = this.readFloat32();
        matrix.elements[15] = this.readFloat32();
    }

    writeMatrix512(matrix: Matrix4) {
        this.writeFloat32(matrix.elements[0]);
        this.writeFloat32(matrix.elements[1]);
        this.writeFloat32(matrix.elements[2]);
        this.writeFloat32(matrix.elements[3]);
        this.writeFloat32(matrix.elements[4]);
        this.writeFloat32(matrix.elements[5]);
        this.writeFloat32(matrix.elements[6]);
        this.writeFloat32(matrix.elements[7]);
        this.writeFloat32(matrix.elements[8]);
        this.writeFloat32(matrix.elements[9]);
        this.writeFloat32(matrix.elements[10]);
        this.writeFloat32(matrix.elements[11]);
        this.writeFloat32(matrix.elements[12]);
        this.writeFloat32(matrix.elements[13]);
        this.writeFloat32(matrix.elements[14]);
        this.writeFloat32(matrix.elements[15]);
    }
}
