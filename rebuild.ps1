try {
    pushd $PSScriptRoot

    ./clean.ps1

    npm i --no-fund --prefer-offline
    npm run build
}
finally {
    popd
}