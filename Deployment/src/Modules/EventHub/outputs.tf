output "log_primary_connection_string" {
  value     = azurerm_eventhub_authorization_rule.log.primary_connection_string
  sensitive = true
}

output "logstash_primary_connection_string" {
  value     = azurerm_eventhub_authorization_rule.logstash.primary_connection_string
  sensitive = true
}

output "entity_path" {
  value = azurerm_eventhub.eventhub.name
}

output "eventgrid_authorization_id" {
  value = azurerm_eventhub_authorization_rule.eventgrid.id
}
