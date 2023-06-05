import { globalExternals, ModuleInfo } from "@fal-works/esbuild-plugin-global-externals";
import { build, Plugin, context, BuildOptions } from "esbuild";
import * as fs from "fs";
import * as path from "path";


type Define = [string, string];
type DefineFactory = (minify: boolean) => Define;
type DefMap = { [key: string]: string };
type OptionAlterer = (opts: BuildOptions) => void;
type PluginFactory = (minify: boolean) => Plugin;

export class Build {
    private readonly browserEntries = new Array<string>();
    private readonly minBrowserEntries = new Array<string>();
    private readonly plugins = new Array<PluginFactory>();
    private readonly defines = new Array<DefineFactory>();
    private readonly externals = new Array<string>();
    private readonly manualOptionsChanges = new Array<OptionAlterer>();
    private readonly globalExternals: Record<string, ModuleInfo> = {};

    private readonly isWatch: boolean;

    public get buildType() {
        return this.isWatch ? "watch" : "build";
    }

    private entryNames = "[dir]/[name]";
    private outbase = "src";
    private outDirName = "wwwroot/js/";

    constructor(args: string[], private readonly buildWorkers: boolean) {
        this.isWatch = args.indexOf("--watch") !== -1;
    }

    entryName(name: string) {
        this.entryNames = name;
        return this;
    }

    outDir(name: string) {
        this.outDirName = name;
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

    globalExternal(packageName: string, info: ModuleInfo) {
        this.globalExternals[packageName] = info;
        return this;
    }

    addThreeJS(enabled: boolean) {
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

    bundle(name: string) {
        const entry = path.join(name, "index.ts");
        this.browserEntries.push(entry);
        this.minBrowserEntries.push(entry);
        return this;
    }

    bundles(...names: string[]) {
        for (const name of names) {
            console.log(this.buildType, this.buildWorkers ? "worker" : "bundle", name);
            this.bundle(name);
        }
        return this;
    }

    find(...rootDirs: string[]) {
        const dirs = rootDirs
            .map(LS)
            .filter(identity);
        const entryPoints = findEntries(...dirs);
        return this.bundles(...entryPoints);
    }

    manually(thunk: OptionAlterer): this {
        this.manualOptionsChanges.push(thunk);
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

    private async makeBundle(entryPoints: string[], name: string, isRelease: boolean) {
        const JS_EXT = isRelease ? ".min" : "";
        const entryNames = this.entryNames + JS_EXT;
        const define: DefMap = {
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

        plugins.push({
            name: "my-plugin",
            setup(build) {
                let count = 0;
                build.onEnd(() => {
                    const type = count++ > 0 ? "rebuilt" : "built";
                    console.log(name, type);
                });
            },
        });

        const opts: BuildOptions = {
            platform: "browser",
            format: "esm",
            target: "es2021",
            logLevel: "error",
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
            legalComments: "none",
            treeShaking: true,
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

type DirSpec = [string, fs.Dirent[]];

function LS(path: string): DirSpec {
    if (!fs.existsSync(path)) {
        return null;
    }

    return [path, fs.readdirSync(path, {
        withFileTypes: true
    })];
}

function findEntries(...dirs: DirSpec[]) {
    return dirs
        .filter(identity)
        .map(withIndexDirs)
        .flatMap(identity);
}

function identity<T>(x: T): T {
    return x;
}

function withIndexDirs(dirSpec: DirSpec) {
    const [parent, dirs] = dirSpec;
    return dirs
        .filter(dir => hasIndexFile(parent, dir))
        .map(dir => path.join(parent, dir.name));
}

function hasIndexFile(parent: string, dir: fs.Dirent) {
    if (!dir.isDirectory()) {
        return false;
    }

    const fileName = path.join(parent, dir.name);
    const results = LS(fileName);
    if (!results) {
        return false;
    }

    const [_, files] = results;
    return files.filter(f => f.isFile()
        && f.name === "index.ts")
        .length === 1;
}