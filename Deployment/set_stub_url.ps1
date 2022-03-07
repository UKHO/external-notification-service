param (
    [Parameter(Mandatory = $true)] [string] $sandboxUri,
    [Parameter(Mandatory = $true)] [string] $resourceGroup,
    [Parameter(Mandatory = $true)] [string] $webappName
)

Write-Output "Set dev D365 url in appsetting..."
az webapp config appsettings set -g $resourceGroup -n $webappName --settings D365CallbackConfiguration:D365ApiUri=$sandboxUri
az webapp restart --name $webappName --resource-group $resourceGroup
