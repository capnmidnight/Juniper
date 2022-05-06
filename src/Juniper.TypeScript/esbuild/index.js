import { globalExternals } from "@fal-works/esbuild-plugin-global-externals";
import { build as esbuild } from "esbuild";
function normalizeDirName(dirName) {
    if (!dirName.endsWith('/')) {
        dirName += '/';
    }
    return dirName;
}
export class Build {
    browserEntries = new Array();
    minBrowserEntries = new Array();
    plugins = new Array();
    defines = new Array();
    externals = new Array();
    globalExternals = new Array();
    isWatch;
    rootDirName = "src/";
    outDirName = "wwwroot/js/";
    constructor(args) {
        args.sort();
        this.isWatch = args.indexOf("--watch") !== -1;
    }
    rootDir(name) {
        this.rootDirName = normalizeDirName(name);
        return this;
    }
    outDir(name) {
        this.outDirName = normalizeDirName(name);
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
    globalExternal(packageName, globalName) {
        this.globalExternals.push([packageName, globalName]);
        return this;
    }
    bundle(name) {
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
    makeBundle(entryPoints, name, minify) {
        const JS_EXT = minify ? ".min" : "";
        const entryNames = `[dir]/[name]${JS_EXT}`;
        const define = {
            DEBUG: JSON.stringify(!minify),
            JS_EXT: JSON.stringify(JS_EXT + ".js")
        };
        for (const def of this.defines) {
            const [key, value] = def(minify);
            define[key] = value;
        }
        const plugins = this.plugins.map((p) => p(minify));
        if (this.globalExternals.length > 0) {
            const config = {};
            for (const [packageName, globalName] of this.globalExternals) {
                config[packageName] = globalName;
            }
            plugins.unshift(globalExternals(config));
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
//# sourceMappingURL=index.js.map