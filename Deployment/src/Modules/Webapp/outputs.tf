output "webapp_service" {
  value = azurerm_app_service.webapp_service.name
}

output "web_app_object_id" {
  value = azurerm_app_service.webapp_service.identity.0.principal_id
}

output "web_app_tenant_id" {
  value = azurerm_app_service.webapp_service.identity.0.tenant_id
}

output "default_site_hostname" {
  value = azurerm_app_service.webapp_service.default_site_hostname
}

output "webapp_scm_credentials"{
    value = merge({
    "${var.name}-scm-username" =  webapp.site_credential.username 
    },
    {
    "${var.name}-scm-password" =  webapp.site_credential.password 
    })
  sensitive = true
}
