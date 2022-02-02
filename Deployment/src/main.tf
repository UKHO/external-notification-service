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
  app_service_sku           = var.app_service_sku[local.env_name]
  app_settings = {
    "KeyVaultSettings:ServiceUri"                              = "https://${local.key_vault_name}.vault.azure.net/"
    "EventHubLoggingConfiguration:Environment"                 = local.env_name
    "EventHubLoggingConfiguration:MinimumLoggingLevel"         = "Warning"
    "EventHubLoggingConfiguration:UkhoMinimumLoggingLevel"     = "Information"
    "APPINSIGHTS_INSTRUMENTATIONKEY"                           = module.app_insights.instrumentation_key
    "ASPNETCORE_ENVIRONMENT"                                   = local.env_name
    "WEBSITE_RUN_FROM_PACKAGE"                                 = "1"
    "WEBSITE_ENABLE_SYNC_UPDATE_SITE"                          = "true"
    "EnsEventGridDomainConfiguration:ResourceGroup"            = azurerm_resource_group.rg.name
    "EnsEventGridDomainConfiguration:EventGridDomainName"      = module.eventgriddomain.event_grid_domain_name
    "EnsEventGridDomainConfiguration:EventGridDomainEndpoint"  = module.eventgriddomain.event_grid_domain_endpoint
  }
  tags                      = local.tags
}

module "key_vault" {
  source              = "./Modules/KeyVault"
  name                = local.key_vault_name
  resource_group_name = azurerm_resource_group.rg.name
  env_name            = local.env_name
  tenant_id           = module.webapp_service.web_app_tenant_id
  location            = azurerm_resource_group.rg.location
  read_access_objects = {
     "webapp_service" = module.webapp_service.web_app_object_id
  }
  secrets = {
        "EventHubLoggingConfiguration--ConnectionString"               = module.eventhub.log_primary_connection_string
        "EventHubLoggingConfiguration--EntityPath"                     = module.eventhub.entity_path
        "EnsSubscriptionStorageConfiguration--StorageAccountName"      = module.storage.name
        "EnsSubscriptionStorageConfiguration--StorageAccountKey"       = module.storage.primary_access_key
        "EnsSubscriptionStorageConfiguration--StorageConnectionString" = module.storage.connection_string
        "EnsSubscriptionStorageConfiguration--QueueName"               = module.storage.event_storage_queue
        "EnsEventGridDomainConfiguration--EventGridDomainAccessKey"    = module.eventgriddomain.event_grid_domain_primary_access_key
      }
  tags                = local.tags
}

module "eventgriddomain" {
  source              = "./Modules/EventGridDomain"
  name                = "${local.service_name}-${local.env_name}-eventgriddomain"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  tags                = local.tags
  webapp_principal_id = module.webapp_service.web_app_object_id
}

module "storage" {
  source              = "./Modules/Storage"
  name                = "${local.service_name}${local.env_name}storageukho"
  resource_group_name = azurerm_resource_group.rg.name
  location            = var.location
  tags                = local.tags
  webapp_principal_id = module.webapp_service.web_app_object_id
}
