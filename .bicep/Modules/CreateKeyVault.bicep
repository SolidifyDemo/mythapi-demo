
// Parameter for the name of the key vault to create
@description('The name of the key vault to create.')
param keyVaultName string = 'godkeyvault${uniqueString(resourceGroup().id)}'

// Parameter for the location of the key vault to create
@description('The location of the key vault to create.')
param keyVaultLocation string = resourceGroup().location

// Get the tenant id
var tennant_id = subscription().tenantId

// Create KeyVault

resource vault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: keyVaultName
  location: keyVaultLocation
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: tennant_id
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enableRbacAuthorization: true
    provisioningState: 'Succeeded'
    publicNetworkAccess: 'Enabled'
  }
}
