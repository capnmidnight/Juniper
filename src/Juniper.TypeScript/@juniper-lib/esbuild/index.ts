import { globalExternals } from "@fal-works/esbuild-plugin-global-externals";
import { build as esbuild, Plugin } from "esbuild";

type Define = [string, string];
type DefineFactory = (minify: boolean) => Define;
type DefMap = { [key: string]: string };

type PluginFactory = (minify: boolean) => Plugin;

function normalizeDirName(dirName: string): string {
    if (!dirName.endsWith('/')) {
        dirName += '/';
    }
    return dirName;
}

export class Build {
    private readonly browserEntries = new Array<string>();
    private readonly minBrowserEntries = new Array<string>();
    private readonly plugins = new Array<PluginFactory>();
    private readonly defines = new Array<DefineFactory>();
    private readonly externals = new Array<string>();
    private readonly globalExternals = new Array<[string, string]>();

    private readonly isWatch: boolean;

    public get buildType() {
        return this.isWatch ? "watch" : "build";
    }

    private entryNames = "[dir]/[name]";
    private outbase = "src";
    private rootDirName = "src/";
    private outDirName = "wwwroot/js/";

    constructor(args: string[]) {
        this.isWatch = args.indexOf("--watch") !== -1;
    }

    entryName(name: string) {
        this.entryNames = name;
        return this;
    }

    rootDir(name: string) {
        this.rootDirName = normalizeDirName(name);
        return this;
    }

    outDir(name: string) {
        this.outDirName = normalizeDirName(name);
        return this;
    }

    outBase(name: string) {
        this.outbase = name;
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

    globalExternal(packageName: string, globalName: string) {
        this.globalExternals.push([packageName, globalName]);
        return this;
    }

    bundle(name: string) {
        name = normalizeDirName(name);
        const entry = this.rootDirName + name + "index.ts";
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
        const entryNames = this.entryNames + JS_EXT;
        const define: DefMap = {
            DEBUG: JSON.stringify(!minify),
            JS_EXT: JSON.stringify(JS_EXT + ".js")
        };

        for (const def of this.defines) {
            const [key, value] = def(minify);
            define[key] = value;
        }

        const plugins = this.plugins.map((p) => p(minify));
        if (this.globalExternals.length > 0) {
            const config: Record<string, string> = {};
            for (const [packageName, globalName] of this.globalExternals) {
                config[packageName] = globalName;
            }
            plugins.unshift(globalExternals(config));
        }

        return esbuild({
            platform: "browser",
            color: true,
            logLevel: "warning",
            format: "esm",
            target: "exnext",
            bundle: true,
            sourcemap: true,
            entryPoints,
            outbase: this.outbase,
            outdir: this.outDirName,
            entryNames,
            define,
            minify,
            external: this.externals,
            plugins,
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