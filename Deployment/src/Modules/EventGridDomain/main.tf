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

