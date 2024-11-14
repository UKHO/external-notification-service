variable "resource_group_name" {
  type = string 
}

variable "apim_service_name" {
  type = string
}

variable "env" {
  type = string
}

variable "env_suffix" {
  type = string
}

variable "apim_api_service_url" {
  type = string
} 

variable "api_openapi_spec_path" {
  type = string
}

variable "api_monitor_openapi_spec_path" {
  type = string
}

variable "d365_product_daily_quota_limit" {
  type = number
}

variable "ad_tenant_id" {
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

variable "ens_app_client_id" {
  type    = string    
}

locals {
  formatted_env = lower(replace(trimspace(var.env), " ", "-"))  
}

variable "d365_product_call_limit" {
  type = number
  default = 5
}

variable "d365_product_call_renewal_period" {
  type = number
  default = 5
}

variable "ees_product_call_limit" {
  type = number
  default = 20
}
