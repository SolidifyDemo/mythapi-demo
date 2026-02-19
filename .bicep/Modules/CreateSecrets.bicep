// Parameter for KeyVault ID
@description('The KeyVault ID')
param keyVaultId string

// Parameter for KeyVault Secret Name
@description('The KeyVault Secret Name')
param secretName string

// Parameter for KeyVault Secret Value
@description('The KeyVault Secret Value')
@secure()
param secretValue string

// Get existibg KeyVault
resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: keyVaultId
}

// Create KeyVault Secret
resource keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  parent: keyVault
  name: secretName
  properties: {
    value: secretValue
  }
}


