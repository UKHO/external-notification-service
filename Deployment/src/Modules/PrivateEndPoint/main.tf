resource "azurerm_private_endpoint" "eventgriddomain_endpoint" {
  name                    = "EventGridDomain-${var.env_name}-EndPoint"
  resource_group_name     = var.resource_group_name
  location                = var.location
  subnet_id               = var.subnet_id

  private_service_connection {
    name                              = "${var.env_name}-privateserviceconnection"
    private_connection_resource_id    = var.event_grid_domain_resource_id
    is_manual_connection              = false
    subresource_names                 = ["domain"]
  }
  private_dns_zone_group {
    name                 = "privatelink.eventgrid.azure.net"
    private_dns_zone_ids = [var.private_dns_zone_resource_id]
  }
}
