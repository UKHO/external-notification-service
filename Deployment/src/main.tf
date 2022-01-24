module "user_identity" {
  source              = "./Modules/UserIdentity"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  env_name            = local.env_name
  tags                = local.tags
}

module "app_insights" {
  source              = "./Modules/AppInsights"
  name                = "${local.service_name}-${local.env_name}-insights"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  tags                = local.tags
}

module "eventhub" {
  source              = "./Modules/EventHub"
  name                = "${local.service_name}-${local.env_name}-events"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  logstashStorageName = lower("${local.service_name}logstash${local.env_name}")
  tags                = local.tags
}

module "webapp_service" {
  source                    = "./Modules/Webapp"
  name                      = local.web_app_name
  resource_group_name       = azurerm_resource_group.rg.name
  location                  = azurerm_resource_group.rg.location
  user_assigned_identity    = module.user_identity.ens_service_identity_id
  app_service_sku           = var.app_service_sku[local.env_name]
  app_settings = {
    "EventHubLoggingConfiguration:Environment"             = local.env_name
    "EventHubLoggingConfiguration:MinimumLoggingLevel"     = "Warning"
    "EventHubLoggingConfiguration:UkhoMinimumLoggingLevel" = "Information"
    "APPINSIGHTS_INSTRUMENTATIONKEY"                       = module.app_insights.instrumentation_key
    "ASPNETCORE_ENVIRONMENT"                               = local.env_name
    "WEBSITE_RUN_FROM_PACKAGE"                             = "1"
    "WEBSITE_ENABLE_SYNC_UPDATE_SITE"                      = "true"
  }
  tags                      = local.tags
}

module "key_vault" {
  source              = "./Modules/KeyVault"
  name                = local.key_vault_name
  resource_group_name = azurerm_resource_group.rg.name
  env_name            = local.env_name
  tenant_id           = module.user_identity.ess_service_identity_tenant_id
  location            = azurerm_resource_group.rg.location
  read_access_objects = {
    "ess_service_identity" = module.user_identity.ess_service_identity_principal_id
  }
  secrets = merge(
      {
        "EventHubLoggingConfiguration--ConnectionString"            = module.eventhub.log_primary_connection_string
        "EventHubLoggingConfiguration--EntityPath"                  = module.eventhub.entity_path
        "ESSFulfilmentConfiguration--StorageAccountName"            = module.fulfilment_storage.small_exchange_set_name
        "ESSFulfilmentConfiguration--StorageAccountKey"             = module.fulfilment_storage.small_exchange_set_primary_access_key
        "ESSFulfilmentConfiguration--SmallExchangeSetAccountName"   = module.fulfilment_storage.small_exchange_set_name
        "ESSFulfilmentConfiguration--SmallExchangeSetAccountKey"    = module.fulfilment_storage.small_exchange_set_primary_access_key
        "ESSFulfilmentConfiguration--MediumExchangeSetAccountName"  = module.fulfilment_storage.medium_exchange_set_name
        "ESSFulfilmentConfiguration--MediumExchangeSetAccountKey"   = module.fulfilment_storage.medium_exchange_set_primary_access_key
        "ESSFulfilmentConfiguration--LargeExchangeSetAccountName"   = module.fulfilment_storage.large_exchange_set_name
        "ESSFulfilmentConfiguration--LargeExchangeSetAccountKey"    = module.fulfilment_storage.large_exchange_set_primary_access_key
      },
      module.fulfilment_webapp.small_exchange_set_scm_credentials,
      module.fulfilment_webapp.medium_exchange_set_scm_credentials,
      module.fulfilment_webapp.large_exchange_set_scm_credentials
  )
  tags                                         = local.tags
}
