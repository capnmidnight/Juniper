#!/usr/bin/env node

import * as fs from "fs";
import * as path from "path";

const args = process.argv.splice(2);

if(args.length < 3) {
    console.error("Not enough arguments! Expected <src> <dest> <...types>");
}

const [src, dest, ...types] = args;

function* recurse(...dirs) {
    while (dirs.length > 0) {
        const dir = dirs.shift();
        const subDirs = fs.readdirSync(dir, { withFileTypes: true })
            .filter(e => e.isDirectory()
                && e.name !== "node_modules"
                && e.name !== "bin"
                && e.name !== "obj")
            .map(e => path.join(dir, e.name));
        dirs.push(...subDirs);
        yield dir;
    }
}

await Promise.all(Array.from(recurse(src)).map(async inDir => {
    const files = fs.readdirSync(inDir, { withFileTypes: true })
        .filter(e => e.isFile()
            && types.filter(t => e.name.endsWith(t)).length > 0)
        .map(e => e.name);
    
    if(files.length > 0){
        const outDir = dest + inDir.substring(src.length);
        if(!fs.existsSync(outDir)){
            await fs.promises.mkdir(outDir);
        }
        await Promise.all(files.map(file => {
            const inFile = path.join(inDir, file);
            const outFile = path.join(outDir, file);
            console.log(inFile, "->", outFile);
            return fs.promises.copyFile(inFile, outFile);
        }));
    }
}));