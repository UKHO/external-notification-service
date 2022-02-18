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

locals {
  formatted_env_suffix = lower(replace(trimspace(var.env_suffix), " ", "-"))
}
