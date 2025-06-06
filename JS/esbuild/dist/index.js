import { globalExternals } from "@fal-works/esbuild-plugin-global-externals";
import { build, context } from "esbuild";
import * as fs from "fs";
import * as path from "path";
function stringRepeat(str, count, sep = "") {
    if (count < 0) {
        throw new Error("Can't repeat negative times: " + count);
    }
    let sb = "";
    for (let i = 0; i < count; ++i) {
        sb += str;
        if (i < count - 1) {
            sb += sep;
        }
    }
    return sb;
}
export class Build {
    #browserEntries = new Array();
    #minBrowserEntries = new Array();
    #plugins = new Array();
    #defines = new Array();
    #externals = new Array();
    #manualOptionsChanges = new Array();
    #globalExternals = {};
    #loaderConfig = {
        ".js": "js",
        ".ts": "ts",
        ".json": "json",
        ".css": "css",
        ".module.css": "local-css",
        ".frag": "text",
        ".vert": "text",
        ".glsl": "text"
    };
    #isWatch;
    #buildWorkers;
    #entryName = "[dir]/[name]";
    #outBase = "src";
    #outDir = "wwwroot/js/";
    #extensionPrefix = "";
    #enableSeperateMinifiedFiles = false;
    #enableSplitting = false;
    get buildType() {
        return this.#isWatch ? "watch" : "build";
    }
    constructor(args, buildWorkers) {
        this.#isWatch = args.indexOf("--watch") !== -1;
        this.#buildWorkers = buildWorkers;
    }
    entryName(name) {
        this.#entryName = name;
        return this;
    }
    outDir(name) {
        this.#outDir = name;
        return this;
    }
    outBase(name) {
        this.#outBase = name;
        return this;
    }
    plugin(pgn) {
        this.#plugins.push(pgn);
        return this;
    }
    define(def) {
        this.#defines.push(def);
        return this;
    }
    external(extern) {
        this.#externals.push(extern);
        return this;
    }
    externAllDependencies() {
        const pkgText = fs.readFileSync("package.json", { encoding: "utf8" });
        const pkg = JSON.parse(pkgText);
        if ("dependencies" in pkg) {
            for (const name of Object.keys(pkg.dependencies)) {
                this.external(name);
            }
        }
        if ("devDependencies" in pkg) {
            for (const name of Object.keys(pkg.devDependencies)) {
                this.external(name);
            }
        }
        return this;
    }
    globalExternal(packageName, info) {
        this.#globalExternals[packageName] = info;
        return this;
    }
    addThreeJS(enabled) {
        if (enabled) {
            const threeJS = fs.readFileSync("node_modules/three/build/three.module.js", { encoding: "utf8" });
            const match = /^export\s*\{\s*(((\w+\s+as\s+)?\w+,\s*)*((\w+\s+as\s+)?\w+))\s*}/gmi.exec(threeJS);
            const namedExports = match[1]
                .replace(/\b\w+\s+as\s+/g, "")
                .split(",")
                .map(v => v.trim());
            this.globalExternal("three", {
                varName: "THREE",
                namedExports,
                defaultExport: false
            });
        }
        return this;
    }
    loaders(loaders) {
        Object.assign(this.#loaderConfig, loaders);
        return this;
    }
    seperateMinifiedFiles(v) {
        this.#enableSeperateMinifiedFiles = v;
        return this;
    }
    splitting(enable) {
        this.#enableSplitting = enable;
        return this;
    }
    extensionPrefix(prefix) {
        this.#extensionPrefix = prefix;
        return this;
    }
    bundleDir(dirPath) {
        const entry = path.join(dirPath, "index.ts");
        return this.bundleFile(entry);
    }
    bundleFile(filePath) {
        this.#browserEntries.push(filePath);
        this.#minBrowserEntries.push(filePath);
        return this;
    }
    bundleDirs(...dirNames) {
        for (const dirName of dirNames) {
            console.log(this.buildType, this.#buildWorkers ? "worker" : "bundle", dirName);
            this.bundleDir(dirName);
        }
        return this;
    }
    bundleFiles(...fileNames) {
        for (const fileName of fileNames) {
            console.log(this.buildType, this.#buildWorkers ? "worker" : "bundle", fileName);
            this.bundleFile(fileName);
        }
        return this;
    }
    find(...rootDirs) {
        function* recurse(dirs) {
            while (dirs.length > 0) {
                const dir = dirs.shift();
                const subDirs = fs.readdirSync(dir, { withFileTypes: true })
                    .filter(e => e.isDirectory()
                    && e.name !== "node_modules"
                    && e.name !== "bin"
                    && e.name !== "obj")
                    .map(e => path.join(dir, e.name));
                dirs.push(...subDirs);
                yield dir;
            }
        }
        const entryPoints = Array.from(recurse(rootDirs))
            .map(x => path.join(x, "index.ts"))
            .filter(x => fs.existsSync(x));
        return this.bundleFiles(...entryPoints);
    }
    manually(thunk) {
        this.#manualOptionsChanges.push(thunk);
        return this;
    }
    async _run(onStart, onEnd) {
        const start = Date.now();
        const mode = `[${this.buildType}]`;
        const tasks = new Array();
        if (this.#isWatch || this.#enableSeperateMinifiedFiles) {
            tasks.push(this.#makeBundle(this.#browserEntries, mode, false, onStart, onEnd));
        }
        if (!this.#isWatch) {
            tasks.push(this.#makeBundle(this.#minBrowserEntries, mode, true, onStart, onEnd));
        }
        await Promise.all(tasks).then(() => {
            const end = Date.now();
            const delta = (end - start) / 1000;
            console.log(`done in ${delta}s`);
        });
    }
    async #makeBundle(entryPoints, mode, isRelease, onStart, onEnd) {
        let EXT_PRE = this.#extensionPrefix;
        if (isRelease && this.#enableSeperateMinifiedFiles) {
            EXT_PRE += ".min";
        }
        const entryNames = this.#entryName + EXT_PRE;
        const define = {
            IS_WORKER: JSON.stringify(this.#buildWorkers)
        };
        for (const def of this.#defines) {
            const [key, value] = def(isRelease);
            define[key] = value;
        }
        const plugins = this.#plugins.map((p) => p(isRelease));
        if (Object.keys(this.#globalExternals).length > 0) {
            plugins.unshift(globalExternals(this.#globalExternals));
        }
        const names = entryPoints.map(entryPoint => {
            const parts = entryPoint.split(/[\\\/]/);
            while (parts.length > 2) {
                parts.shift();
            }
            return parts.join("/");
        });
        const label = isRelease ? "[min-bundle]" : "[bundle]";
        const bundleName = names.join(", ");
        let ranOnce = false;
        let runCount = 0;
        plugins.push({
            name: "my-plugin",
            setup(build) {
                build.onStart(() => {
                    if (onStart) {
                        onStart(bundleName);
                    }
                    const type = ranOnce ? "rebuilding" : "building";
                    console.log(mode, label, bundleName, type, ...entryPoints);
                    ++runCount;
                });
                build.onEnd((result) => {
                    --runCount;
                    if (runCount === 0) {
                        if (result.errors && result.errors.length > 0) {
                            const errorMessage = result.errors.map(error => {
                                const spacer = stringRepeat("-", error.location.column - 1) + "^";
                                return `  [${error.id}]: ${error.location?.file} (Line ${error.location?.line}, Col ${error.location?.column})
      ${error.location?.lineText}
      ${spacer}
      ${error.location?.suggestion}
      ${error.text}
`;
                            }).join("\n");
                            console.log(mode, label, bundleName, "error\n", errorMessage);
                        }
                        else {
                            const type = ranOnce ? "rebuilt" : "built";
                            console.log(mode, label, bundleName, type, ...Object.keys(result.metafile.outputs).filter(v => v.endsWith(".js")));
                        }
                    }
                    ranOnce = true;
                    if (onEnd) {
                        onEnd(bundleName);
                    }
                });
            },
        });
        const opts = {
            bundle: true,
            color: true,
            define,
            entryNames,
            entryPoints,
            external: this.#externals,
            format: "esm",
            legalComments: "none",
            logLevel: "error",
            metafile: true,
            minify: isRelease,
            outbase: this.#outBase,
            outdir: this.#outDir,
            platform: "browser",
            plugins,
            sourcemap: !isRelease,
            splitting: this.#enableSplitting,
            treeShaking: true,
            loader: this.#loaderConfig,
            tsconfigRaw: {
                compilerOptions: {
                    experimentalDecorators: true
                }
            }
        };
        for (const alterer of this.#manualOptionsChanges) {
            alterer(opts);
        }
        if (!this.#isWatch) {
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
export async function bundle(args, ...paths) {
    const build = new Build(args, false)
        .outDir("src")
        .extensionPrefix(".bundle");
    for (const path of paths) {
        build.bundleFile(path);
    }
    await runBuilds(args, build);
}
export async function runBuilds(args, ...builds) {
    const isRelease = args.indexOf("--watch") === -1;
    const mode = isRelease ? "[build]" : "[watch]";
    const queue = new Map();
    const isZero = () => Array.from(queue.values()).reduce((a, b) => a + b, 0) === 0;
    const onStart = (name) => {
        if (isZero()) {
            console.log(mode, "build started");
        }
        if (!queue.has(name)) {
            queue.set(name, 0);
        }
        queue.set(name, queue.get(name) + 1);
    };
    const onEnd = (name) => {
        if (queue.has(name)) {
            queue.set(name, queue.get(name) - 1);
        }
        if (isZero()) {
            console.log(mode, "build finished, waiting for changes...");
        }
    };
    await Promise.all(builds.map(build => build._run(onStart, onEnd)));
    console.log(mode, "build finished");
}
//# sourceMappingURL=index.js.map