# Used to migrate deprecated resources without having to destroy and recreate them.
# This file can be removed once the migration is complete in all environments, but will do no harm if left.
# See PBI #145964.

# webapp
removed {
  from = azurerm_app_service.webapp_service

  lifecycle {
    destroy = false
  }
}

import {
  to = azurerm_windows_web_app.webapp_service
  id = "${var.resource_group_id}/providers/Microsoft.Web/sites/${var.name}"
}

# staging
removed {
  from = azurerm_app_service_slot.staging

  lifecycle {
    destroy = false
  }
}

import {
  to = azurerm_windows_web_app_slot.staging
  id = "${var.resource_group_id}/providers/Microsoft.Web/sites/${var.name}"
  id = "${var.resource_group_id}/providers/Microsoft.Web/sites/${var.name}/slots/staging"
}

# stub (non-live only)
removed {
  from = azurerm_app_service.stub_webapp_service

  lifecycle {
    destroy = false
  }
}

locals {
  stubs = (
    var.env_name == "live" ? [] : [0]
  )
}

import {
  for_each = local.stubs
  to    = azurerm_windows_web_app.stub_webapp_service[each.key]
  id    = "${var.resource_group_id}/providers/Microsoft.Web/sites/${var.name}-stub"
}
