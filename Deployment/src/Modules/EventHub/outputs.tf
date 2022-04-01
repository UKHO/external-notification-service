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

output "eventhub_namespace_authorization_rule_id" {
  value = azurerm_eventhub_namespace_authorization_rule.namespace_rule.id
}

output "eventgrid_eventhub_name" {
  value = azurerm_eventhub.eventhub_logging.name
}
