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

    #print(type: string, ...msg: any[]) {
        const pre = document.createElement("pre");
        pre.append(...msg);
        pre.className = type;
        document.body.append(pre);
    }

    override log(...msg: any[]) {
        this.#print("log", ...msg);
    }

    override success(...msg: any[]) {
        this.#print("success", ...msg);
    }

    override fail(...msg: any[]) {
        this.#print("fail", ...msg);
    }

    override error(exp: any, ...msg: any[]) {
        this.#print("error", ...msg, exp.message || exp.target.error || exp);
    }
}