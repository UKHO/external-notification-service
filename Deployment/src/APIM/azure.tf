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
    key            = "terraform.apim.tfplan"
  }
}

provider "azurerm" {
  features {}
}
