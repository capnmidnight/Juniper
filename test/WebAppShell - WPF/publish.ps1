if($args.Length -ne 1 -or $args[0] -notmatch "^(Debug|Release)$") {
    echo "Expected usage: publish [Debug|Release]"
}
else {
    $config = $args[0]

    dotnet publish `
        --sc `
        --nologo `
        --framework net7.0-windows `
        --runtime win-x64 `
        --configuration $config
}