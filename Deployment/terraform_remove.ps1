param (
    [Parameter(Mandatory = $true)] [string] $deploymentResourceGroupName,
    [Parameter(Mandatory = $true)] [string] $deploymentStorageAccountName,
    [Parameter(Mandatory = $true)] [string] $workSpace,
    [Parameter(Mandatory = $true)] [boolean] $continueEvenIfResourcesAreGettingDestroyed,
    [Parameter(Mandatory = $true)] [string] $terraformJsonOutputFile,
    [Parameter(Mandatory = $true)] [string] $elasticApmServerUrl,
    [Parameter(Mandatory = $true)] [string] $elasticApmApiKey,
    [Parameter(Mandatory = $true)] [string] $elasticApmEnvironment,
    [Parameter(Mandatory = $true)] [string] $elasticApmWebJobServiceName
)

cd $env:AGENT_BUILDDIRECTORY/terraformartifact/src

terraform --version

Write-Output "Executing terraform scripts for deployment in $workSpace enviroment"
terraform init -backend-config="resource_group_name=$deploymentResourceGroupName" -backend-config="storage_account_name=$deploymentStorageAccountName" -backend-config="key=terraform.deployment.tfplan"
if ( !$? ) { echo "Something went wrong during terraform initialization"; throw "Error" }

Write-Output "Selecting workspace"

$ErrorActionPreference = 'SilentlyContinue'
terraform workspace new $workSpace 2>&1 > $null
$ErrorActionPreference = 'Continue'

terraform workspace select $workSpace
if ( !$? ) { echo "Error while selecting workspace"; throw "Error" }

Write-Output "Checking if old style resources are present..."

$webApp1=terraform state list | Select-String "module.webapp_service.azurerm_app_service.webapp_service"
$newWebApp1='module.webapp_service.azurerm_windows_web_app.webapp_service'
$webApp2=terraform state list | Select-String "module.webapp_service.azurerm_app_service_slot.staging"
$newWebApp2='module.webapp_service.azurerm_windows_web_app_slot.staging'
$webApp3=terraform state list | Select-String "module.webapp_service.azurerm_app_service.stub_webapp_service"
$newWebApp3='module.webapp_service.azurerm_windows_web_app.stub_webapp_service'

Write-Output "Output WebApp1: $webApp1"
Write-Output "Output WebApp2: $webApp2"
Write-Output "Output WebApp3: $webApp3"

if (![string]::IsNullOrWhiteSpace($webApp1))
{
    Write-Output "$webApp1 present in state file - removing it..."
    terraform state rm "module.webapp_service.azurerm_app_service.webapp_service"
    Write-Output "$webApp1 removal from state file done..."
    Write-Output "$newWebApp1 importing to state file..."
    terraform import -var elastic_apm_server_url=$elasticApmServerUrl -var elastic_apm_api_key=$elasticApmApiKey "$newWebApp1" "/subscriptions/db328b32-0563-44a5-8f51-689c5cae8a7e/resourceGroups/ens-dev-rg/providers/Microsoft.Web/serverfarms/ens-dev-webapp-asp"
    if ( !$? ) { echo "Something went wrong during terraform import"; throw "Error" }
    Write-Output "$newWebApp1 import done..."
}
else
{
    Write-Output "$webApp1 not present in state file..."
}

if (![string]::IsNullOrWhiteSpace($webApp2))
{
    Write-Output "$webApp2 present in state file - removing it..."
    terraform state rm "module.webapp_service.azurerm_app_service_slot.staging"
    Write-Output "$webApp2 removal from state file done..."
    Write-Output "$newWebApp2 importing to state file..."
    terraform import -var elastic_apm_server_url=$elasticApmServerUrl -var elastic_apm_api_key=$elasticApmApiKey "$newWebApp2" "/subscriptions/db328b32-0563-44a5-8f51-689c5cae8a7e/resourceGroups/ens-dev-rg/providers/Microsoft.Web/serverfarms/ens-dev-webapp-asp"
    if ( !$? ) { echo "Something went wrong during terraform import"; throw "Error" }
    Write-Output "$newWebApp2 import done..."
}
else
{
    Write-Output "$webApp2 not present in state file..."
}

if (![string]::IsNullOrWhiteSpace($webApp3))
{
    Write-Output "$webApp3 present in state file - removing it..."
    terraform state rm "module.webapp_service.azurerm_app_service.stub_webapp_service"
    Write-Output "$webApp3 removal from state file done..."
    Write-Output "$newWebApp3 importing to state file..."
    terraform import -var elastic_apm_server_url=$elasticApmServerUrl -var elastic_apm_api_key=$elasticApmApiKey "$newWebApp3" "/subscriptions/db328b32-0563-44a5-8f51-689c5cae8a7e/resourceGroups/ens-dev-rg/providers/Microsoft.Web/serverfarms/ens-dev-webapp-asp"
    if ( !$? ) { echo "Something went wrong during terraform import"; throw "Error" }
    Write-Output "$newWebApp3 import done..."
}
else
{
    Write-Output "$webApp3 not present in state file..."
}
