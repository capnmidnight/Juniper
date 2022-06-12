import { IFetcher } from "@juniper-lib/fetcher-base/IFetcher";
import { IProgress, isDefined } from "@juniper-lib/tslib";

export class Asset<PathT extends string | URL, ResultT, ErrorT = unknown> implements Promise<ResultT> {

    private readonly promise: Promise<ResultT>;

    private _result: ResultT = null;
    private _error: ErrorT = null;
    private _started = false;
    private _finished = false;

    get result(): ResultT {
        if (isDefined(this.error)) {
            throw this.error;
        }

        return this._result;
    }

    get error(): ErrorT {
        return this._error;
    }

    get started(): boolean {
        return this._started;
    }

    get finished(): boolean {
        return this._finished;
    }

    private resolve: (value: ResultT) => void = null;
    private reject: (reason: ErrorT) => void = null;

    constructor(private readonly path: PathT, private readonly getter: (path: PathT, prog?: IProgress) => Promise<ResultT>) {
        this.promise = new Promise((resolve, reject) => {
            this.resolve = (value: ResultT) => {
                this._result = value;
                this._finished = true;
                resolve(value);
            };

            this.reject = (reason: ErrorT) => {
                this._error = reason;
                this._finished = true;
                reject(reason);
            };
        });
    }

    getSize(fetcher: IFetcher): Promise<[this, number]> {
        return fetcher
            .head(this.path)
            .exec()
            .then(response => [this, response.contentLength]);
    }

    async getContent(prog?: IProgress): Promise<void> {
        try {
            const response = await this.getter(this.path, prog);
            this.resolve(response);
        }
        catch (err) {
            this.reject(err);
        }
    }

    get [Symbol.toStringTag](): string {
        return this.promise.toString();
    }

    then<TResult1 = ResultT, TResult2 = never>(onfulfilled?: (value: ResultT) => TResult1 | PromiseLike<TResult1>, onrejected?: (reason: any) => TResult2 | PromiseLike<TResult2>): Promise<TResult1 | TResult2> {
        return this.promise.then(onfulfilled, onrejected);
    }

    catch<TResult = never>(onrejected?: (reason: any) => TResult | PromiseLike<TResult>): Promise<ResultT | TResult> {
        return this.promise.catch(onrejected);
    }

    finally(onfinally?: () => void): Promise<ResultT> {
        return this.promise.finally(onfinally);
    }
}