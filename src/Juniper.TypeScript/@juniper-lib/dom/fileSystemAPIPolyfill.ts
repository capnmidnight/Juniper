import { Target } from "@juniper-lib/dom/attrs";
import { A, InputFile } from "@juniper-lib/dom/tags";
import { once } from "@juniper-lib/events/once";
import { identity } from "@juniper-lib/tslib/identity";

if (!("showOpenFilePicker" in globalThis)) {
    const fileInput = InputFile();

    const anchor = A(Target("_blank"));

    Object.assign(globalThis, {

        async showOpenFilePicker(options?: OpenFilePickerOptions) {
            fileInput.multiple = options && options.multiple;
            fileInput.accept = options && options.types
                .filter(v => v.accept)
                .flatMap(v => Object.keys(v.accept))
                .join(",");

            const task = once(fileInput, "input", "cancel")
                .catch(() => {
                    throw new DOMException("The user aborted a request.");
                });

            fileInput.click();

            await task;

            return Array.from(fileInput.files)
                .map(file => {
                    return {

                        getFile() {
                            return Promise.resolve(file);
                        }

                    };
                });
        },

        showSaveFilePicker(options?: SaveFilePickerOptions) {
            const blobParts = new Array<BlobPart>();
            return Promise.resolve({
                createWritable() {
                    return Promise.resolve({

                        write(blob: BlobPart) {
                            blobParts.push(blob);
                            return Promise.resolve();
                        },

                        async close() {
                            const types = options
                                && options.types
                                && options.types.filter(v => v.accept)
                                    .flatMap(v => Object.keys(v.accept))
                                    .filter(identity);
                            const type = types && types.length > 0 && types[0] || null;
                            const blob = new Blob(blobParts, { type });
                            const url = URL.createObjectURL(blob);
                            const task = once(anchor, "click");
                            anchor.download = options && options.suggestedName || null;
                            anchor.href = url;
                            anchor.click();
                            await task;
                        }

                    });
                }
            });
        }
    });
}
