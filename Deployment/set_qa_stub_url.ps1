param (
    [Parameter(Mandatory = $true)] [string] $d365qauri,
    [Parameter(Mandatory = $true)] [string] $resourceGroup,
    [Parameter(Mandatory = $true)] [string] $webappName
)

Write-Output "Set dev D365 url in appsetting..."
az webapp config appsettings set -g $resourceGroup -n $webappName --settings D365CallbackConfiguration:D365ApiUri=$d365qauri
az webapp restart --name $webappName --resource-group $resourceGroup
