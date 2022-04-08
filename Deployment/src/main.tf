data "azurerm_subnet" "main_subnet" {
  name                 = var.spoke_subnet_name
  virtual_network_name = var.spoke_vnet_name
  resource_group_name  = var.spoke_rg
}

data "azurerm_subnet" "private_endpoint_subnet" {
  name                 = var.private_endpoint_subnet_name
  virtual_network_name = var.spoke_vnet_name
  resource_group_name  = var.spoke_rg
}

data "azurerm_subnet" "agent_subnet" {
  provider             = azurerm.build_agent
  name                 = var.agent_subnet_name
  virtual_network_name = var.agent_vnet_name
  resource_group_name  = var.agent_rg
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
  tags                = local.tags
}

module "webapp_service" {
  source                    = "./Modules/Webapp"
  name                      = local.web_app_name
  resource_group_name       = azurerm_resource_group.rg.name
  subnet_id                 = data.azurerm_subnet.main_subnet.id
  agent_id                  = data.azurerm_subnet.agent_subnet.id
  env_name                  = local.env_name
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
    "EventGridDomainConfiguration:ResourceGroup"               = azurerm_resource_group.rg.name
    "EventGridDomainConfiguration:EventGridDomainName"         = module.eventgriddomain.event_grid_domain_name
  }
  app_settings_stub = {
    "ASPNETCORE_ENVIRONMENT"                               = local.env_name
    "WEBSITE_RUN_FROM_PACKAGE"                             = "1"
    "WEBSITE_ENABLE_SYNC_UPDATE_SITE"                      = "true"
    "APPINSIGHTS_INSTRUMENTATIONKEY"                       = "NOT_CONFIGURED"
  }
  tags                      = local.tags
  allowed_ips               = var.allowed_ips
}

module "key_vault" {
  source              = "./Modules/KeyVault"
  name                = local.key_vault_name
  resource_group_name = azurerm_resource_group.rg.name
  env_name            = local.env_name
  tenant_id           = module.webapp_service.web_app_tenant_id
  allowed_ips         = var.allowed_ips
  m_spoke_subnet      = data.azurerm_subnet.main_subnet.id
  agent_subnet        = data.azurerm_subnet.agent_subnet.id
  location            = azurerm_resource_group.rg.location
  read_access_objects = {
     "webapp_service" = module.webapp_service.web_app_object_id
  }
  secrets = merge(
        module.webapp_service.webapp_scm_credentials,
       {
        "EventHubLoggingConfiguration--ConnectionString"            = module.eventhub.log_primary_connection_string
        "EventHubLoggingConfiguration--EntityPath"                  = module.eventhub.entity_path
        "SubscriptionStorageConfiguration--StorageAccountName"      = module.storage.name
        "SubscriptionStorageConfiguration--StorageAccountKey"       = module.storage.primary_access_key
        "SubscriptionStorageConfiguration--StorageConnectionString" = module.storage.connection_string
        "AzureWebJobsStorage"                                       = module.storage.connection_string
        "EventGridDomainConfiguration--EventGridDomainAccessKey"    = module.eventgriddomain.event_grid_domain_primary_access_key
      })
  tags                = local.tags
}

module "eventgriddomain" {
  source              = "./Modules/EventGridDomain"
  name                = "${local.service_name}-${local.env_name}-eventgriddomain"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  tags                = local.tags
  webapp_principal_id = module.webapp_service.web_app_object_id
  storage_account_id  = module.storage.id
}

module "storage" {
  source              = "./Modules/Storage"
  resource_group_name = azurerm_resource_group.rg.name
  location            = var.location
  allowed_ips         = var.allowed_ips
  m_spoke_subnet      = data.azurerm_subnet.main_subnet.id
  agent_subnet        = data.azurerm_subnet.agent_subnet.id
  tags                = local.tags
  webapp_principal_id = module.webapp_service.web_app_object_id
  env_name            = local.env_name
  service_name        = local.service_name
}

module "azure-dashboard" {
  source         = "./Modules/AzureDashboard"
  name           = "ENS-${local.env_name}-monitoring-dashboard"
  location       = azurerm_resource_group.rg.location
  environment    = local.env_name
  resource_group = azurerm_resource_group.rg
  tags           = local.tags
}

module "EventGridDomainPrivateEndpoint " {
  source              = "./Modules/EventGridDomainPrivateEndpoint "
  resource_group_name = azurerm_resource_group.rg.name
  subnet_id           = data.azurerm_subnet.private_endpoint_subnet.id
  location            = var.location
  tags                = local.tags
  env_name            = local.env_name
  event_grid_domain_resource_id = module.eventgriddomain.event_grid_domain_resource_id
}
