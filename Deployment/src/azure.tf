terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=2.91.0"
    }
  }

  required_version = "=1.1.4"
  backend "azurerm" {
    container_name = "tfstate"
    key            = "terraform.deployment.tfplan"
  }
}

provider "azurerm" {
  features {}
}

provider "azurerm" {
  features {}
  alias = "build_agent"
  subscription_id = var.agent_subscription_id
}

provider "azurerm" {
  features {}
  alias           = "coreservices"
  subscription_id = var.core_services_subscription_id
}
