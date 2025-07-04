@description('The name of the web app')
param webAppName string = 'easywowmacro'

@description('The name of the resource group')
param resourceGroupName string = 'EasyWoWMacro-RG'

@description('The location for all resources')
param location string = 'West Europe'

@description('The SKU for the App Service Plan')
param appServicePlanSku string = 'B1'

@description('The runtime for the web app')
param runtime string = 'DOTNETCORE:9.0'

@description('Custom domain name (optional)')
param customDomain string = ''

@description('Enable HTTPS only')
param httpsOnly bool = true

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: '${webAppName}-Plan'
  location: location
  sku: {
    name: appServicePlanSku
    tier: appServicePlanSku == 'F1' ? 'Free' : 
          appServicePlanSku == 'B1' ? 'Basic' : 
          appServicePlanSku == 'S1' ? 'Standard' : 'Premium'
  }
  kind: 'linux'
  reserved: true
}

// Web App
resource webApp 'Microsoft.Web/sites@2023-01-01' = {
  name: webAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: httpsOnly
    siteConfig: {
      linuxFxVersion: runtime
      appSettings: [
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'DOTNET_ENVIRONMENT'
          value: 'Production'
        }
      ]
    }
  }
}

// Custom domain binding (if provided)
resource customDomainBinding 'Microsoft.Web/sites/hostNameBindings@2023-01-01' = if (!empty(customDomain)) {
  name: '${webApp.name}/${customDomain}'
  properties: {
    hostNameType: 'Verified'
  }
}

// Application Insights (optional - uncomment if needed)
// resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
//   name: '${webAppName}-insights'
//   location: location
//   kind: 'web'
//   properties: {
//     Application_Type: 'web'
//     WorkspaceResourceId: logAnalyticsWorkspace.id
//   }
// }

// Log Analytics Workspace (optional - uncomment if needed)
// resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
//   name: '${webAppName}-logs'
//   location: location
//   properties: {
//     sku: {
//       name: 'PerGB2018'
//     }
//     retentionInDays: 30
//   }
// }

// Outputs
output webAppName string = webApp.name
output webAppUrl string = 'https://${webApp.properties.defaultHostName}'
output customDomainUrl string = !empty(customDomain) ? 'https://${customDomain}' : ''
output resourceGroupName string = resourceGroupName
output appServicePlanName string = appServicePlan.name 