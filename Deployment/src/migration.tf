# Used to migrate deprecated resources without having to destroy and recreate them.
# This file can be removed once the migration is complete in all environments, but will do no harm if left.
# See PBI #145964.

# service plan
removed {
  from = module.webapp_service.azurerm_app_service_plan.app_service_plan

  lifecycle {
    destroy = false
  }
}

import {
  to = module.webapp_service.azurerm_service_plan.app_service_plan
  id = "${azurerm_resource_group.rg.id}/providers/Microsoft.Web/serverFarms/${local.web_app_name}-asp"
}

# webapp
removed {
  from = module.webapp_service.azurerm_app_service.webapp_service

  lifecycle {
    destroy = false
  }
}

import {
  to = module.webapp_service.azurerm_windows_web_app.webapp_service
  id = "${azurerm_resource_group.rg.id}/providers/Microsoft.Web/sites/${local.web_app_name}"
}

# staging
removed {
  from = module.webapp_service.azurerm_app_service_slot.staging

  lifecycle {
    destroy = false
  }
}

import {
  to = module.webapp_service.azurerm_windows_web_app_slot.staging
  id = "${azurerm_resource_group.rg.id}/providers/Microsoft.Web/sites/${local.web_app_name}/slots/staging"
}

# stub (non-live only)
removed {
  from = module.webapp_service.azurerm_app_service.stub_webapp_service

  lifecycle {
    destroy = false
  }
}

locals {
  stubs = (
    local.deploy_stub ? [0] : []
  )
}

import {
  for_each = local.stubs
  to    = module.webapp_service.azurerm_windows_web_app.stub_webapp_service[each.key]
  id    = "${azurerm_resource_group.rg.id}/providers/Microsoft.Web/sites/${local.web_app_name}-stub"
}

# swift
removed {
  from = module.webapp_service.azurerm_app_service_virtual_network_swift_connection.webapp_vnet_integration

  lifecycle {
    destroy = false
  }
}

removed {
  from = module.webapp_service.azurerm_app_service_slot_virtual_network_swift_connection.slot_vnet_integration

  lifecycle {
    destroy = false
  }
}

import {
  to = module.webapp_service.azurerm_app_service_virtual_network_swift_connection.webapp_vnet_integration2
  id = "${azurerm_resource_group.rg.id}/providers/Microsoft.Web/sites/${local.web_app_name}/config/virtualNetwork"
}

import {
  to = module.webapp_service.azurerm_app_service_slot_virtual_network_swift_connection.slot_vnet_integration2
  id = "${azurerm_resource_group.rg.id}/providers/Microsoft.Web/sites/${local.web_app_name}/slots/staging/config/virtualNetwork"
}

# dashboard
removed {
  from = module.azure-dashboard.azurerm_dashboard.azure-dashboard

  lifecycle {
    destroy = false
  }
}

import {
  to = module.azure-dashboard.azurerm_portal_dashboard.azure-dashboard
  id = "${azurerm_resource_group.rg.id}/providers/Microsoft.Portal/dashboards/ENS-${local.env_name}-monitoring-dashboard"
}
