resource "azurerm_app_service_plan" "app_service_plan" {
  name                = "${var.name}-asp"
  location            = var.location
  resource_group_name = var.resource_group_name
  
  sku {
	tier = var.app_service_sku.tier
	size = var.app_service_sku.size
  }
  tags                = var.tags
}

resource "azurerm_app_service" "webapp_service" {
  name                = var.name
  location            = var.location
  resource_group_name = var.resource_group_name
  app_service_plan_id = azurerm_app_service_plan.app_service_plan.id
  tags                = var.tags

  site_config {
    windows_fx_version  =   "DOTNET|8.0"
    
    always_on  = true
    ftps_state = "Disabled"

    ip_restriction {
      virtual_network_subnet_id = var.agent_id
    }

    ip_restriction {
      virtual_network_subnet_id = var.subnet_id
    }

    dynamic "ip_restriction" {
      for_each = var.allowed_ips
      content {
          ip_address  = length(split("/",ip_restriction.value)) > 1 ? ip_restriction.value : "${ip_restriction.value}/32"
      }
    }

}

  app_settings = var.app_settings

  identity {
    type = "SystemAssigned"
  }

  https_only = true
}

resource "azurerm_app_service_slot" "staging" {
  name                = "staging"
  app_service_name    = azurerm_app_service.webapp_service.name
  location            = azurerm_app_service.webapp_service.location
  resource_group_name = azurerm_app_service.webapp_service.resource_group_name
  app_service_plan_id = azurerm_app_service.webapp_service.app_service_plan_id
  tags                = azurerm_app_service.webapp_service.tags

  site_config {
    windows_fx_version  =   "DOTNET|8.0"
    
    always_on  = true
    ftps_state = "Disabled"

    ip_restriction {
      virtual_network_subnet_id = var.agent_id
    }

    ip_restriction {
      virtual_network_subnet_id = var.subnet_id
    }

    dynamic "ip_restriction" {
      for_each = var.allowed_ips
      content {
          ip_address  = length(split("/",ip_restriction.value)) > 1 ? ip_restriction.value : "${ip_restriction.value}/32"
      }
    }

}

  app_settings = azurerm_app_service.webapp_service.app_settings

  identity {
    type = "SystemAssigned"
  }

  https_only = azurerm_app_service.webapp_service.https_only
}

resource "azurerm_app_service" "stub_webapp_service" {
  count               = var.env_name == "live" ? 0 : 1
  name                = "${var.name}-stub"
  location            = var.location
  resource_group_name = var.resource_group_name
  app_service_plan_id = azurerm_app_service_plan.app_service_plan.id
  tags                = var.tags

  site_config {
    windows_fx_version  =   "DOTNET|8.0"

    always_on  = true
    ftps_state = "Disabled"
  }

  app_settings = var.app_settings_stub

  identity {
    type = "SystemAssigned"
  }

  https_only = true
}

resource "azurerm_app_service_virtual_network_swift_connection" "webapp_vnet_integration" {
  app_service_id = azurerm_app_service.webapp_service.id
  subnet_id      = var.subnet_id
}

resource "azurerm_app_service_slot_virtual_network_swift_connection" "slot_vnet_integration" {
  app_service_id = azurerm_app_service.webapp_service.id
  subnet_id      = var.subnet_id
  slot_name      = azurerm_app_service_slot.staging.name
}
