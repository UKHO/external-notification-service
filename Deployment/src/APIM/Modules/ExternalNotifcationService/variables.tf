variable "resource_group_name" {
  type = string
  default = "apim-rg"
}

variable "apim_service_name" {
  type = string
  default = "vk-apim"
}

variable "env" {
  type = string
}

variable "env_suffix" {
  type = string
}

variable "apim_api_service_url" {
  type = string
  default = "https://vk-ens.azurewebsites.net/"
} 

variable "api_openapi_spec_path" {
  type = string
}

variable "d365_product_daily_quota_limit" {
  type = number
  default = 25 
}

variable "client_credentials_tenant_id" {
  type    = string
}

variable "client_credentials_client_id" {
  type    = string
}

variable "client_credentials_secret" {
  type    = string
}

variable "client_credentials_scope" {
  type    = string
}

locals {
  formatted_env = lower(replace(trimspace(var.env), " ", "-"))
}
