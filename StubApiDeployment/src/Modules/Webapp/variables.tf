variable "ens_api_asp" {
  type    = string
}

variable "name" {
  type = string
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
