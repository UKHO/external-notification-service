output "webapp_name" {
  value = azurerm_app_service.stub_webapp_service.name
}

output "default_site_hostname_ens_stub" {
  value = azurerm_app_service.stub_webapp_service.default_site_hostname
}
