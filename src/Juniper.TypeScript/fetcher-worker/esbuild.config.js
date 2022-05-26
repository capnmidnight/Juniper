import { Build } from "@juniper-lib/esbuild";

await new Build(process.argv.slice(2))
    .bundle("./")
    .outDir("dist")
    .run();