resource "azurerm_eventgrid_domain" "eventgrid_domain" {
  name                = var.name
  location            = var.location
  resource_group_name = var.resource_group_name
  tags                = var.tags
  input_schema        = "CloudEventSchemaV1_0"
}

resource "azurerm_role_assignment" "eventgrid_domain_role" {
  scope                = azurerm_eventgrid_domain.eventgrid_domain.id
  role_definition_name = "EventGrid Contributor"
  principal_id         = var.webapp_principal_id
}

resource "azurerm_monitor_diagnostic_setting" "egdiagnosticsetting" {
  name               = "${var.name}-diagnostic"
  target_resource_id = azurerm_eventgrid_domain.eventgrid_domain.id
  eventhub_authorization_rule_id = var.eventhub_namespace_authorization_rule_id

  log {
    category = "DeliveryFailures"
    enabled  = true

    retention_policy {
      enabled = true
    }
    }

  log {
    category = "PublishFailures"
    enabled  = true

    retention_policy {
      enabled = true
    }
  }
}

