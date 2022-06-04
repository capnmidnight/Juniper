$apikey = get-content ~/source/repos/capnmidnight/nuget.juniper.2022.apikey
dotnet nuget push bin\Release\SeanMcBeth.Juniper.1.0.0.nupkg --api-key $apikey --source https://api.nuget.org/v3/index.json