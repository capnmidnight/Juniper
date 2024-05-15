import { Build, runBuilds } from "@juniper-lib/esbuild";

const args = process.argv.slice(2);

// Builds can run in parallel.
await runBuilds(args,
    // This will iterate through a folder and find all
    // of the `index.ts` files.
    findBundles(false, "Pages"),

    // We can also build bundles meant to be ran as WebWorkers
    findBundles(true, "workers")
);

/**
 * Use this function to customize build settings common across your project.
 * 
 * @param {boolean} isWorker the discovered bundles are meant to be WebWorkers.
 * @param {...string} dirs the directories to search for `index.ts` files.
 */
function findBundles(isWorker, ...dirs) {
    return new Build(args, isWorker)
        .find(...dirs)

        // the common base directory between inputs and outputs.
        .outBase("./")

        // the output location will be the input.ts file's full path, 
        // minus the path of the outBase, prefixed with the path for
        // the outDir.
        .outDir("wwwroot/js/")

        // remove this line if you would prefer minified release bundles
        // to overwrite unminified development bundles.
        .seperateMinifiedFiles(true)

        // ESBuild can split output bundles into chunks that are common
        // between bundles to help leverage browser caching.
        .splitting(!isWorker);
}