output ens_service_identity_principal_id {
	value = azurerm_user_assigned_identity.ens_service_identity.principal_id
}

output ens_service_identity_tenant_id {
	value = azurerm_user_assigned_identity.ens_service_identity.tenant_id
}

output ens_service_identity_id {
	value = azurerm_user_assigned_identity.ens_service_identity.id
}

output ens_service_client_id {
	value = azurerm_user_assigned_identity.ens_service_identity.client_id
}
