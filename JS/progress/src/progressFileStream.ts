import { IProgress } from "./IProgress";

export function progressStreamFile(file: File, prog: IProgress): BodyInit {
    if (!prog || !("stream" in file)) {
        return file;
    }
    let bytesUploaded = 0;
    const transformer = new TransformStream({
        transform(chunk, controller) {
            controller.enqueue(chunk);
            bytesUploaded += chunk.byteLength;
            prog.report(bytesUploaded, file.size, file.name);
        },
        flush() {
            prog.end(file.name);
        },
    });
    prog.start(file.name);
    return file.stream().pipeThrough(transformer);
}