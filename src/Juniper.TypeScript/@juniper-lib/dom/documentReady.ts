export const documentReady = /*@__PURE__*/ (document.readyState === "complete"
    ? Promise.resolve("already")
    : new Promise((resolve) => {
        const onReadyStateChanged = () => {
            if (document.readyState === "complete") {
                document.removeEventListener("readystatechange", onReadyStateChanged);
                resolve("had to wait for it");
            }
        };
        document.addEventListener("readystatechange", onReadyStateChanged, false);
    }));