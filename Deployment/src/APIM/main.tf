module "APIM" {
  source					  = "./Modules/ExternalNotifcationService"
  env						  = local.env_name  
  env_suffix                  = var.env_suffix_text[local.env_name]
  api_openapi_spec_path       = file("${path.module}/external-notification-service.openApi.yaml")
}
