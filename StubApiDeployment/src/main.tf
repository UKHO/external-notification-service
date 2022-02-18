module "webapp_service" {
  source              = "./Modules/Webapp"
  service_name        = local.service_name
  env_name            = local.env_name
  resource_group_name = azurerm_resource_group.webapp_rg.name
  location            = azurerm_resource_group.webapp_rg.location
  app_service_sku     = var.app_service_sku[local.env_name]
  user_assigned_identity    = module.user_identity.service_identity_id
  app_settings = {
    "ASPNETCORE_ENVIRONMENT"                               = local.env_name
    "WEBSITE_RUN_FROM_PACKAGE"                             = "1"
    "WEBSITE_ENABLE_SYNC_UPDATE_SITE"                      = "true"
    "APPINSIGHTS_INSTRUMENTATIONKEY"                       = "NOT_CONFIGURED"
  }
  tags = local.tags

}
