& cls
& terraform init -backend-config="resource_group_name=apim-learn-rg" -backend-config="storage_account_name=tpsstoragevk" -backend-config="key=ensapim.deployment.tfplan"
& terraform workspace select dev 
& terraform validate
& terraform plan -out "ensapim.deployment.tfplan"
#& terraform apply ensapim.deployment.tfplan