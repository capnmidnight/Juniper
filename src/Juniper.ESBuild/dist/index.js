import { build as esbuild } from "esbuild";
export class Build {
    browserEntries = new Array();
    minBrowserEntries = new Array();
    testEntries = new Array();
    minTestEntries = new Array();
    workerEntries = new Array();
    plugins = new Array();
    defines = new Array();
    externals = new Array();
    minWorkerEntries = new Array();
    isWatch;
    isTest;
    constructor(args) {
        args.sort();
        this.isWatch = args.indexOf("--watch") !== -1;
        this.isTest = args.indexOf("--test") !== -1;
    }
    plugin(pgn) {
        this.plugins.push(pgn);
        return this;
    }
    define(def) {
        this.defines.push(def);
        return this;
    }
    external(extern) {
        this.externals.push(extern);
        return this;
    }
    bundle(name) {
        this.task("src", name, false, false);
        return this;
    }
    worker(name) {
        this.task("src", name, false, true);
        return this;
    }
    test(name) {
        this.task("tests", name, true, false);
        return this;
    }
    task(root, name, isTest, isWorker) {
        const entry = `${root}/${name}/index.ts`;
        if (isTest) {
            this.testEntries.push(entry);
            this.minTestEntries.push(entry);
        }
        else if (isWorker) {
            this.workerEntries.push(entry);
            this.minWorkerEntries.push(entry);
        }
        else {
            this.browserEntries.push(entry);
            this.minBrowserEntries.push(entry);
        }
    }
    async run() {
        const start = Date.now();
        const tasks = this.isTest
            ? [
                this.makeBundle(this.testEntries, "minified browser bundles", true, false, false),
                this.makeBundle(this.minTestEntries, "minified workers", true, true, false)
            ]
            : [
                this.makeBundle(this.browserEntries, "browser bundles", false, false, false),
                this.makeBundle(this.workerEntries, "workers", false, false, true),
                this.makeBundle(this.minBrowserEntries, "minified browser bundles", false, true, false),
                this.makeBundle(this.minWorkerEntries, "minified workers", false, true, true)
            ];
        await Promise.all(tasks).then(() => {
            const end = Date.now();
            const delta = (end - start) / 1000;
            console.log(`done in ${delta}s`);
        });
    }
    makeBundle(entryPoints, name, isTest, minify, isWorker) {
        const JS_EXT = minify ? ".min" : "";
        const outdir = `wwwroot/${isWorker ? "workers" : "js"}`;
        const stub = isTest ? "-test" : "";
        const entryNames = `[dir]/[name]${stub}${JS_EXT}`;
        const define = {
            WORKER: JSON.stringify(isWorker),
            DEBUG: JSON.stringify(!minify),
            JS_EXT: JSON.stringify(JS_EXT + ".js")
        };
        for (const def of this.defines) {
            const [key, value] = def(minify);
            define[key] = value;
        }
        return esbuild({
            color: true,
            outbase: "src",
            logLevel: "warning",
            format: "esm",
            target: "es2019",
            bundle: true,
            sourcemap: true,
            entryPoints,
            outdir,
            entryNames,
            define,
            minify,
            external: this.externals,
            plugins: this.plugins.map(p => p(minify)),
            incremental: this.isWatch,
            legalComments: "none",
            watch: this.isWatch && {
                onRebuild(error, result) {
                    if (error) {
                        console.error(name, "failed.", error, result);
                    }
                    else {
                        console.log(name, "rebuilt");
                    }
                }
            }
        });
    }
}
