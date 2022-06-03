import { Build } from "@juniper-lib/esbuild";

await new Build(process.argv.slice(2))
    .external("three")
    .bundle("./")
    .outDir("dist")
    .run();