output "web_app_name" {
value = local.web_app_name
}

output "web_app_url" {
value = "https://${module.webapp_service.default_site_hostname}"
}

output "keyvault_uri"{
  value = module.key_vault.keyvault_uri
}

output "web_app_resource_group" {
   value = azurerm_resource_group.rg.name
}

output "ens_managed_user_identity_client_id"{
    value = module.user_identity.ens_service_client_id
}
