import { Build } from "juniper-esbuild";

await new Build(process.argv.slice(2))
    .worker("fetcher")
    .outDir("./")
    .run();