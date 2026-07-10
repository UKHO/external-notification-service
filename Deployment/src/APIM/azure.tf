terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">=4.54.0"
    }
  }

  required_version = "=1.15.8"
  backend "azurerm" {
    container_name = "tfstate"
    key            = "terraform.apim.tfplan"
  }
}

provider "azurerm" {
  features {}
  version = ">=4.54.0"
}
