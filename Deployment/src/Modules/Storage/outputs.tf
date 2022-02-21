output name {
  value     = azurerm_storage_account.ens_storage.name
}

output connection_string {
  value     = azurerm_storage_account.ens_storage.primary_connection_string
  sensitive = true
}

output primary_access_key {
  value     = azurerm_storage_account.ens_storage.primary_access_key
  sensitive = true
}

output event_storage_queue {
  value     = azurerm_storage_queue.ens_storage_queue.name
}

output dead_letter_storage_container {
  value     = azurerm_storage_container.ens_storage_container.name
}
