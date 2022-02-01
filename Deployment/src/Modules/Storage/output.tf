output name {
  value = azurerm_storage_account.storage.name
}

output connection_string {
  value     = azurerm_storage_account.storage.primary_connection_string
  sensitive = true
}

output primary_access_key {
  value     = azurerm_storage_account.storage.primary_access_key
  sensitive = true
}

output events_storage_queue {
  value = azurerm_storage_queue.events_storage_queue
}

output storage_account_id {
  value = azurerm_storage_account.storage.id
}
