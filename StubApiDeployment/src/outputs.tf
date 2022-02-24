output "mock_webappname" {
  value = module.webapp_service.webapp_name
}

output "ens_stub_web_app_url" {
value = "https://${module.webapp_service.default_site_hostname_ens_stub}/api/callback"
}
