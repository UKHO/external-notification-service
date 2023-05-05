resource "azurerm_storage_account" "ens_storage" {
  name                              = lower("${var.service_name}${var.env_name}storage")
  resource_group_name               = var.resource_group_name
  location                          = var.location
  account_tier                      = "Standard"
  account_replication_type          = "LRS"
  account_kind                      = "StorageV2"
  allow_nested_items_to_be_public   = false
  tags                              = var.tags
  min_tls_version                   = "TLS1_2"
    network_rules {
    default_action             = "Deny"
    ip_rules                   = var.allowed_ips
    bypass                     = ["Logging", "Metrics", "AzureServices"]
    virtual_network_subnet_ids = [var.m_spoke_subnet,var.agent_subnet]
   }
}

resource "azurerm_storage_container" "ens_storage_container" {
  name                      = "dead-letter"
  storage_account_name      = azurerm_storage_account.ens_storage.name
}

resource "azurerm_storage_container" "ens_storage_destination_container" {
  name                      = "dead-letter-destination"
  storage_account_name      = azurerm_storage_account.ens_storage.name
}

resource "azurerm_storage_queue" "ens_storage_queue" {
  name                      = "ens-storage-queue"
  storage_account_name      = azurerm_storage_account.ens_storage.name
}

resource "azurerm_role_assignment" "storage_account_role" {
  scope                     = azurerm_storage_account.ens_storage.id
  role_definition_name      = "Storage Account Contributor"
  principal_id              = var.webapp_principal_id
}

resource "azurerm_role_assignment" "storage_account_role_slot" {
  scope                = azurerm_storage_account.ens_storage.id
  role_definition_name = "Storage Account Contributor"
  principal_id         = var.slot_principal_id
}
