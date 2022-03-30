variable "name" {
  type = string
}

variable "resource_group_name" {
  type = string
}

variable "location" {
  type = string
}

variable "tags" {

}

variable "webapp_principal_id" {
  type  = string
}

variable "eventhub_namespace_authorization_rule_id" {
  type = string
}

variable "eventhub_name" {
  type = string
}
