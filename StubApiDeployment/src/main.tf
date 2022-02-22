module "webapp_service" {
  source              = "./Modules/Webapp"
  name                = local.web_app_name
  resource_group_name = var.ens_api_rg
  app_service_sku     = var.app_service_sku[local.env_name]
  app_settings = {
    "ASPNETCORE_ENVIRONMENT"                               = local.env_name
    "WEBSITE_RUN_FROM_PACKAGE"                             = "1"
    "WEBSITE_ENABLE_SYNC_UPDATE_SITE"                      = "true"
    "APPINSIGHTS_INSTRUMENTATIONKEY"                       = "NOT_CONFIGURED"
  }
  tags = local.tags

}
