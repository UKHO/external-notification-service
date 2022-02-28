& cls
& terraform init -backend-config="resource_group_name=apim-rg" -backend-config="storage_account_name=vjapimterraformstorage" -backend-config="key=ensapim.deployment.tfplan"
& terraform workspace select dev
& terraform validate
& terraform plan -var 'resource_group_name=apim-rg' -var 'apim_service_name=vjapim' -var 'apim_api_service_url=https://vk-ens.azurewebsites.net/' -var 'ad_tenant_id=9134ca48-663d-4a05-968a-31a42f0aed3e' -var 'client_credentials_client_id=80513673-79f7-45e2-8488-2d31bcc774aa' -var 'client_credentials_secret=J027Q~GRGF3tqVZK.gcvnU40AdHR29IvQoExh' -var 'client_credentials_scope=6eb29be7-d5f5-4cab-b48f-96393e7b0695/.default' -var 'ens_app_client_id=6eb29be7-d5f5-4cab-b48f-96393e7b0695' -out "ensapim.deployment.tfplan"
#& terraform apply ensapim.deployment.tfplan