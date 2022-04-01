param (
    [Parameter(Mandatory = $true)] [string] $d365uri,
    [Parameter(Mandatory = $true)] [string] $resourceGroup,
    [Parameter(Mandatory = $true)] [string] $webappName
)

Write-Output "Set dev D365 url in appsetting..."
az webapp config appsettings set -g $resourceGroup -n $webappName --settings D365CallbackConfiguration:D365ApiUri=$d365uri
az webapp restart --name $webappName --resource-group $resourceGroup
