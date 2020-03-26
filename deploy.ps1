$rg = 'hellofunctionsdotnetcore-rg'
$location = 'australiaeast'
$loc = 'aue'
$tags = "expires=$([DateTime]::UtcNow.AddDays(7))", "owner=dalars", "project=DanielLarsenNZ/HelloFunctionsDotNetCore"
$webjobsStorage = "hellofunctions$loc"
$dataStorage = "hellofunctionsdata$loc"
$functionApp = "hellofunctionsdotnetcore-$loc"
$insights = 'hellofunctionsdotnetcore-insights'
$container = 'hellofunctionsdotnetcore'
$testFile = 'test.txt'


# RESOURCE GROUP
az group create -n $rg --location $location --tags $tags

# STORAGE ACCOUNTS
# https://docs.microsoft.com/en-us/cli/azure/storage/account?view=azure-cli-latest#az-storage-account-create
az storage account create -n $webjobsStorage -g $rg -l $location --sku Standard_LRS
$webjobsStorageConnection = ( az storage account show-connection-string -g $rg -n $webjobsStorage | ConvertFrom-Json ).connectionString
az storage container create -n $container --account-name $webjobsStorage --public-access off --connection-string $webjobsStorageConnection
az storage blob upload --account-name $webjobsStorage -f $testFile -c $container -n $testFile --connection-string $webjobsStorageConnection

az storage account create -n $dataStorage -g $rg -l $location --sku Standard_LRS
$dataStorageConnection = ( az storage account show-connection-string -g $rg -n $dataStorage | ConvertFrom-Json ).connectionString
az storage container create -n $container --account-name $dataStorage --public-access off --connection-string $dataStorageConnection
az storage blob upload --account-name $datastorage -f $testFile -c $container -n $testFile --connection-string $dataStorageConnection


# APPLICATION INSIGHTS
#  https://docs.microsoft.com/en-us/cli/azure/ext/application-insights/monitor/app-insights/component?view=azure-cli-latest
az extension add -n application-insights
$instrumentationKey = ( az monitor app-insights component create --app $insights --location $location -g $rg --tags $tags | ConvertFrom-Json ).instrumentationKey


# FUNCTION APP
az functionapp create -n $functionApp -g $rg --consumption-plan-location $location -s $webjobsStorage --functions-version 3 --app-insights $insights --app-insights-key $instrumentationKey
az functionapp config appsettings set -n $functionApp -g $rg --settings `
    "APPINSIGHTS_INSTRUMENTATIONKEY=$instrumentationKey" `
    "AzureWebJobsStorage=$webjobsStorageConnection" `
    "Blob1.StorageConnectionString=$webjobsStorageConnection" `
    "Blob2.StorageConnectionString=$dataStorageConnection" `
    "GetUrls=https://www.microsoft.com/;https://www.google.com/;https://www.dropbox.com/" `
    "Blob.Path=$container/$testFile"


# TEAR DOWN
#az group delete -n $rg --yes