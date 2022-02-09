import { build as esbuild, Plugin } from "esbuild";

type Define = [string, string];
type DefineFactory = (minify: boolean) => Define;
type DefMap = { [key: string]: string };

type PluginFactory = (minify: boolean) => Plugin;

export class Build {
    private readonly browserEntries = new Array<string>();
    private readonly minBrowserEntries = new Array<string>();
    private readonly plugins = new Array<PluginFactory>();
    private readonly defines = new Array<DefineFactory>();
    private readonly externals = new Array<string>();

    private readonly isWatch: boolean;

    private rootDirName = "src";
    private outDirName = "wwwroot/js";

    constructor(args: string[]) {
        args.sort();
        this.isWatch = args.indexOf("--watch") !== -1;
    }

    rootDir(name: string) {
        this.rootDirName = name;
        return this;
    }

    outDir(name: string) {
        this.outDirName = name;
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
        const entry = `${this.rootDirName}/${name}/index.ts`;
        this.browserEntries.push(entry);
        this.minBrowserEntries.push(entry);
        return this;
    }

    async run() {
        const start = Date.now();

        const tasks = [
            this.makeBundle(this.browserEntries, "browser bundles", false),
            this.makeBundle(this.minBrowserEntries, "minified browser bundles", true)
        ];

        await Promise.all(tasks).then(() => {
            const end = Date.now();
            const delta = (end - start) / 1000;
            console.log(`done in ${delta}s`);
        });
    }

    private makeBundle(entryPoints: string[], name: string, minify: boolean) {
        const JS_EXT = minify ? ".min" : "";
        const entryNames = `[dir]/[name]${JS_EXT}`;
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
            outdir: this.outDirName,
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