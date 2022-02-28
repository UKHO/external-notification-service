param (
    [Parameter(Mandatory = $true)] [string] $stubUri,
    [Parameter(Mandatory = $true)] [string] $resourceGroup,
    [Parameter(Mandatory = $true)] [string] $webappName
)
Write-Output "List ens webapp appsettings..."
az webapp config appsettings list -g $resourceGroup -n $webappName
Write-Output "Set dev D365 url in appsetting..."
az webapp config appsettings set -g $resourceGroup -n $webappName --settings D365CallbackConfiguration:D365ApiUri=$stubUri
az webapp restart --name $webappName --resource-group $resourceGroup
Write-Output "List ens webapp appsettings after pointing to Dev environment..."
az webapp config appsettings list -g $resourceGroup -n $webappName
