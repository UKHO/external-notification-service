data "azurerm_api_management" "apim_instance" {
  resource_group_name = var.resource_group_name
  name                = var.apim_service_name
}

# Create apim group
resource "azurerm_api_management_group" "ens_management_group" {
  resource_group_name = var.resource_group_name
  api_management_name = data.azurerm_api_management.apim_instance.name
  name                = "ens-group-${local.formatted_env_suffix}"
  display_name        = "External Notification Service Group ${var.env_suffix}"
  description         = "Management group for users with access to the ${var.env} External Notifcation Service."
}

# Create ENS API
resource "azurerm_api_management_api" "ens_api" {
  resource_group_name = var.resource_group_name
  api_management_name = data.azurerm_api_management.apim_instance.name 
  name                = "ens-api-${local.formatted_env_suffix}"
  display_name        = "External Notification Service API ${var.env_suffix}"
  description         = "The External Notification Service API provides the ability to subscribe and deliver notifications."
  revision            = "1"
  path                = "ens-api-${local.formatted_env_suffix}"
  protocols           = ["https"]
  service_url         = var.apim_api_service_url

  subscription_key_parameter_names {
    header = "Ocp-Apim-Subscription-Key"
    query  = "subscription-key"
  }

  import {
    content_format = "openapi"
    content_value  = var.api_openapi_spec_path
  }
}

# Create D365 Product
resource "azurerm_api_management_product" "d365_product" {
  resource_group_name   = var.resource_group_name
  api_management_name   = data.azurerm_api_management.apim_instance.name
  product_id            = "ens-d365-${local.formatted_env_suffix}"
  display_name          = "External Notification Service for D365 ${var.env_suffix}"
  description           = "The External Notification Service provides the ability to subscribe and deliver notifications and to be used by D365."
  subscription_required = true
  approval_required     = true
  published             = true
  subscriptions_limit   = 1
}

# Create EES Product
resource "azurerm_api_management_product" "ees_product" {
  resource_group_name   = var.resource_group_name
  api_management_name   = data.azurerm_api_management.apim_instance.name
  product_id            = "ens-ees-${local.formatted_env_suffix}"
  display_name          = "External Notification Service for EES ${var.env_suffix}"
  description           = "The External Notification Service provides the ability to subscribe and deliver notifications and to be used by Enterprise Event Service (EES)"
  subscription_required = true
  approval_required     = true
  published             = true
  subscriptions_limit   = 1
}

# API - Product mappings
resource "azurerm_api_management_product_api" "d365_product_api_mapping" {
  resource_group_name = var.resource_group_name
  api_management_name = data.azurerm_api_management.apim_instance.name
  api_name            = azurerm_api_management_api.ens_api.name
  product_id          = azurerm_api_management_product.d365_product.product_id
}

resource "azurerm_api_management_product_api" "ees_product_api_mapping" {
  resource_group_name = var.resource_group_name
  api_management_name = data.azurerm_api_management.apim_instance.name
  api_name            = azurerm_api_management_api.ens_api.name
  product_id          = azurerm_api_management_product.ees_product.product_id
}

#D365 Product policy
resource "azurerm_api_management_product_policy" "d365_product_policy" {
  resource_group_name = var.resource_group_name
  api_management_name = data.azurerm_api_management.apim_instance.name
  product_id          = azurerm_api_management_product.d365_product.product_id
  depends_on          = [azurerm_api_management_product.d365_product, azurerm_api_management_product_api.d365_product_api_mapping]

  xml_content = <<XML
	<policies>
	  <inbound>
      <base />
      <quota calls="${var.d365_product_daily_quota_limit}" renewal-period="86400" />

      <!-- Send request to generate-token url with required values -->
      <send-request mode="new" response-variable-name="client_credentials_token" timeout="60" ignore-error="true">            
          <set-url>https://login.microsoftonline.com/${var.client_credentials_tenant_id}/oauth2/v2.0/token</set-url>
          <set-method>POST</set-method>
          <set-header name="Content-Type" exists-action="override">
              <value>application/x-www-form-urlencoded</value>
          </set-header>
          <set-body>@{
              return $"client_id=${var.client_credentials_client_id}&client_secret=${var.client_credentials_secret}&grant_type=client_credentials&scope=${var.client_credentials_scope}";
              }
          </set-body>
      </send-request>
      <choose>
        <when condition="@(((IResponse)context.Variables["client_credentials_token"]).StatusCode == 200)">
           <!-- Retrieve access_token and set Authorization header -->
           <set-header name="Authorization" exists-action="override">
                  <value>@{
                      var body = ((IResponse)context.Variables["client_credentials_token"]).Body.As<JObject>();
                      var access_token = body["access_token"];
                      return "Bearer "+ access_token.ToString();
                    }                
                  </value>
            </set-header>                    
        </when>
        <otherwise>
            <!-- Retrieve error details and send internal server response  -->
            <set-variable name="errorSource" value="@{ 
                return ((IResponse)context.Variables["client_credentials_token"]).Body.As<JObject>(true)["error"].ToString();
                }" />
            <set-variable name="rawErrorDescription" value="@{ 
                return ((IResponse)context.Variables["client_credentials_token"]).Body.As<JObject>()["error_description"].ToString();
                }" />
            <set-variable name="errorDescription" value="@{ 
                    return ((string)context.Variables["rawErrorDescription"]).Substring(0, ((string)context.Variables["rawErrorDescription"]).IndexOf("\r"));
                }" />
            <set-variable name="tokenResponseStatusCode" value="@{ 
                return ((IResponse)context.Variables.GetValueOrDefault<IResponse>("client_credentials_token")).StatusCode;
                }" />
            <set-variable name="tokenResponseStatusReason" value="@{ 
                return ((IResponse)context.Variables.GetValueOrDefault<IResponse>("client_credentials_token")).StatusReason;
                }" />
            
            <return-response>
                <set-status code="500" reason="Internal Server Error" />
                <set-header name="Content-Type" exists-action="override">
                    <value>application/json</value>
                </set-header>
                <set-body template="liquid">{
                        "correlationId": "{{context.Request.Headers["X-Correlation-ID"]}}",
                        "errors": [
                                    {
                                        "Source": "Oauth2 token"
                                        "Description": "Error while generating Oauth2 token for backend ENS service"
                                        "InnerExceptionSource": "{{context.Variables["errorSource"]}}",
                                        "InnerExceptionDescription": "{{context.Variables["errorDescription"]}}"
                                        "TokenResponseStatusCode": "{{context.Variables["tokenResponseStatusCode"]}}"
                                        "TokenResponseStatusReason": "{{context.Variables["tokenResponseStatusReason"]}}"
                                    }
                                ]
                            }
                    </set-body>
            </return-response>                    
        </otherwise>
      </choose>

	  </inbound>
	</policies>
	XML
}



