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
    "${var.name}-scm-username" =  azurerm_app_service.webapp_service.site_credential[0].username 
    },
    {
    "${var.name}-scm-password" =  azurerm_app_service.webapp_service.site_credential[0].password 
    })
  sensitive = true
}

output "webapp_name" {
  value = azurerm_app_service.stub_webapp_service[count.index].name
}

output "default_site_hostname_ens_stub" {
  value = azurerm_app_service.stub_webapp_service[count.index].default_site_hostname
}
