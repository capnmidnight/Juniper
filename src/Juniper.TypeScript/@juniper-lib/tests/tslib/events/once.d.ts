import { TestCase } from "@juniper-lib/testing/tdd/TestCase";
export declare class OnceTests extends TestCase {
    private rig;
    setup(): void;
    test_Good(): Promise<void>;
    test_WithValue(): Promise<void>;
    test_Timeout(): Promise<void>;
    test_NoTimeout(): Promise<void>;
    test_Bad(): Promise<void>;
}
export declare class SuccessTests extends TestCase {
    private rig;
    setup(): void;
    test_Good(): Promise<void>;
    test_WithValue(): Promise<void>;
    test_Timeout(): Promise<void>;
    test_NoTimeout(): Promise<void>;
    test_Bad(): Promise<void>;
}
//# sourceMappingURL=once.d.ts.map