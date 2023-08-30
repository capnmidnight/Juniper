import { Build } from "@juniper/esbuild";

const args = process.argv.slice(2);

await (new Build(args, false)
    .outDir("wwwroot/js")
    .bundle("src/junk")
    .run()
);