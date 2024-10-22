export class Promisifier {
    #promise;
    constructor(resolveRejectTest, selectValue, selectRejectionReason) {
        this.callback = null;
        this.#promise = new Promise((resolve, reject) => {
            this.callback = (...args) => {
                if (resolveRejectTest(...args)) {
                    resolve(selectValue(...args));
                }
                else {
                    reject(selectRejectionReason(...args));
                }
            };
        });
    }
    get [Symbol.toStringTag]() {
        return this.#promise.toString();
    }
    then(onfulfilled, onrejected) {
        return this.#promise.then(onfulfilled, onrejected);
    }
    catch(onrejected) {
        return this.#promise.catch(onrejected);
    }
    finally(onfinally) {
        return this.#promise.finally(onfinally);
    }
}
//# sourceMappingURL=Promisifier.js.map