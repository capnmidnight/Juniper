import { isWorker } from "@juniper-lib/util";

export const documentReady = /*@__PURE__*/ (function () {
    if (isWorker) {
        return Promise.resolve("worker, no document");
    }
    else if (document.readyState === "complete") {
        return Promise.resolve("document started ready");
    }
    else {
        return new Promise((resolve) => {
            const onReadyStateChanged = () => {
                if (document.readyState === "complete") {
                    document.removeEventListener("readystatechange", onReadyStateChanged);
                    resolve("had to wait for it");
                }
            };
            document.addEventListener("readystatechange", onReadyStateChanged, false);
        });
    }
})();