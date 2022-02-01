output "event_grid_domain_id" {
  value     = azurerm_eventgrid_domain.eventgrid_domain.id
}

output "event_grid_domain_name" {
  value     = azurerm_eventgrid_domain.eventgrid_domain.name
}

output "event_grid_domain_endpoint" {
  value     = azurerm_eventgrid_domain.eventgrid_domain.endpoint
  sensitive = true
}

output "event_grid_domain_primary_access_key" {
  value     = azurerm_eventgrid_domain.eventgrid_domain.primary_access_key
  sensitive = true
}
