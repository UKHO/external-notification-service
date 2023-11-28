param (
    [Parameter(Mandatory = $true)] [string] $deploymentResourceGroupName,
    [Parameter(Mandatory = $true)] [string] $deploymentStorageAccountName,
    [Parameter(Mandatory = $true)] [string] $workSpace,
    [Parameter(Mandatory = $true)] [boolean] $continueEvenIfResourcesAreGettingDestroyed,
    [Parameter(Mandatory = $true)] [string] $terraformJsonOutputFile,
    [Parameter(Mandatory = $true)] [string] $elasticApmServerUrl,
    [Parameter(Mandatory = $true)] [string] $elasticApmApiKey,
    [Parameter(Mandatory = $true)] [string] $elasticApmEnvironment,
)

cd $env:AGENT_BUILDDIRECTORY/terraformartifact/src

terraform --version

Write-output "Executing terraform scripts for deployment in $workSpace enviroment"
terraform init -backend-config="resource_group_name=$deploymentResourceGroupName" -backend-config="storage_account_name=$deploymentStorageAccountName" -backend-config="key=terraform.deployment.tfplan"
if ( !$? ) { echo "Something went wrong during terraform initialization"; throw "Error" }

Write-output "Selecting workspace"

$ErrorActionPreference = 'SilentlyContinue'
terraform workspace new $WorkSpace 2>&1 > $null
$ErrorActionPreference = 'Continue'

terraform workspace select $workSpace
if ( !$? ) { echo "Error while selecting workspace"; throw "Error" }

Write-output "Validating terraform"
terraform validate
if ( !$? ) { echo "Something went wrong during terraform validation" ; throw "Error" }

Write-output "Execute Terraform plan"
terraform plan -out "terraform.deployment.tfplan" -var elastic_apm_server_url=$elasticApmServerUrl -var elastic_apm_api_key=$elasticApmApiKey -var elastic_apm_environment=$elasticApmEnvironment | tee terraform_output.txt
if ( !$? ) { echo "Something went wrong during terraform plan" ; throw "Error" }

$totalDestroyLines=(Get-Content -Path terraform_output.txt | Select-String -Pattern "destroy" -CaseSensitive |  where {$_ -ne ""}).length
if($totalDestroyLines -ge 2) 
{
    write-Host("Terraform is destroying some resources, please verify...................")
    if ( !$ContinueEvenIfResourcesAreGettingDestroyed) 
    {
        write-Host("exiting...................")
        Write-Output $_
        exit 1
    }
    write-host("Continue executing terraform apply - as continueEvenIfResourcesAreGettingDestroyed param is set to true in pipeline")
}

Write-output "Executing terraform apply"
terraform apply  "terraform.deployment.tfplan"
if ( !$? ) { echo "Something went wrong during terraform apply" ; throw "Error" }

Write-output "Terraform output as json"
$terraformOutput = terraform output -json | ConvertFrom-Json

write-output "Set JSON output into pipeline variables"
Write-Host "##vso[task.setvariable variable=WEB_APP_NAME]$($terraformOutput.web_app_name.value)"
Write-Host "##vso[task.setvariable variable=WEB_APP;isOutput=true]$($terraformOutput.web_app_name.value)"
Write-Host "##vso[task.setvariable variable=EnsApiUrl]$env:SERVICE_DNS_URL"
Write-Host "##vso[task.setvariable variable=SubscriptionStorageConfiguration.QueueName]$($terraformOutput.event_storage_queue.value)"
Write-Host "##vso[task.setvariable variable=EventHubLoggingConfiguration.Environment]$($terraformOutput.env_name.value)"
Write-Host "##vso[task.setvariable variable=EnsStorageConnectionString;issecret=true]$($terraformOutput.ens_storage_connection_string.value)"
Write-Host "##vso[task.setvariable variable=EnsStorageQueueName]$($terraformOutput.event_storage_queue.value)"
Write-Host "##vso[task.setvariable variable=SubscriptionStorageConfiguration.StorageContainerName]$($terraformOutput.dead_letter_storage_container.value)"
Write-Host "##vso[task.setvariable variable=stubWebAppName]$($terraformOutput.stub_webappname.value)"
Write-Host "##vso[task.setvariable variable=D365ApiStubUri]$($terraformOutput.ens_stub_web_app_url.value)"
Write-Host "##vso[task.setvariable variable=ResourceGroup]$($terraformOutput.webapp_rg.value)"
Write-Host "##vso[task.setvariable variable=ResourceGroupName;isOutput=true]$($terraformOutput.webapp_rg.value)"
Write-Host "##vso[task.setvariable variable=EventGridDomainConfiguration.EventGridDomainEndpoint;issecret=true]$($terraformOutput.event_grid_domain_endpoint_url.value)"
Write-Host "##vso[task.setvariable variable=SubscriptionStorageConfiguration.DeadLetterDestinationContainerName]$($terraformOutput.dead_letter_storage_destination_container.value)"
Write-Host "##vso[task.setvariable variable=WEB_APP_SLOT_NAME]$($terraformOutput.web_app_slot_name.value)"
Write-Host "##vso[task.setvariable variable=WEB_APP_SLOT_HOST_NAME]$($terraformOutput.web_app_slot_default_site_hostname.value)"

$terraformOutput | ConvertTo-Json -Depth 5 > $terraformJsonOutputFile
