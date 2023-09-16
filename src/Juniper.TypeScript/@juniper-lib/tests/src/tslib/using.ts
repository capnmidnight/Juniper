import { TestCase } from "@juniper-lib/testing/dist/tdd/TestCase";
import { dispose, IClosable, IDestroyable, IDisposable, using, usingArray, usingArrayAsync, usingAsync } from "@juniper-lib/tslib/dist/using";

type MockDisposable = IDisposable & { disposed: boolean };

function makeIDisposable(): MockDisposable {
    return {
        disposed: false,
        dispose() {
            this.disposed = true;
        }
    };
}

function makeIClosable(): IClosable & { closed: boolean } {
    return {
        closed: false,
        close() {
            this.closed = true;
        }
    };
}

function makeIDestroyable(): IDestroyable & { destroyed: boolean } {
    return {
        destroyed: false,
        destroy() {
            this.destroyed = true;
        }
    };
}

export class UsingTests extends TestCase {
    test_DisposeIDisposable() {
        const obj = makeIDisposable();
        this.isFalse(obj.disposed, "Before");
        dispose(obj);
        this.isTrue(obj.disposed, "After");
    }

    test_DisposeIClosable() {
        const obj = makeIClosable();
        this.isFalse(obj.closed, "Before");
        dispose(obj);
        this.isTrue(obj.closed, "After");
    }

    test_DisposeIDestroyable() {
        const obj = makeIDestroyable();
        this.isFalse(obj.destroyed, "Before");
        dispose(obj);
        this.isTrue(obj.destroyed, "After");
    }

    test_DisposeNull() {
        this.doesNotThrow(() =>
            dispose(null));
    }

    test_DisposeUndefined() {
        this.doesNotThrow(() =>
            dispose(undefined));
    }

    test_DisposeString() {
        this.doesNotThrow(() =>
            dispose("ASDFXYZ"));
    }

    test_DisposeNumber() {
        this.doesNotThrow(() =>
            dispose(3));
    }

    test_DisposeObject() {
        this.doesNotThrow(() =>
            dispose({}));
    }

    test_DisposeArray() {
        this.doesNotThrow(() =>
            dispose([123, 456]));
    }

    test_UsingIDisposable() {
        const obj = makeIDisposable();
        this.isFalse(obj.disposed, "Before");

        using(obj, (o) => {
            this.areExact(obj, o);
            this.isFalse(obj.disposed, "Inside");
        });

        this.isTrue(obj.disposed, "After");
    }

    test_UsingIClosable() {
        const obj = makeIClosable();

        this.isFalse(obj.closed, "Before");

        using(obj, (o) => {
            this.areExact(obj, o);
            this.isFalse(obj.closed, "Inside");
        });

        this.isTrue(obj.closed, "After");
    }

    test_UsingIDestroyable() {
        const obj = makeIDestroyable();

        this.isFalse(obj.destroyed, "Before");

        using(obj, (o) => {
            this.areExact(obj, o);
            this.isFalse(obj.destroyed, "Inside");
        });

        this.isTrue(obj.destroyed, "After");
    }

    test_UsingNull() {
        this.doesNotThrow(() =>
            using(null, this.isNull.bind(this)));
    }

    test_UsingUndefined() {
        this.doesNotThrow(() =>
            using(undefined, this.isUndefined.bind(this)));
    }


    async test_UsingAsyncIDisposable() {
        const obj = makeIDisposable();
        this.isFalse(obj.disposed, "Before");

        await usingAsync(obj, async (o) => {
            this.areExact(obj, o);
            this.isFalse(obj.disposed, "Inside");
        });

        this.isTrue(obj.disposed, "After");
    }

    async test_UsingAsyncIClosable() {
        const obj = makeIClosable();

        this.isFalse(obj.closed, "Before");

        await usingAsync(obj, async (o) => {
            this.areExact(obj, o);
            this.isFalse(obj.closed, "Inside");
        });

        this.isTrue(obj.closed, "After");
    }

    async test_UsingAsyncIDestroyable() {
        const obj = makeIDestroyable();

        this.isFalse(obj.destroyed, "Before");

        await usingAsync(obj, async (o) => {
            this.areExact(obj, o);
            this.isFalse(obj.destroyed, "Inside");
        });

        this.isTrue(obj.destroyed, "After");
    }

    test_UsingAsyncNull() {
        this.doesNotThrow(() =>
            usingAsync(null, async (obj) =>
                this.isNull(obj)));
    }

    test_UsingAsyncUndefined() {
        this.doesNotThrow(() =>
            usingAsync(undefined, async (obj) =>
                this.isUndefined(obj)));
    }



    test_UsingArrayIDisposable() {
        const arr = [
            makeIDisposable(),
            makeIDisposable(),
            makeIDisposable()
        ];

        arr.forEach((obj) =>
            this.isFalse(obj.disposed, "Before"));

        usingArray(arr, (a) => {
            this.areExact(arr, a);
            arr.forEach((obj) =>
                this.isFalse(obj.disposed, "Inside"));
        });

        arr.forEach((obj) =>
            this.isTrue(obj.disposed, "After"));
    }

    test_UsingArrayIClosable() {
        const arr = [
            makeIClosable(),
            makeIClosable(),
            makeIClosable()
        ];

        arr.forEach((obj) =>
            this.isFalse(obj.closed, "Before"));

        usingArray(arr, (a) => {
            this.areExact(arr, a);
            arr.forEach((obj) =>
                this.isFalse(obj.closed, "Inside"));
        });

        arr.forEach((obj) =>
            this.isTrue(obj.closed, "After"));
    }

    test_UsingArrayIDestroyable() {
        const arr = [
            makeIDestroyable(),
            makeIDestroyable(),
            makeIDestroyable()
        ];

        arr.forEach((obj) =>
            this.isFalse(obj.destroyed, "Before"));

        usingArray(arr, (a) => {
            this.areExact(arr, a);
            arr.forEach((obj) =>
                this.isFalse(obj.destroyed, "Inside"));
        });

        arr.forEach((obj) =>
            this.isTrue(obj.destroyed, "After"));
    }

    test_UsingArrayNull() {
        this.doesNotThrow(() =>
            usingArray(null, this.isNull.bind(this)));
    }

    test_UsingArrayOfNulls() {
        this.doesNotThrow(() =>
            usingArray([null, null, null], (arr) =>
                arr.forEach((obj) =>
                    this.isNull(obj))));
    }

    test_UsingArrayUndefined() {
        this.doesNotThrow(() =>
            usingArray(undefined, this.isUndefined.bind(this)));
    }

    test_UsingArrayOfUndefineds() {
        this.doesNotThrow(() =>
            usingArray([undefined, undefined, undefined], (arr) =>
                arr.forEach((obj) =>
                    this.isUndefined(obj))));
    }

    async test_UsingArrayAsyncIDisposable() {
        const arr = [
            makeIDisposable(),
            makeIDisposable(),
            makeIDisposable()
        ];

        arr.forEach((obj) =>
            this.isFalse(obj.disposed, "Before"));

        await usingArrayAsync(arr, async (a) => {
            this.areExact(arr, a);
            arr.forEach((obj) =>
                this.isFalse(obj.disposed, "Inside"));
        });

        arr.forEach((obj) =>
            this.isTrue(obj.disposed, "After"));
    }

    async test_UsingArrayAsyncIClosable() {
        const arr = [
            makeIClosable(),
            makeIClosable(),
            makeIClosable()
        ];

        arr.forEach((obj) =>
            this.isFalse(obj.closed, "Before"));

        await usingArrayAsync(arr, async (a) => {
            this.areExact(arr, a);
            arr.forEach((obj) =>
                this.isFalse(obj.closed, "Inside"));
        });

        arr.forEach((obj) =>
            this.isTrue(obj.closed, "After"));
    }

    async test_UsingArrayAsyncIDestroyable() {
        const arr = [
            makeIDestroyable(),
            makeIDestroyable(),
            makeIDestroyable()
        ];

        arr.forEach((obj) =>
            this.isFalse(obj.destroyed, "Before"));

        await usingArrayAsync(arr, async (a) => {
            this.areExact(arr, a);
            arr.forEach((obj) =>
                this.isFalse(obj.destroyed, "Inside"));
        });

        arr.forEach((obj) =>
            this.isTrue(obj.destroyed, "After"));
    }

    test_UsingArrayAsyncNull() {
        this.doesNotThrow(() =>
            usingArrayAsync(null, async (arr) =>
                this.isNull(arr)));
    }

    test_UsingArrayAsyncOfNulls() {
        this.doesNotThrow(() =>
            usingArrayAsync([null, null, null], async (arr) =>
                arr.forEach((obj) =>
                    this.isNull(obj))));
    }

    test_UsingArrayAsyncUndefined() {
        this.doesNotThrow(() =>
            usingArrayAsync(undefined, async (arr) =>
                this.isUndefined(arr)));
    }

    test_UsingArrayAsyncOfUndefineds() {
        this.doesNotThrow(() =>
            usingArrayAsync([undefined, undefined, undefined], async (arr) =>
                arr.forEach((obj) =>
                    this.isUndefined(obj))));
    }
}