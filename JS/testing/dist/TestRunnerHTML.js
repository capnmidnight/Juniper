import { BaseTestRunner } from "./BaseTestRunner";
if ("document" in globalThis) {
    const link = document.createElement("link");
    const css = `pre {
    margin: 0;
    padding: .5rem;
}

pre.log {
    background-color: hsl(0, 0%, 90%);
}

pre.success {
    background-color: hsl(120deg, 100%, 75%);
}

pre.fail {
    background-color: hsl(60deg, 100%, 85%);
}

pre.error {
    color: white;
    font-weight: bold;
    background-color: hsl(0, 100%, 85%);
}`;
    const blob = new Blob([css], { type: "text/css" });
    link.rel = "stylesheet";
    link.href = URL.createObjectURL(blob);
    document.head.append(link);
}
export class TestRunnerHTML extends BaseTestRunner {
    #print(type, ...msg) {
        const pre = document.createElement("pre");
        pre.append(...msg);
        pre.className = type;
        document.body.append(pre);
    }
    log(...msg) {
        this.#print("log", ...msg);
    }
    success(...msg) {
        this.#print("success", ...msg);
    }
    fail(...msg) {
        this.#print("fail", ...msg);
    }
    error(exp, ...msg) {
        this.#print("error", ...msg, exp.message || exp.target.error || exp);
    }
}
//# sourceMappingURL=TestRunnerHTML.js.map