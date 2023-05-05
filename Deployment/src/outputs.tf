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

output "event_grid_doamin_name" {
  value = module.eventgriddomain.event_grid_domain_name
}

output "webapp_rg" {
  value = azurerm_resource_group.rg.name
}


output "dead_letter_storage_container" {
  value = module.storage.dead_letter_storage_container
}

output "dead_letter_storage_destination_container" {
  value = module.storage.dead_letter_storage_destination_container
}

output "stub_webappname" {
  value = lower(terraform.workspace) == "live" ? null : module.webapp_service.webapp_name
}

output "ens_stub_web_app_url" {
  value = lower(terraform.workspace) == "live" ? null : "https://${module.webapp_service.default_site_hostname_ens_stub}/api/dynamics/"
}

output "event_grid_domain_endpoint_url" {
  value     = module.eventgriddomain.event_grid_domain_endpoint
  sensitive = true
}

output "web_app_slot_name" {
  value = module.webapp_service.slot_name
}

output "web_app_slot_default_site_hostname" {
  value = module.webapp_service.slot_default_site_hostname
}
