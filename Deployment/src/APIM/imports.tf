# This will import an existing APIM if Terraform is not already aware of it.
# This was used to import following the migration of APIMs from TPE to UKHO tenants.

data "azurerm_api_management" "apim_instance" {
  resource_group_name = var.resource_group_name
  name                = var.apim_service_name
}

locals {
  import_formatted_env   = lower(replace(trimspace("${local.env_name}"), " ", "-"))
  import_group_name      = "ens-group-${local.import_formatted_env}"
  import_api_name        = "ens-api-${local.import_formatted_env}"
  import_d365_product_id = "ens-d365-${local.import_formatted_env}"
  import_ees_product_id  = "ens-ees-${local.import_formatted_env}"
  import_named_value     = "ens-token-secret-${local.import_formatted_env}"
}

import {
  to = module.APIM.azurerm_api_management_group.ens_management_group
  id = "${data.azurerm_api_management.apim_instance.id}/groups/${local.import_group_name}"
}

import {
  to = module.APIM.azurerm_api_management_api.ens_api
  id = "${data.azurerm_api_management.apim_instance.id}/apis/${local.import_api_name}"
}

import {
  to = module.APIM.azurerm_api_management_product.d365_product
  id = "${data.azurerm_api_management.apim_instance.id}/products/${local.import_d365_product_id}"
}

import {
  to = module.APIM.azurerm_api_management_product.ees_product
  id = "${data.azurerm_api_management.apim_instance.id}/products/${local.import_ees_product_id}"
}

import {
  to = module.APIM.azurerm_api_management_product_api.d365_product_api_mapping
  id = "${data.azurerm_api_management.apim_instance.id}/products/${local.import_d365_product_id}/apis/${local.import_api_name}"
}

import {
  to = module.APIM.azurerm_api_management_product_api.ees_product_api_mapping
  id = "${data.azurerm_api_management.apim_instance.id}/products/${local.import_ees_product_id}/apis/${local.import_api_name}"
}

import {
  to = module.APIM.azurerm_api_management_product_group.d365_product_group_mappping
  id = "${data.azurerm_api_management.apim_instance.id}/products/${local.import_d365_product_id}/groups/${local.import_group_name}"
}

import {
  to = module.APIM.azurerm_api_management_product_group.ees_product_group_mappping
  id = "${data.azurerm_api_management.apim_instance.id}/products/${local.import_ees_product_id}/groups/${local.import_group_name}"
}

import {
  to = module.APIM.azurerm_api_management_named_value.ens_token_secret_named_value
  id = "${data.azurerm_api_management.apim_instance.id}/namedValues/${local.import_named_value}"
}

import {
  to = module.APIM.azurerm_api_management_product_policy.d365_product_policy
  id = "${data.azurerm_api_management.apim_instance.id}/products/${local.import_d365_product_id}"
}

import {
  to = module.APIM.azurerm_api_management_product_policy.ees_product_policy
  id = "${data.azurerm_api_management.apim_instance.id}/products/${local.import_ees_product_id}"
}
