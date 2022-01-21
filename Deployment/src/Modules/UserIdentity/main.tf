resource "azurerm_user_assigned_identity" "ens_service_identity" {
  resource_group_name = var.resource_group_name
  location            = var.location
  name				  = "ens-${var.env_name}-service-identity"
  tags				  = var.tags
}
