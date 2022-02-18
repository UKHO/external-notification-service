variable "env_suffix_text" {
  type = map(string)
  default = {
    "dev"     = "Dev"
    "qa"      = "QA"
    "distdev" = "Dist Dev"
    "prod"    = " "
  }
}

locals {
  env_name                        = lower(terraform.workspace)
}