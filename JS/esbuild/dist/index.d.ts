import { ModuleInfo } from "@fal-works/esbuild-plugin-global-externals";
import { BuildOptions, Loader, Plugin } from "esbuild";
type Define = [string, string];
type DefineFactory = (minify: boolean) => Define;
type OptionAlterer = (opts: BuildOptions) => void;
type PluginFactory = (minify: boolean) => Plugin;
export declare class Build {
    #private;
    get buildType(): "watch" | "build";
    constructor(args: string[], buildWorkers: boolean);
    entryName(name: string): this;
    outDir(name: string): this;
    outBase(name: string): this;
    plugin(pgn: PluginFactory): this;
    define(def: DefineFactory): this;
    external(extern: string): this;
    externAllDependencies(): this;
    globalExternal(packageName: string, info: ModuleInfo): this;
    addThreeJS(enabled: boolean): this;
    loaders(loaders: {
        [ext: string]: Loader;
    }): this;
    seperateMinifiedFiles(v: boolean): this;
    splitting(enable: boolean): this;
    extensionPrefix(prefix: string): this;
    bundleDir(dirPath: string): this;
    bundleFile(filePath: string): this;
    bundleDirs(...dirNames: string[]): this;
    bundleFiles(...fileNames: string[]): this;
    find(...rootDirs: string[]): this;
    manually(thunk: OptionAlterer): this;
    _run(onStart: (name: string) => void, onEnd: (name: string) => void): Promise<void>;
}
export declare function bundle(args: string[], ...paths: string[]): Promise<void>;
export declare function runBuilds(args: string[], ...builds: Build[]): Promise<void>;
export {};
//# sourceMappingURL=index.d.ts.map