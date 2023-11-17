variable "location" {
  type    = string
  default = "uksouth"
}

variable "resource_group_name" {
  type    = string
  default = "ens"
}

locals {
  env_name           = lower(terraform.workspace)
  service_name       = "ens"
  web_app_name       = "${local.service_name}-${local.env_name}-webapp"
  key_vault_name     = "${local.service_name}-ukho-${local.env_name}-kv"
  tags = {
    SERVICE          = "External Notification Service"
    ENVIRONMENT      = local.env_name
    SERVICE_OWNER    = "Paul Tooze"
    RESPONSIBLE_TEAM = "Abzu"
    CALLOUT_TEAM     = "On-Call_N/A"
    COST_CENTRE      = "A.008.02"
  }
}


variable "app_service_sku" {
  type = map(any)
  default = {
    "dev"    = {
	    tier = "PremiumV2"
	    size = "P1v2"
        }
    "qa"     = {
	    tier = "PremiumV3"
	    size = "P1v3"
        }
    "live"   = {
	    tier = "PremiumV3"
	    size = "P1v3"
        }
  }
}

variable "spoke_rg" {
  type = string
}

variable "spoke_vnet_name" {
  type = string
}

variable "spoke_subnet_name" {
  type = string
}

variable "private_endpoint_subnet_name" {
  type = string
}

variable "agent_rg" {
  type = string
}

variable "agent_vnet_name" {
  type = string
}

variable "agent_subnet_name" {
  type = string
}

variable "agent_subscription_id" {
  type = string
}

variable "core_services_subscription_id" {
  type = string
}

variable "core_services_rg" {
  type = string
}

variable "allowed_ips" {
  type = list
}

variable "elastic_apm_server_url" {
  type = string
}

variable "elastic_apm_api_key" {
  type = string
}

variable "elastic_apm_environment" {
  type = string
}

variable "elastic_apm_service_name" {
  type = string
}
