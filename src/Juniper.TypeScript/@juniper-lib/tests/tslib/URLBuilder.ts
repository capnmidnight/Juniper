import { TestCase } from "@juniper-lib/testing/tdd";
import { URLBuilder } from "@juniper-lib/tslib";

export class URLBuilderTests extends TestCase {

    test_EmptyConstructor() {
        this.doesNotThrow(() => {
            const b = new URLBuilder();
            this.throws(() =>
                b.toString());
        });
    }

    test_EmptyConstructor_SetProtocol() {
        const b = new URLBuilder();
        this.doesNotThrow(() => {
            b.protocol("http:");
            this.throws(() =>
                b.toString());
        });
    }

    test_EmptyConstructor_SetHost() {
        const b = new URLBuilder();
        this.doesNotThrow(() => {
            b.host("www.seanmcbeth.com:8081");
            this.throws(() =>
                b.toString());
        });
    }

    test_EmptyConstructor_SetHostName() {
        const b = new URLBuilder();
        this.doesNotThrow(() => {
            b.hostName("www.seanmcbeth.com");
            this.throws(() =>
                b.toString());
        });
    }

    test_EmptyConstructor_SetProtocolAndHost() {
        const b = new URLBuilder();
        this.areExact(b
            .protocol("http:")
            .host("www.seanmcbeth.com:8081")
            .toString(), "http://www.seanmcbeth.com:8081/");
    }

    test_EmptyConstructor_SetProtocolAndHostName() {
        const b = new URLBuilder();
        this.areExact(b
            .protocol("http:")
            .hostName("www.seanmcbeth.com")
            .toString(), "http://www.seanmcbeth.com/");
    }

    test_EmptyConstructor_SetProtocolAndHostNameAndPathName() {
        const b = new URLBuilder();
        this.areExact(b
            .protocol("http:")
            .hostName("www.seanmcbeth.com")
            .path("images")
            .toString(), "http://www.seanmcbeth.com/images");
    }

    test_EmptyConstructor_SetProtocolAndPathNameAndHostName() {
        const b = new URLBuilder();
        this.areExact(b
            .protocol("http:")
            .path("images")
            .hostName("www.seanmcbeth.com")
            .toString(), "http://www.seanmcbeth.com/images");
    }

    test_EmptyConstructor_SetHostNameAndProtocolAndPathName() {
        const b = new URLBuilder();
        this.areExact(b
            .hostName("www.seanmcbeth.com")
            .protocol("http:")
            .path("images")
            .toString(), "http://www.seanmcbeth.com/images");
    }

    test_EmptyConstructor_SetHostNameAndPathNameAndProtocol() {
        const b = new URLBuilder();
        this.areExact(b
            .hostName("www.seanmcbeth.com")
            .path("images")
            .protocol("http:")
            .toString(), "http://www.seanmcbeth.com/images");
    }

    test_EmptyConstructor_SetPathNameAndProtocolAndHostName() {
        const b = new URLBuilder();
        this.areExact(b
            .path("images")
            .protocol("http:")
            .hostName("www.seanmcbeth.com")
            .toString(), "http://www.seanmcbeth.com/images");
    }

    test_EmptyConstructor_SetPathNameAndHostNameAndProtocol() {
        const b = new URLBuilder();
        this.areExact(b
            .path("images")
            .hostName("www.seanmcbeth.com")
            .protocol("http:")
            .toString(), "http://www.seanmcbeth.com/images");
    }

    test_EmptyConstructor_BaseAndPathName() {
        const b = new URLBuilder();
        this.areExact(b
            .path("")
            .base(new URL("https://www.seanmcbeth.com"))
            .toString(), "https://www.seanmcbeth.com/");
    }

    test_EmptyConstructor_PathNameAndBase() {
        const b = new URLBuilder();
        this.areExact(b
            .base("https://www.seanmcbeth.com")
            .path("")
            .toString(), "https://www.seanmcbeth.com/");
    }

    test_CantResetBase1() {
        const b = new URLBuilder("https://www.seanmcbeth.com");
        this.throws(() => b.base("https://yahoo.com"));
    }

    test_CantResetBase2() {
        const b = new URLBuilder()
            .base("https://www.seanmcbeth.com")
            .path("");
        this.throws(() => b.base("https://yahoo.com"));
    }

    test_CanResetBaseBeforePathIsSet() {
        const b = new URLBuilder()
            .base("https://www.seanmcbeth.com")
            .base("https://yahoo.com")
            .path("");
        this.areExact(b.toString(), "https://yahoo.com/");
    }

    test_PathPush1() {
        const b = new URLBuilder("https://www.seanmcbeth.com")
            .pathPush("images");
        this.areExact(b.toString(), "https://www.seanmcbeth.com/images");
    }

    test_PathPush2() {
        const b = new URLBuilder("https://www.seanmcbeth.com")
            .pathPush("images")
            .pathPush("archive");
        this.areExact(b.toString(), "https://www.seanmcbeth.com/images/archive");
    }

    test_PathPop() {
        const b = new URLBuilder("https://www.seanmcbeth.com/images/archive")
            .pathPop();
        this.areExact(b.toString(), "https://www.seanmcbeth.com/images");
    }

    test_PathPopPush() {
        const b = new URLBuilder("https://www.seanmcbeth.com/images/archive")
            .pathPop()
            .pathPush("whatever");
        this.areExact(b.toString(), "https://www.seanmcbeth.com/images/whatever");
    }

    test_UndefinedConstructor() {
        this.doesNotThrow(() =>
            new URLBuilder(undefined));
    }

    test_UndefinedUndefinedConstructor() {
        this.doesNotThrow(() =>
            new URLBuilder(undefined, undefined));
    }

    test_UndefinedNullConstructor() {
        this.doesNotThrow(() =>
            new URLBuilder(undefined, null));
    }

    test_NullConstructor() {
        this.throws(() =>
            new URLBuilder(null));
    }

    test_NullUndefinedConstructor() {
        this.throws(() =>
            new URLBuilder(null, undefined));
    }

    test_NullNullConstructor() {
        this.throws(() =>
            new URLBuilder(null, null));
    }

    test_FullURLConstructor() {
        this.doesNotThrow(() => {
            const b = new URLBuilder("https://www.seanmcbeth.com");
            this.areExact(b.toString(), "https://www.seanmcbeth.com/");
        });
    }

    test_RelativeConstructorWithStringBase1() {
        this.doesNotThrow(() => {
            const b = new URLBuilder("image.jpg", "https://www.seanmcbeth.com");
            this.areExact(b.toString(), "https://www.seanmcbeth.com/image.jpg");
        });
    }

    test_RelativeConstructorWithNullBase() {
        this.throws(() =>
            new URLBuilder("image.jpg", null));
    }

    test_RelativeConstructorWithUndefinedBase() {
        this.throws(() =>
            new URLBuilder("image.jpg", undefined));
    }

    test_RelativeConstructorWithStringBase2() {
        this.doesNotThrow(() => {
            const b = new URLBuilder("image.jpg", "https://www.seanmcbeth.com/images/");
            this.areExact(b.toString(), "https://www.seanmcbeth.com/images/image.jpg");
        });
    }

    test_RelativeConstructorWithStringBase3() {
        this.doesNotThrow(() => {
            const b = new URLBuilder("../image.jpg", "https://www.seanmcbeth.com/images/");
            this.areExact(b.toString(), "https://www.seanmcbeth.com/image.jpg");
        });
    }

    test_AbsoluteConstructorWithStringBase1() {
        this.doesNotThrow(() => {
            const b = new URLBuilder("/image.jpg", "https://www.seanmcbeth.com");
            this.areExact(b.toString(), "https://www.seanmcbeth.com/image.jpg");
        });
    }

    test_AbsoluteConstructorWithStringBase2() {
        this.doesNotThrow(() => {
            const b = new URLBuilder("/image.jpg", "https://www.seanmcbeth.com/images");
            this.areExact(b.toString(), "https://www.seanmcbeth.com/image.jpg");
        });
    }
}