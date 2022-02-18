data "azurerm_api_management" "apim_instance" {
  resource_group_name = var.resource_group_name
  name                = var.apim_service_name
}

# Create apim group
resource "azurerm_api_management_group" "ens_management_group" {
  resource_group_name = var.resource_group_name
  api_management_name = data.azurerm_api_management.apim_instance.name
  name                = "ens-group-${local.formatted_env_suffix}"
  display_name        = "External Notification Service Group ${var.env_suffix}"
  description         = "Management group for users with access to the ${var.env} External Notifcation Service."
}

# Create ENS API
resource "azurerm_api_management_api" "ens_api" {
  resource_group_name = var.resource_group_name
  api_management_name = data.azurerm_api_management.apim_instance.name 
  name                = "ens-api-${local.formatted_env_suffix}"
  display_name        = "External Notification Service API ${var.env_suffix}"
  description         = "The External Notification Service API provides the ability to subscribe and deliver notifications."
  revision            = "1"
  path                = "ens-api-${local.formatted_env_suffix}"
  protocols           = ["https"]
  service_url         = var.apim_api_service_url

  subscription_key_parameter_names {
    header = "Ocp-Apim-Subscription-Key"
    query  = "subscription-key"
  }

  import {
    content_format = "openapi"
    content_value  = var.api_openapi_spec_path
  }
}

# Create D365 Product
resource "azurerm_api_management_product" "d365_product" {
  resource_group_name   = var.resource_group_name
  api_management_name   = data.azurerm_api_management.apim_instance.name
  product_id            = "ens-d365-${local.formatted_env_suffix}"
  display_name          = "External Notification Service for D365 ${var.env_suffix}"
  description           = "The External Notification Service provides the ability to subscribe and deliver notifications and to be used by D365."
  subscription_required = true
  approval_required     = true
  published             = true
  subscriptions_limit   = 1
}

# Create EES Product
resource "azurerm_api_management_product" "ees_product" {
  resource_group_name   = var.resource_group_name
  api_management_name   = data.azurerm_api_management.apim_instance.name
  product_id            = "ens-ees-${local.formatted_env_suffix}"
  display_name          = "External Notification Service for EES ${var.env_suffix}"
  description           = "The External Notification Service provides the ability to subscribe and deliver notifications and to be used by Enterprise Event Service (EES)"
  subscription_required = true
  approval_required     = true
  published             = true
  subscriptions_limit   = 1
}

# API - Product mappings
resource "azurerm_api_management_product_api" "d365_product_api_mapping" {
  resource_group_name = var.resource_group_name
  api_management_name = data.azurerm_api_management.apim_instance.name
  api_name            = azurerm_api_management_api.ens_api.name
  product_id          = azurerm_api_management_product.d365_product.product_id
}

resource "azurerm_api_management_product_api" "ees_product_api_mapping" {
  resource_group_name = var.resource_group_name
  api_management_name = data.azurerm_api_management.apim_instance.name
  api_name            = azurerm_api_management_api.ens_api.name
  product_id          = azurerm_api_management_product.ees_product.product_id
}



