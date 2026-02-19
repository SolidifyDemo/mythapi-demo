
param keyVaultName string
param principalId string

// Get existibg KeyVault
resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: keyVaultName
}

resource keyVaultSecretAccessRoleDef 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  scope: keyVault
  name: '4633458b-17de-408a-b874-0445c86b69e6'
}


resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, principalId, keyVaultSecretAccessRoleDef.id)
  properties: {
    roleDefinitionId: keyVaultSecretAccessRoleDef.id
    principalId: principalId
    principalType: 'ServicePrincipal'
  }
}
