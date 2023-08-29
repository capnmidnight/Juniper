import { Build } from "@juniper/esbuild";
import { glsl } from "esbuild-plugin-glsl";

const args = process.argv.slice(2);

await (new Build(args)
    .plugin((minify) => glsl({ minify }))
    .addThreeJS(true)
    .rootDir("src")
    .outBase("src")
    .entryName("[dir]/[name]")
    .outDir("wwwroot/js")

    .bundle("junk")
    .run());