try {
    pushd $PSScriptRoot
    
    npm i --no-fund --prefer-offline
    npm run force-build

    try{ 
        pushd JS/esbuild
        
        if(Test-Path .\tsconfig.tsbuildinfo) {
            rm .\tsconfig.tsbuildinfo
        }

        npm i --no-fund --prefer-offline
        npm run build
    }
    finally { 
        popd
    }
}
finally {
    popd
}