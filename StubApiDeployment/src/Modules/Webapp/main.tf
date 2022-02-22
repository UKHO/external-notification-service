data "azurerm_resource_group" "rg" {
  name = var.ens_api_rg
}

data "azurerm_app_service_plan" "app_service_plan" {
  name                = "var.ens_api_asp"
  resource_group_name = data.azurerm_resource_group.rg.name
}

resource "azurerm_app_service" "stub_webapp_service" {
  name                = var.name
  location            = data.azurerm_resource_group.rg.location
  resource_group_name = data.azurerm_resource_group.rg.name
  app_service_plan_id = azurerm_app_service_plan.app_service_plan.id
  tags                = var.tags

  site_config {
    windows_fx_version  =   "DOTNET|6.0"
    
    always_on  = true
    ftps_state = "Disabled"
  }

  app_settings = var.app_settings

  identity {
    type = "SystemAssigned"
  }

  https_only = true
}
