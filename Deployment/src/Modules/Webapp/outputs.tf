output "web_app_object_id" {
  value = azurerm_windows_web_app.webapp_service.identity.0.principal_id
}

output "web_app_tenant_id" {
  value = azurerm_windows_web_app.webapp_service.identity.0.tenant_id
}

output "default_site_hostname" {
  value = azurerm_windows_web_app.webapp_service.default_hostname
}

output "webapp_scm_credentials"{
  value = merge({
    "${var.name}-scm-username" =  azurerm_windows_web_app.webapp_service.site_credential[0].name 
    },
    {
    "${var.name}-scm-password" =  azurerm_windows_web_app.webapp_service.site_credential[0].password 
    })
  sensitive = true
}

output "webapp_name" {
  value = var.env_name == "live" ? null : azurerm_windows_web_app.stub_webapp_service[0].name
}

output "default_site_hostname_ens_stub" {
  value = var.env_name == "live" ?  null : azurerm_windows_web_app.stub_webapp_service[0].default_hostname
}

output "slot_principal_id" {
  value = azurerm_windows_web_app_slot.staging.identity.0.principal_id
}

output "slot_name" {
  value = azurerm_windows_web_app_slot.staging.name
}

output "slot_default_site_hostname" {
  value = azurerm_windows_web_app_slot.staging.default_hostname
}
