resource "azurerm_storage_account" "ens_storage" {
  name = lower("${var.service_name}${var.env_name}storageukho")
  resource_group_name = var.resource_group_name
  location = var.location
  account_tier = "Standard"
  account_replication_type = "LRS"
  account_kind = "StorageV2"
  allow_blob_public_access  = false
  tags = var.tags
}

resource "azurerm_storage_queue" "ens_storage_queue" {
  name                 = "ens-storage-queue"
  storage_account_name = azurerm_storage_account.ens_storage.name
}

resource "azurerm_role_assignment" "storage_account_role" {
  scope                = azurerm_storage_account.ens_storage.id
  role_definition_name = "Storage Account Contributor"
  principal_id         = var.webapp_principal_id
}
