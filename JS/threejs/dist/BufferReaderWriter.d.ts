import { BufferReaderWriter as BaseBufferReaderWriter } from "@juniper-lib/util";
import { Matrix4, Vector3 } from "three";
export declare class BufferReaderWriter extends BaseBufferReaderWriter {
    readVector48(vector: Vector3): void;
    writeVector48(vector: Vector3): void;
    readMatrix512(matrix: Matrix4): void;
    writeMatrix512(matrix: Matrix4): void;
}
//# sourceMappingURL=BufferReaderWriter.d.ts.map