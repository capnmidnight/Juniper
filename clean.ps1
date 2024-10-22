Get-ChildItem -recurse `
    -include tsconfig.tsbuildinfo,node_modules,dist,package-lock.json `
| ForEach-Object { $_ | Remove-Item -Recurse -Force }