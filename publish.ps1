$rg = 'hellofunctionsdotnetcore-rg'
$loc = 'aue'
$functionApp = "hellofunctionsdotnetcore-$loc"


# Package and zip the Function App
Remove-Item './_functionzip' -Recurse -Force
New-Item './_functionzip' -ItemType Directory
dotnet publish . --configuration Release -o './_functionzip'
Compress-Archive -Path ./_functionzip/* -DestinationPath ./deployfunction.zip -Force

# Deploy source code
az functionapp deployment source config-zip -g $rg -n $functionApp --src ./deployfunction.zip

start "https://hellofunctionsdotnetcore-$loc.azurewebsites.net/api/GetHealth"

# Log tail
az webapp log tail -n $functionapp -g $rg
