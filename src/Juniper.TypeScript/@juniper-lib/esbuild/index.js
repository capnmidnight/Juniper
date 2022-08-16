import { globalExternals } from "@fal-works/esbuild-plugin-global-externals";
import { build as esbuild } from "esbuild";
import * as fs from "fs";
function normalizeDirName(dirName) {
    if (!dirName.endsWith('/')) {
        dirName += '/';
    }
    return dirName;
}
export class Build {
    constructor(args, buildWorkers) {
        this.buildWorkers = buildWorkers;
        this.browserEntries = new Array();
        this.minBrowserEntries = new Array();
        this.plugins = new Array();
        this.defines = new Array();
        this.externals = new Array();
        this.globalExternals = {};
        this.entryNames = "[dir]/[name]";
        this.outbase = "src";
        this.rootDirName = "src/";
        this.outDirName = "wwwroot/js/";
        this.isWatch = args.indexOf("--watch") !== -1;
    }
    get buildType() {
        return this.isWatch ? "watch" : "build";
    }
    entryName(name) {
        this.entryNames = name;
        return this;
    }
    rootDir(name) {
        this.rootDirName = normalizeDirName(name);
        return this;
    }
    outDir(name) {
        this.outDirName = normalizeDirName(name);
        return this;
    }
    outBase(name) {
        this.outbase = name;
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
    globalExternal(packageName, info) {
        this.globalExternals[packageName] = info;
        return this;
    }
    addThreeJS() {
        const threeJS = fs.readFileSync("node_modules/three/build/three.module.js", { encoding: "utf8" });
        const match = /^export\s*\{\s*(((\w+\s+as\s+)?\w+,\s*)*((\w+\s+as\s+)?\w+))\s*}/gmi.exec(threeJS);
        const namedExports = match[1]
            .replace(/\b\w+\s+as\s+/g, "")
            .split(',')
            .map(v => v.trim());
        this.globalExternal("three", {
            varName: "THREE",
            namedExports,
            defaultExport: false
        });
        return this;
    }
    bundle(name) {
        name = normalizeDirName(name);
        const entry = this.rootDirName + name + "index.ts";
        this.browserEntries.push(entry);
        this.minBrowserEntries.push(entry);
        return this;
    }
    bundles(names) {
        for (const name of names) {
            console.log(this.buildType, this.buildWorkers ? "worker" : "bundle", name);
            this.bundle(name);
        }
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
    makeBundle(entryPoints, name, isRelease) {
        const JS_EXT = isRelease ? ".min" : "";
        const entryNames = this.entryNames + JS_EXT;
        const define = {
            DEBUG: JSON.stringify(!isRelease),
            IS_WORKER: JSON.stringify(this.buildWorkers)
        };
        for (const def of this.defines) {
            const [key, value] = def(isRelease);
            define[key] = value;
        }
        const plugins = this.plugins.map((p) => p(isRelease));
        if (Object.keys(this.globalExternals).length > 0) {
            plugins.unshift(globalExternals(this.globalExternals));
        }
        return esbuild({
            platform: "browser",
            format: "esm",
            target: "es2021",
            logLevel: "warning",
            color: true,
            bundle: true,
            sourcemap: !isRelease,
            entryPoints,
            outbase: this.outbase,
            outdir: this.outDirName,
            entryNames,
            define,
            minify: isRelease,
            external: this.externals,
            plugins,
            incremental: this.isWatch,
            legalComments: "none",
            treeShaking: true,
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