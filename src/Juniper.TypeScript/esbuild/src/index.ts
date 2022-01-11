import { build as esbuild, Plugin } from "esbuild";

type Define = [string, string];
type DefineFactory = (minify: boolean) => Define;
type DefMap = { [key: string]: string };

type PluginFactory = (minify: boolean) => Plugin;

export class Build {
    private readonly browserEntries = new Array<string>();
    private readonly minBrowserEntries = new Array<string>();
    private readonly testEntries = new Array<string>();
    private readonly minTestEntries = new Array<string>();
    private readonly workerEntries = new Array<string>();
    private readonly plugins = new Array<PluginFactory>();
    private readonly defines = new Array<DefineFactory>();
    private readonly externals = new Array<string>();
    private readonly minWorkerEntries = new Array<string>();

    private readonly isWatch: boolean;
    private readonly isTest: boolean;

    private outDirName = "wwwroot";
    private bundleOutDirName = "workers";
    private workerOutDirName = "js";

    constructor(args: string[]) {
        args.sort();
        this.isWatch = args.indexOf("--watch") !== -1;
        this.isTest = args.indexOf("--test") !== -1;
    }

    outDir(name: string) {
        this.outDirName = name;
        return this;
    }

    bundleOutDir(name: string) {
        this.bundleOutDirName = name;
        return this;
    }

    workerOutDir(name: string) {
        this.workerOutDirName = name;
        return this;
    }

    plugin(pgn: PluginFactory) {
        this.plugins.push(pgn);
        return this;
    }

    define(def: DefineFactory) {
        this.defines.push(def);
        return this;
    }

    external(extern: string) {
        this.externals.push(extern);
        return this;
    }

    bundle(name: string) {
        this.task("src", name, false, false);
        return this;
    }

    worker(name: string) {
        this.task("src", name, false, true);
        return this;
    }

    test(name: string) {
        this.task("tests", name, true, false);
        return this;
    }

    private task(root: string, name: string, isTest: boolean, isWorker: boolean) {
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
                this.makeBundle(this.minTestEntries, "minified workers", true, true, false)]
            : [
                this.makeBundle(this.browserEntries, "browser bundles", false, false, false),
                this.makeBundle(this.workerEntries, "workers", false, false, true),
                this.makeBundle(this.minBrowserEntries, "minified browser bundles", false, true, false),
                this.makeBundle(this.minWorkerEntries, "minified workers", false, true, true)];

        await Promise.all(tasks).then(() => {
            const end = Date.now();
            const delta = (end - start) / 1000;
            console.log(`done in ${delta}s`);
        });
    }

    private makeBundle(entryPoints: string[], name: string, isTest: boolean, minify: boolean, isWorker: boolean) {
        const JS_EXT = minify ? ".min" : "";

        const outDirParts = [
            this.outDirName,
            isWorker
                ? this.workerOutDirName
                : this.bundleOutDirName
        ];
        const outdir = outDirParts.filter(x => x).join("/");

        const stub = isTest ? "-test" : "";
        const entryNames = `[dir]/[name]${stub}${JS_EXT}`;
        const define: DefMap = {
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