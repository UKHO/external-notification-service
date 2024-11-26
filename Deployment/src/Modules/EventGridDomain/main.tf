resource "azurerm_eventgrid_domain" "eventgrid_domain" {
  name                          = var.name
  location                      = var.location
  resource_group_name           = var.resource_group_name
  tags                          = var.tags
  input_schema                  = "CloudEventSchemaV1_0"
  public_network_access_enabled = false
}

resource "azurerm_role_assignment" "eventgrid_domain_role" {
  scope                = azurerm_eventgrid_domain.eventgrid_domain.id
  role_definition_name = "EventGrid Contributor"
  principal_id         = var.webapp_principal_id
}

resource "azurerm_monitor_diagnostic_setting" "egdiagnosticsetting" {
  name                           = "${var.name}-diagnostic"
  target_resource_id             = azurerm_eventgrid_domain.eventgrid_domain.id
  eventhub_name                  = var.eventhub_name
  eventhub_authorization_rule_id = var.eventhub_namespace_authorization_rule_id

  enabled_log {
    category = "DeliveryFailures"
  }

  enabled_log {
    category = "PublishFailures"
  }
}
