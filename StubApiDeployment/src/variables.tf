variable "location" {
  type    = string
  default = "uksouth"
}

variable "ens_api_rg" {
  type    = string
  default = "ens-dev-rg"
}

variable "ens_api_asp" {
  type    = string
}

locals {
  env_name           = lower(terraform.workspace)
  ens_api_rg         = "${ens_api_rg}"
  service_name       = "ens"
  web_app_name       = "${local.service_name}-${local.env_name}-stub-webapp"
  tags = {
    SERVICE          = "External Notification Service"
    ENVIRONMENT      = local.env_name
    SERVICE_OWNER    = "UKHO"
    RESPONSIBLE_TEAM = "Mastek"
    CALLOUT_TEAM     = "On-Call_N/A"
    COST_CENTRE      = "A.008.02"
  }
}


variable "app_service_sku" {
  type = map(any)
  default = {
    "qc"     = {
	    tier = "PremiumV3"
	    size = "P1v3"
    }
  }
}
