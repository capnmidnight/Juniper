$projectName = "NTAN"
$version = "None"
$config = "Release"

if($args.Length -eq 1) {
    if($args[0] -match "^(Debug|Test|Release)$") {
        $config = $args[0]
    }
    elseif($args[0] -match "^(Major|Minor|Patch)$") {
        $version = $args[0]
    }
}
elseif($args.Length -eq 2) {
    if($args[0] -match "^(Major|Minor|Patch)$" -and $args[1] -match "^(Debug|Test|Release)$") {
        $version = $args[0]
        $config = $args[1]
    }
    elseif($args[0] -match "^(Debug|Test|Release)$" -and $args[1] -match "^(Major|Minor|Patch)$") {
        $config = $args[0]
        $version = $args[1]
    }
}

$buildProj = ".\$projectName\$projectName.csproj"
$publishdir = ".\pack\$config"
$archivedir = ".\archive\$config"
$archivefile = "$archivedir\$projectName.zip"

if(-not (Test-Path $publishdir)) {
    mkdir $publishdir
}

if(-not (Test-Path $archivedir)) {
    mkdir $archivedir
}

if((Test-Path $archivefile)) {
    del $archivefile
}

Write-Output "Building in $config mode with version bump $version"

if($version -ne "None") {
    cd $projectName
    npm version $version.ToLower()
    cd ..
}

# make sure the JavaScript is up to date.
dotnet run `
    --project $buildProj `
    --configuration Debug `
    -- --build

# check Properties/PublishProfiles/FolderProfile.pubxml for publish settings
dotnet publish `
    --nologo `
    -p:PublishProfile=FolderProfile `
    --configuration $config `
    $buildProj
    
dotnet publish `
    --nologo `
    -p:PublishProfile=FolderProfile `
    --configuration $config `
    .\Starter\Starter.csproj

Write-Output "Compressing $publishdir to $archivedir"

Compress-Archive `
    -Path "$publishdir\*" `
    -CompressionLevel "Optimal" `
    -DestinationPath $archivefile

# open the output directory
explorer $archivedir