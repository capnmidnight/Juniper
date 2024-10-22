param (
    [ValidateNotNullOrEmpty()]
    [String] $MigrationName
)

try {
    pushd Example

    dotnet ef migrations add $MigrationName --project ..\Example.Migrations.Sqlite --context CedrusContextInsecure -- --database=local
    dotnet ef migrations add $MigrationName --project ..\Example.Migrations.Postgres --context CedrusContextInsecure -- --database=server
}
finally {
    popd
}