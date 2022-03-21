import { isDefined } from "./typeChecks";

export class Task<T = void> implements Promise<T> {

    private readonly promise: Promise<T>;

    resolve: (value: T) => void = null;
    reject: (reason: any) => void = null;

    constructor(resolveTest?: (value: T) => boolean, rejectTest?: (reason: any) => boolean) {
        if (isDefined(resolveTest) && isDefined(rejectTest)) {
            this.promise = new Promise((resolve, reject) => {
                this.resolve = (value: T) => {
                    if (resolveTest(value)) {
                        resolve(value);
                    }
                };

                this.reject = (reason: any) => {
                    if (rejectTest(reason)) {
                        reject(reason);
                    }
                };
            });
        }
        else if (isDefined(resolveTest)) {
            this.promise = new Promise((resolve, reject) => {
                this.resolve = (value: T) => {
                    if (resolveTest(value)) {
                        resolve(value);
                    }
                };

                this.reject = reject;
            });
        }
        else if (isDefined(rejectTest)) {
            this.promise = new Promise((resolve, reject) => {
                this.resolve = resolve;
                this.reject = (reason: any) => {
                    if (rejectTest(reason)) {
                        reject(reason);
                    }
                };
            });
        }
        else {
            this.promise = new Promise((resolve, reject) => {
                this.resolve = resolve;
                this.reject = reject;
            });
        }

    }

    get [Symbol.toStringTag](): string {
        return this.promise.toString();
    }

    then<TResult1 = T, TResult2 = never>(onfulfilled?: (value: T) => TResult1 | PromiseLike<TResult1>, onrejected?: (reason: any) => TResult2 | PromiseLike<TResult2>): Promise<TResult1 | TResult2> {
        return this.promise.then(onfulfilled, onrejected);
    }

    catch<TResult = never>(onrejected?: (reason: any) => TResult | PromiseLike<TResult>): Promise<T | TResult> {
        return this.promise.catch(onrejected);
    }

    finally(onfinally?: () => void): Promise<T> {
        return this.promise.finally(onfinally);
    }
}

export class Promisifier<T = void> implements Promise<T> {

    private readonly promise: Promise<T>;

    callback: (...args: any[]) => void = null;

    constructor(
        resolveRejectTest: (...args: any[]) => boolean,
        selectValue: (...args: any[]) => T,
        selectRejectionReason: (...args: any[]) => any) {
        this.promise = new Promise((resolve, reject) => {
            this.callback = (...args: any[]) => {
                if (resolveRejectTest(...args)) {
                    resolve(selectValue(...args));
                }
                else {
                    reject(selectRejectionReason(...args));
                }
            };
        });

    }

    get [Symbol.toStringTag](): string {
        return this.promise.toString();
    }

    then<TResult1 = T, TResult2 = never>(onfulfilled?: (value: T) => TResult1 | PromiseLike<TResult1>, onrejected?: (reason: any) => TResult2 | PromiseLike<TResult2>): Promise<TResult1 | TResult2> {
        return this.promise.then(onfulfilled, onrejected);
    }

    catch<TResult = never>(onrejected?: (reason: any) => TResult | PromiseLike<TResult>): Promise<T | TResult> {
        return this.promise.catch(onrejected);
    }

    finally(onfinally?: () => void): Promise<T> {
        return this.promise.finally(onfinally);
    }
}