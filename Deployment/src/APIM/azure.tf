terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=2.58.0"
    }
  }

  required_version = "=0.14.9"
  backend "azurerm" {
    container_name = "tfstate"
    key            = "terraform.apim.tfplan"
  }
}

provider "azurerm" {
  features {}
}