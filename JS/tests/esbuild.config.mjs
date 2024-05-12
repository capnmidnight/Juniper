import { bundle } from "@juniper-lib/esbuild";

const args = process.argv.slice(2);

await bundle(args,
    "src/tests/dom/boundsComputing/index.ts",
    "src/tests/util/arrays/index.ts",
    "src/tests/widgets/data-table/index.ts",
    "src/tests/widgets/inclusion-list/index.ts"
);
