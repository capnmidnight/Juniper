import { Build, runBuilds } from "@juniper-lib/esbuild";

const args = process.argv.slice(2);

await runBuilds(args,
    new Build(args, false)
        .find("Pages")
        .outBase("./")
        .outDir("wwwroot/js/")
        .seperateMinifiedFiles(true)
        .splitting(false)
);