import { build as esbuild } from "esbuild";
export class Build {
    browserEntries = new Array();
    minBrowserEntries = new Array();
    workerEntries = new Array();
    plugins = new Array();
    defines = new Array();
    externals = new Array();
    minWorkerEntries = new Array();
    isWatch;
    rootDirName = "src";
    outDirName = "wwwroot";
    bundleOutDirName = "js";
    workerOutDirName = "workers";
    constructor(args) {
        args.sort();
        this.isWatch = args.indexOf("--watch") !== -1;
    }
    rootDir(name) {
        this.rootDirName = name;
        return this;
    }
    outDir(name) {
        this.outDirName = name;
        return this;
    }
    bundleOutDir(name) {
        this.bundleOutDirName = name;
        return this;
    }
    workerOutDir(name) {
        this.workerOutDirName = name;
        return this;
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
        this.task(name, false);
        return this;
    }
    worker(name) {
        this.task(name, true);
        return this;
    }
    task(name, isWorker) {
        const entry = `${this.rootDirName}/${name}/index.ts`;
        if (isWorker) {
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
        const tasks = [
            this.makeBundle(this.browserEntries, "browser bundles", false, false),
            this.makeBundle(this.workerEntries, "workers", false, true),
            this.makeBundle(this.minBrowserEntries, "minified browser bundles", true, false),
            this.makeBundle(this.minWorkerEntries, "minified workers", true, true)
        ];
        await Promise.all(tasks).then(() => {
            const end = Date.now();
            const delta = (end - start) / 1000;
            console.log(`done in ${delta}s`);
        });
    }
    makeBundle(entryPoints, name, minify, isWorker) {
        const JS_EXT = minify ? ".min" : "";
        const outDirParts = [
            this.outDirName,
            isWorker
                ? this.workerOutDirName
                : this.bundleOutDirName
        ];
        const outdir = outDirParts.filter(x => x).join("/");
        const entryNames = `[dir]/[name]${JS_EXT}`;
        const define = {
            DEBUG: JSON.stringify(!minify),
            JS_EXT: JSON.stringify(JS_EXT + ".js")
        };
        for (const def of this.defines) {
            const [key, value] = def(minify);
            define[key] = value;
        }
        return esbuild({
            platform: "browser",
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
