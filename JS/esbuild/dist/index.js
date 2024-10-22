import { build, context } from "esbuild";
import * as fs from "fs";
import * as path from "path";
export class Build {
    get buildType() {
        return this.isWatch ? "watch" : "build";
    }
    constructor(args, buildWorkers) {
        this.buildWorkers = buildWorkers;
        this.browserEntries = new Array();
        this.minBrowserEntries = new Array();
        this.plugins = new Array();
        this.defines = new Array();
        this.externals = new Array();
        this.manualOptionsChanges = new Array();
        this.entryNames = "[dir]/[name]";
        this.outbase = "src";
        this.outDirName = "wwwroot/js/";
        this.enableSplitting = false;
        this.isWatch = args.indexOf("--watch") !== -1;
    }
    entryName(name) {
        this.entryNames = name;
        return this;
    }
    outDir(name) {
        this.outDirName = name;
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
    external(extern, enabled) {
        if (enabled) {
            this.externals.push(extern);
        }
        return this;
    }
    splitting(enable) {
        this.enableSplitting = enable;
        return this;
    }
    bundle(name) {
        const entry = path.join(name, "index.ts");
        this.browserEntries.push(entry);
        this.minBrowserEntries.push(entry);
        return this;
    }
    bundles(...names) {
        for (const name of names) {
            console.log(this.buildType, this.buildWorkers ? "worker" : "bundle", name);
            this.bundle(name);
        }
        return this;
    }
    find(...rootDirs) {
        const files = rootDirs
            .flatMap(dir => fs.readdirSync(dir, { withFileTypes: true })
            .filter(e => e
            && e.isDirectory()
            && e.name !== "node_modules"
            && e.name !== "bin"
            && e.name !== "obj")
            .map(e => path.join(dir, e.name)))
            .filter(x => x && fs.existsSync(path.join(x, "index.ts")));
        if (files.length > 0) {
            this.bundles(...files);
        }
        return this;
    }
    manually(thunk) {
        this.manualOptionsChanges.push(thunk);
        return this;
    }
    getTasks(onStart, onEnd) {
        return [
            this.makeBundle(this.browserEntries, "browser bundles", false, onStart, onEnd),
            this.makeBundle(this.minBrowserEntries, "minified browser bundles", true, onStart, onEnd)
        ];
    }
    async makeBundle(entryPoints, name, isRelease, onStart, onEnd) {
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
        plugins.push({
            name: "my-plugin",
            setup(build) {
                let count = 0;
                build.onStart(() => {
                    console.log("Building", name, ...entryPoints);
                    onStart();
                });
                build.onEnd((result) => {
                    const type = count++ > 0 ? "rebuilt" : "built";
                    console.log(name, type, ...Object.keys(result.metafile?.outputs || []).filter(v => v.endsWith(".js")));
                    onEnd();
                });
            },
        });
        const opts = {
            bundle: true,
            color: true,
            define,
            entryNames,
            entryPoints,
            external: this.externals,
            format: "esm",
            legalComments: "none",
            logLevel: "error",
            metafile: true,
            minify: isRelease,
            outbase: this.outbase,
            outdir: this.outDirName,
            platform: "browser",
            plugins,
            sourcemap: !isRelease,
            splitting: this.enableSplitting,
            treeShaking: true,
            tsconfigRaw: {
                compilerOptions: {
                    experimentalDecorators: true
                }
            }
        };
        for (const alterer of this.manualOptionsChanges) {
            alterer(opts);
        }
        if (!this.isWatch) {
            await build(opts);
        }
        else {
            const ctx = await context(opts);
            await ctx.watch();
            const stall = new Promise((_, __) => {
            });
            await stall;
            await ctx.dispose();
        }
    }
}
export async function runBuilds(...builds) {
    const running = new Set();
    const onStart = (build) => {
        if (running.size === 0) {
            console.log("Build started");
        }
        running.add(build);
    };
    const onEnd = (build) => {
        running.delete(build);
        if (running.size === 0) {
            console.log("Build complete, waiting for changes...");
        }
    };
    const tasks = builds.flatMap(build => build.getTasks(() => onStart(build), () => onEnd(build)));
    await Promise.all(tasks);
}
//# sourceMappingURL=index.js.map