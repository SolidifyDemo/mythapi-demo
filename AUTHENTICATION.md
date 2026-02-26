# Authentication and Authorization

This API uses JWT (JSON Web Token) Bearer authentication to secure endpoints that modify data.

## Overview

- **GET endpoints** - Public access (no authentication required)
- **POST endpoints** - Require authentication with Admin role

## Configuration

Authentication is configured in `src/Program.cs` with the following default values for development:

```csharp
Jwt:Key = "ThisIsASecretKeyForDevelopmentPurposesOnly-ChangeInProduction-MustBeAtLeast32Characters"
Jwt:Issuer = "MythApi"
Jwt:Audience = "MythApiUsers"
```

### Production Configuration

For production, override these values using:
1. Environment variables
2. Azure Key Vault
3. appsettings.json

**Example using environment variables:**
```bash
export Jwt__Key="YourSecureProductionKeyHere"
export Jwt__Issuer="YourProductionIssuer"
export Jwt__Audience="YourProductionAudience"
```

## Authorization Policies

Two authorization policies are defined:

1. **AdminOnly** - Requires the user to have the "Admin" role
2. **ReadOnly** - Requires any authenticated user

## Generating JWT Tokens

For testing purposes, you can generate tokens using the helper in `tests/IntegrationTests/TestJwtTokenGenerator.cs`:

```csharp
// Generate admin token
var adminToken = TestJwtTokenGenerator.GenerateAdminToken("username");

// Generate user token
var userToken = TestJwtTokenGenerator.GenerateUserToken("username");
```

### Manual Token Generation

You can also generate tokens manually using your preferred JWT library. The token must include:

- **Issuer**: Must match configured Jwt:Issuer
- **Audience**: Must match configured Jwt:Audience
- **Claims**:
  - `name` or `sub`: Username
  - `role`: User role (e.g., "Admin", "User")
- **Signature**: Must be signed with the configured Jwt:Key using HS256

### Example Token Payload

```json
{
  "sub": "admin",
  "name": "admin",
  "role": "Admin",
  "jti": "unique-id",
  "exp": 1735689600,
  "iss": "MythApi",
  "aud": "MythApiUsers"
}
```

## Using Authentication

### With cURL

```bash
# Get all gods (no auth required)
curl http://localhost:5000/api/v1/gods

# Create a god (requires Admin token)
curl -X POST http://localhost:5000/api/v1/gods \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '[{"name":"NewGod","mythologyId":1,"description":"A new god"}]'
```

### With Swagger UI

When using the Swagger UI at `/swagger`:

1. Click the "Authorize" button
2. Enter your JWT token in the format: `Bearer YOUR_TOKEN_HERE`
3. Click "Authorize"
4. You can now test authenticated endpoints

## Endpoints and Required Roles

| Endpoint | Method | Authentication Required | Required Role |
|----------|--------|------------------------|---------------|
| `/api/v1/gods` | GET | No | - |
| `/api/v1/gods/{id}` | GET | No | - |
| `/api/v1/gods/search/{name}` | GET | No | - |
| `/api/v1/gods` | POST | Yes | Admin |
| `/api/v1/mythologies` | GET | No | - |

## Security Considerations

1. **HTTPS**: Always use HTTPS in production to protect tokens in transit
2. **Token Expiration**: Tokens expire after 60 minutes by default
3. **Secret Key**: Use a strong, randomly generated key in production (minimum 32 characters)
4. **Clock Skew**: Set to zero for strict expiration validation
5. **Token Storage**: Store tokens securely on the client side (e.g., secure cookies, secure storage)

## Testing

Run the authentication tests to verify the implementation:

```bash
dotnet test --filter "FullyQualifiedName~AuthenticationTests"
```

The test suite includes:
- ✅ Public access to GET endpoints
- ✅ Unauthorized access blocked on POST endpoints
- ✅ Admin role required for data modification
- ✅ Invalid token rejection
- ✅ Expired token rejection
- ✅ Role-based authorization enforcement

## Troubleshooting

### 401 Unauthorized
- Check that your token is valid and not expired
- Verify the token is properly formatted: `Authorization: Bearer TOKEN`
- Ensure the token was signed with the correct key

### 403 Forbidden
- Check that your user has the required role (e.g., "Admin")
- Verify the role claim is present in the token

### Token Not Validated
- Verify Jwt:Key, Jwt:Issuer, and Jwt:Audience match between token generation and API configuration
- Check that UseAuthentication() and UseAuthorization() are called in Program.cs
