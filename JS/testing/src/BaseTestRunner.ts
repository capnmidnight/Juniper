import { AssertFailedException } from "@juniper-lib/util";

export class BaseTestRunner {
    test(name: string, args: string, ...callbacks: VoidFunction[]) {
        let passed = true;
        for (const callback of callbacks) {
            try {
                callback();
            }
            catch (exp) {
                passed = false;
                if (exp instanceof AssertFailedException) {
                    this.fail(`❌ ${name}:${exp.name}:${args}. ${exp.message}`);
                }
                else {
                    this.error(exp as Error, "❌", `${name}:${args}`);
                }
            }
        }

        if (passed) {
            this.success(`✅ ${name}:${args}`);
        }
    }

    log(..._msg: any[]) {
        throw new Error("Not implemented");
    }

    success(..._msg: any[]) {
        throw new Error("Not implemented");
    }

    fail(..._msg: any[]) {
        throw new Error("Not implemented");
    }

    error(_exp: Error, ..._msg: any[]) {
        throw new Error("Not implemented");
    }
}
