
@secure()
param postgresDBUsername string
@secure()
param postgresDBPassword string

param keyVaultId string

param postgresDBName string = 'goddbthisisatest'

param postgresDBLocation string = resourceGroup().location


resource postgresDB 'Microsoft.DBforPostgreSQL/flexibleServers@2023-03-01-preview' = {
  name: postgresDBName
  location: postgresDBLocation
  sku: {
    name: 'Standard_B1ms'
    tier: 'Burstable'
  }
  properties: {
    version: '15'
    createMode: 'Default'
    administratorLogin: postgresDBUsername
    administratorLoginPassword: postgresDBPassword
  }
}

var postgresdbhost = postgresDB.properties.fullyQualifiedDomainName

module setDBHost 'CreateSecrets.bicep' = {
  name: 'setDBHost'
  dependsOn: [
    postgresDB
  ]
  params: {
    keyVaultId: keyVaultId
    secretName: 'dbHost' 
    secretValue: postgresdbhost
  }
}

module setAdminUserName 'CreateSecrets.bicep' = {
  name: 'setAdminUsername'
  dependsOn: [
    postgresDB
  ]
  params: {
    keyVaultId: keyVaultId
    secretName: 'adminUsername' 
    secretValue: postgresDBUsername
  }
}

module setAdminPassword 'CreateSecrets.bicep' = {
  name: 'setAdminPassword'
  dependsOn: [
    postgresDB
  ]
  params: {
    keyVaultId: keyVaultId
    secretName: 'adminPassword' 
    secretValue: postgresDBPassword
  }
}

module setDBName 'CreateSecrets.bicep' = {
  name: 'setDBName'
  dependsOn: [
    postgresDB
  ]
  params: {
    keyVaultId: keyVaultId
    secretName: 'dbName' 
    secretValue: 'postgres'
  }
}
