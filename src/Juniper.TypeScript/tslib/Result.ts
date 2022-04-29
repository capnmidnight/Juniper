import { isDefined } from "./typeChecks";

export class Result<ValueT, ErrorT = unknown> {

    static success<ValueT, ErrorT = unknown>(value: ValueT): Result<ValueT, ErrorT> {
        return new Result(value);
    }

    static error<ValueT, ErrorT>(error: ErrorT): Result<ValueT, ErrorT> {
        return new Result(null, error);
    }

    constructor(private readonly _value: ValueT, public readonly error: ErrorT = null) {
    }

    get hasError(): boolean {
        return isDefined(this.error);
    }

    get hasValue(): boolean {
        return isDefined(this.value);
    }

    get value(): ValueT {
        if (this.hasError) {
            throw this.error;
        }

        return this._value;
    }

    then<NewValueT>(thunk: (value: ValueT) => NewValueT): Result<NewValueT, ErrorT> {
        if (this.hasError) {
            return new Result(null, this.error);
        }

        return new Result(thunk(this.value));
    }

    catch(thunk: () => ValueT): Result<ValueT, ErrorT > {
        if (this.hasError) {
            return new Result(thunk());
        }

        return this;
    }

    finally(thunk: () => void) {
        thunk();
        return this;
    }
}