module "APIM" {
  source					      = "./Modules/ExternalNotificationService"
  env						      = local.env_name  
  env_suffix                      = var.env_suffix_text[local.env_name]
  api_openapi_spec_path           = file("${path.module}/external-notification-service.openApi.yaml")

  resource_group_name             = var.resource_group_name
  apim_service_name               = var.apim_service_name
  apim_api_service_url            = var.apim_api_service_url
  d365_product_daily_quota_limit  = var.d365_product_daily_quota_limit
  ad_tenant_id                    = var.ad_tenant_id
  client_credentials_client_id    = var.client_credentials_client_id
  client_credentials_secret       = var.client_credentials_secret
  client_credentials_scope        = var.client_credentials_scope
  ens_app_client_id               = var.ens_app_client_id
}
