output "scs_fss_mock_webapp" {
  value = azurerm_app_service.scs_fss_mock_webapp.name
}

output "web_app_object_id_scs_fss_mock" {
  value = azurerm_app_service.scs_fss_mock_webapp.identity.0.principal_id
}
