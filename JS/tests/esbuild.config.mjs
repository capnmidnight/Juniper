import { bundle } from "@juniper-lib/esbuild";

const args = process.argv.slice(2);

await bundle(args,
    ...[
        "arrays",
        "boundsComputing",
        "collections",
        "data-table",
        "events",
        "fetcher",
        "gis",
        "hax",
        "inclusion-list",
        "tslib",
        "units"
    ].map(name => `src/${name}/index.ts`)
);
