variable "resource_group_name" {
  type = string
}

variable "apim_service_name" {
  type = string
}

variable "apim_api_service_url" {
  type = string
} 

variable "d365_product_daily_quota_limit" {
  type = number
  default = 25 
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

variable "env_suffix_text" {
  type = map(string)
  default = {
    "dev"     = "Dev"
    "qa"      = "QA"
    "distdev" = "Dist Dev"
    "live"    = " "
    "vne"     = "VNE"
  }
}

locals {
  env_name    = lower(terraform.workspace)
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
