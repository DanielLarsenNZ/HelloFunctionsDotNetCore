# Hello Functions .NET Core

A simple Hello World Azure Function written in .Net Core with an HTTP Trigger Binding and a couple of
features for testing.

> This Function is usually running here: <https://hellofunctionsdotnetcore-aue.azurewebsites.net/api/GetHealth>

## Getting started

You will need:

* Azure Subscription
* PowerShell
* Azure CLI (logged in)

```powershell
git clone https://github.com/DanielLarsenNZ/HelloFunctionsDotNetCore.git
cd HelloFunctionsDotNetCore

# Alter the hardcoded variables in these files for your own use.
./deploy.ps1
./publish.ps1
```

## App Settings

`GetUrls` - A semicolon delimited list of URLs to GET using `HttpClient`. Use this to test if connectivity
to the public internet (or private HTTP endpoints) is enabled. `deploy.ps1` will set the list of sites
to test to:

* https://www.microsoft.com/
* https://www.google.com/
* https://www.dropbox.com/

When these settings are present, the text content of Blob/s will be returned in the response. Use these
settings to test if connectivity to Storage accounts is present. `deploy.ps1` will create two storage
accounts with test containers and files and configure these settings accordingly.

`Blob1.StorageConnectionString` - Storage Account 1 Connection String
`Blob2.StorageConnectionString` - Storage Account 2 Connection String
`Blob.Path` - Path to a Blob to return the contents of.

> For local development, copy `local.settings.template.json` to `local.settings.json` and set your
> own values.
