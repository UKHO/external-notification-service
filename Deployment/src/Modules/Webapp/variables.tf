variable "name" {
  type = string
}

variable "env_name" {
  type  = string
}

variable "resource_group_name" {
  type = string
}

variable "location" {
  type = string
}

variable "app_settings" {
  type = map(string)
}


variable "tags" {

}

variable "app_service_sku" {

}

variable "app_settings_stub" {
  type = map(string)
}

variable "allowed_ips" {

}

variable "subnet_id" {
  type = string
}

variable "agent_id" {
  type = string
}
