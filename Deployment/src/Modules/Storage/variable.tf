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

variable "service_name" {
  type = string
}

variable "env_name" {
  type  = string
}

variable "allowed_ips" {

}
variable "m_spoke_subnet" {
  type = string
}

variable "agent_subnet" {
  type = string
}

variable "slot_principal_id" {
  type  = string
}
