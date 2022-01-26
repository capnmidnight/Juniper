import { Build } from "juniper-esbuild";

await new Build(process.argv.slice(2))
    .external("three")
    .bundle("./")
    .outDir("dist")
    .run();