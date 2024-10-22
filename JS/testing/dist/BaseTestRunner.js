import { AssertFailedException } from "@juniper-lib/util";
export class BaseTestRunner {
    test(name, args, ...callbacks) {
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
                    this.error(exp, "❌", `${name}:${args}`);
                }
            }
        }
        if (passed) {
            this.success(`✅ ${name}:${args}`);
        }
    }
    log(..._msg) {
        throw new Error("Not implemented");
    }
    success(..._msg) {
        throw new Error("Not implemented");
    }
    fail(..._msg) {
        throw new Error("Not implemented");
    }
    error(_exp, ..._msg) {
        throw new Error("Not implemented");
    }
}
//# sourceMappingURL=BaseTestRunner.js.map