output "web_app_name" {
  value = local.web_app_name
}

output "event_storage_queue" {
  value = module.storage.event_storage_queue
}

output "env_name" {
  value = local.env_name
}

output "ens_storage_connection_string" {
  value = module.storage.connection_string
  sensitive = true
}

output "resource_group_name" {
  value = azurerm_resource_group.rg.name
}

output "event_grid_doamin_name" {
  value = module.eventgriddomain.event_grid_domain_name
}
