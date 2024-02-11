import { sleep } from "./sleep";
/**
 * Performs an awaitable action, and if it fails, retries it up to `retryCount` times.
 *
 * If the action failed, the retry system will sleep for a geometrically growing amount of
 * time before retrying. This is to let potentially swamped resources have time to hopefully
 * recover before we hammer them again.
 *
 * The first wait time is 0.5s. Each subsequent wait time is twice the previous wait time.
 *
 * @param retryCount the number of times to re-attempt the action, after the first failed attempt.
 * @param action the action to perform.
 * @returns A promise that resolves to the same value as the action.
 **/
export function withRetry(retryCount, action) {
    return async () => {
        let lastError = null;
        let retryTime = 500;
        for (let retry = 0; retry <= retryCount; ++retry) {
            try {
                if (retry > 0) {
                    await sleep(retryTime);
                    retryTime *= 2;
                }
                return await action();
            }
            catch (error) {
                lastError = error;
            }
        }
        // If we got this far, it's because an error occured
        // in one of the retry attempts, so lastError should
        // not be null by now.
        throw lastError;
    };
}
//# sourceMappingURL=withRetry.js.map