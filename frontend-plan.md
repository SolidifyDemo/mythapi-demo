# Frontend Implementation Plan for MythAPI

## 1. Overview

This document evaluates what it takes to add a frontend to the MythAPI application and provides a comprehensive implementation plan. The frontend will be placed in `src/frontend/` alongside the existing .NET API source code.

---

## 2. Current API Surface

The frontend must integrate with the following existing API endpoints:

### Gods Endpoints (`/api/v1/gods`)

| Method | Route | Description | Request Body | Response |
|--------|-------|-------------|--------------|----------|
| GET | `/api/v1/gods` | Get all gods | — | `List<God>` |
| GET | `/api/v1/gods/{id}` | Get god by ID | — | `God` |
| GET | `/api/v1/gods/search/{name}?includeAliases=bool` | Search gods by name | — | `List<God>` |
| POST | `/api/v1/gods` | Add or update gods | `List<GodInput>` | `List<God>` |

### Mythologies Endpoints (`/api/v1/mythologies`)

| Method | Route | Description | Request Body | Response |
|--------|-------|-------------|--------------|----------|
| GET | `/api/v1/mythologies` | Get all mythologies | — | `List<Mythology>` |

### Data Models

```
Mythology {
  Id: int
  Name: string
  Gods: God[]
}

God {
  Id: int
  Name: string
  Description: string
  MythologyId: int
  Aliases: Alias[]
}

Alias {
  Id: int
  GodId: int
  Name: string
}

GodInput {
  Id?: int
  Name: string
  Description: string
  MythologyId: int
}
```

---

## 3. Technology Evaluation

### Option A: React (with Vite + TypeScript) — **Recommended**

| Criteria | Assessment |
|----------|------------|
| Ecosystem maturity | Excellent — largest ecosystem, extensive tooling |
| TypeScript support | First-class with Vite scaffolding |
| Learning resources | Abundant |
| Testing | Vitest + React Testing Library (well-established) |
| Build tooling | Vite — fast HMR, production builds |
| Docker integration | Simple — multi-stage build with `node` + `nginx` or serve via .NET |
| Community | Largest of all frontend frameworks |

### Option B: Vue 3 (with Vite + TypeScript)

| Criteria | Assessment |
|----------|------------|
| Ecosystem maturity | Strong — growing ecosystem |
| TypeScript support | Good (built-in with Vite) |
| Learning curve | Lower than React for beginners |
| Testing | Vitest + Vue Test Utils |
| Build tooling | Vite — same as React option |

### Option C: Vanilla HTML/CSS/JS (No Framework)

| Criteria | Assessment |
|----------|------------|
| Complexity | Minimal — no build step required |
| Maintenance | Harder to scale as UI grows |
| Testing | Manual or basic Jest setup |
| Best for | Very simple UIs or quick prototypes |

### Recommendation

**React with Vite + TypeScript** is recommended for the following reasons:
- Aligns with industry standard tooling
- TypeScript provides type safety matching the strongly-typed .NET backend
- Vite provides fast development experience
- Easy to integrate with existing Docker and CI workflows
- Scalable architecture for future feature additions

---

## 4. Proposed Project Structure

```
src/
├── frontend/                    # New frontend project root
│   ├── public/                  # Static assets
│   │   └── favicon.ico
│   ├── src/
│   │   ├── api/                 # API client layer
│   │   │   ├── client.ts        # Base HTTP client (fetch/axios wrapper)
│   │   │   ├── gods.ts          # Gods API functions
│   │   │   └── mythologies.ts   # Mythologies API functions
│   │   ├── components/          # Reusable UI components
│   │   │   ├── GodCard.tsx      # Individual god display
│   │   │   ├── GodList.tsx      # List of gods
│   │   │   ├── GodSearch.tsx    # Search interface
│   │   │   ├── GodForm.tsx      # Add/edit god form
│   │   │   ├── MythologyList.tsx# List of mythologies
│   │   │   ├── Layout.tsx       # App shell/layout
│   │   │   └── ErrorBoundary.tsx# Error handling
│   │   ├── pages/               # Page-level components
│   │   │   ├── HomePage.tsx     # Landing page
│   │   │   ├── GodsPage.tsx     # Gods listing/search
│   │   │   ├── GodDetailPage.tsx# Single god detail
│   │   │   └── MythologiesPage.tsx # Mythologies listing
│   │   ├── types/               # TypeScript type definitions
│   │   │   └── models.ts        # God, Mythology, Alias, GodInput types
│   │   ├── App.tsx              # Root component with routing
│   │   ├── main.tsx             # Entry point
│   │   └── index.css            # Global styles
│   ├── index.html               # HTML entry point
│   ├── package.json             # Node dependencies
│   ├── tsconfig.json            # TypeScript configuration
│   ├── vite.config.ts           # Vite configuration (incl. API proxy)
│   └── README.md                # Frontend-specific documentation
├── Common/                      # Existing .NET code
├── Endpoints/                   # Existing .NET code
├── Gods/                        # Existing .NET code
├── Mythologies/                 # Existing .NET code
├── Program.cs                   # Existing .NET entry point
└── MythApi.csproj               # Existing .NET project
```

---

## 5. API Integration Strategy

### Development Proxy

During development, the Vite dev server (typically `http://localhost:5173`) will proxy API requests to the .NET backend (`http://localhost:5280`).

**vite.config.ts** should configure:
```ts
export default defineConfig({
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5280',
        changeOrigin: true,
      }
    }
  }
})
```

### Production Serving Options

| Option | Description | Pros | Cons |
|--------|-------------|------|------|
| **A: Static files via .NET** | Build frontend, copy to `wwwroot/`, serve with `UseStaticFiles()` | Single deployment unit, simple | Couples frontend/backend releases |
| **B: Separate container** | Build frontend into an nginx container | Independent scaling/deployment | More infrastructure to manage |
| **C: CDN/Static hosting** | Deploy built frontend to Azure Static Web Apps or similar | Best performance, independent | CORS configuration needed, separate hosting |

**Recommendation:** Start with **Option A** (static files via .NET) for simplicity, as it requires minimal infrastructure changes. Migrate to Option B or C when scaling demands arise.

### CORS Considerations

- If serving from the same origin (Option A): No CORS changes needed.
- If serving separately (Option B/C): Add CORS middleware in `Program.cs`:
  ```csharp
  builder.Services.AddCors(options =>
  {
      options.AddDefaultPolicy(policy =>
      {
          policy.WithOrigins("http://localhost:5173") // dev
                .AllowAnyHeader()
                .AllowAnyMethod();
      });
  });
  // ...
  app.UseCors();
  ```

---

## 6. Component Architecture

### Page Components

| Page | Route | API Calls | Description |
|------|-------|-----------|-------------|
| `HomePage` | `/` | None | Landing page with navigation to gods/mythologies |
| `GodsPage` | `/gods` | `GET /api/v1/gods` | Browse all gods with search capability |
| `GodDetailPage` | `/gods/:id` | `GET /api/v1/gods/{id}` | Detailed view of a single god with aliases |
| `MythologiesPage` | `/mythologies` | `GET /api/v1/mythologies` | Browse all mythologies with their gods |

### Shared Components

| Component | Responsibility |
|-----------|---------------|
| `Layout` | App header, navigation, footer |
| `GodCard` | Display a single god's name, description, mythology |
| `GodList` | Render a list of `GodCard` components |
| `GodSearch` | Search input with debounce, calls search endpoint |
| `GodForm` | Form for creating/editing gods (POST endpoint) |
| `MythologyList` | Display mythologies with nested gods |
| `ErrorBoundary` | Graceful error handling and display |

### TypeScript Type Definitions

```ts
// src/frontend/src/types/models.ts

export interface Alias {
  id: number;
  godId: number;
  name: string;
}

export interface God {
  id: number;
  name: string;
  description: string;
  mythologyId: number;
  aliases: Alias[];
}

export interface GodInput {
  id?: number;
  name: string;
  description: string;
  mythologyId: number;
}

export interface Mythology {
  id: number;
  name: string;
  gods: God[];
}
```

---

## 7. Testing Strategy

### Unit Tests
- **Framework:** Vitest + React Testing Library
- **Scope:** Component rendering, user interactions, API client mocking
- **Key tests:**
  - `GodList` renders correct number of god cards
  - `GodSearch` triggers API call with debounce
  - `GodForm` validates required fields before submission
  - `GodDetailPage` displays aliases correctly
  - Error states render appropriate messages

### Integration/E2E Tests (Optional Phase 2)
- **Framework:** Playwright or Cypress
- **Scope:** Full user flows against running API
- **Key flows:**
  - Browse gods → click detail → view aliases
  - Search for god by name
  - Create a new god via form

### Test Commands
```bash
# Unit tests
cd src/frontend && npm test

# Unit tests with coverage
cd src/frontend && npm run test:coverage

# E2E tests (Phase 2)
cd src/frontend && npm run test:e2e
```

---

## 8. Impact on Existing Infrastructure

### 8.1 Docker (`src/Dockerfile`)

The Dockerfile needs a multi-stage addition to build the frontend:

```dockerfile
# --- Frontend build stage ---
FROM node:20-alpine AS frontend-build
WORKDIR /app/frontend
COPY src/frontend/package*.json ./
RUN npm ci
COPY src/frontend/ .
RUN npm run build

# --- .NET build stage (existing, modified) ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY src/*.csproj ./
RUN dotnet restore
COPY . .
# Copy built frontend into wwwroot
COPY --from=frontend-build /app/frontend/dist ./src/wwwroot/
RUN dotnet publish -c Release -o out

# --- Runtime stage (existing) ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "MythApi.dll"]
```

### 8.2 .NET Program.cs Changes

Add static file serving for the built frontend:

```csharp
// After app is built
app.UseDefaultFiles();  // Serves index.html for root path
app.UseStaticFiles();   // Serves files from wwwroot/
```

### 8.3 CI/CD Pipeline (`.github/workflows/test.yml`)

Add a frontend build and test job:

```yaml
jobs:
  frontend:
    name: Frontend Build and Test
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    - name: Setup Node.js
      uses: actions/setup-node@v4
      with:
        node-version: 20
        cache: 'npm'
        cache-dependency-path: src/frontend/package-lock.json
    - name: Install dependencies
      run: npm ci
      working-directory: src/frontend
    - name: Lint
      run: npm run lint
      working-directory: src/frontend
    - name: Test
      run: npm test -- --run
      working-directory: src/frontend
    - name: Build
      run: npm run build
      working-directory: src/frontend
```

### 8.4 `.gitignore` Additions

```gitignore
# Frontend
src/frontend/node_modules/
src/frontend/dist/
src/frontend/.env.local
```

### 8.5 Solution File (`MythApi.sln`)

No changes needed — the frontend is a separate Node.js project and doesn't need to be part of the .NET solution.

### 8.6 `.NET Project File` (`src/MythApi.csproj`)

If using Option A (static files via .NET), add a build target to copy frontend dist into `wwwroot/` during local development:

```xml
<ItemGroup>
  <Content Include="wwwroot\**" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

---

## 9. Implementation Steps

### Phase 1: Project Scaffolding & Core Pages (Estimated: 2-3 days)

| Step | Task | Acceptance Criteria |
|------|------|---------------------|
| 1.1 | Scaffold Vite + React + TypeScript project in `src/frontend/` | `npm run dev` starts dev server; `npm run build` produces `dist/` |
| 1.2 | Define TypeScript types matching API models | Types for `God`, `Mythology`, `Alias`, `GodInput` exist |
| 1.3 | Create API client layer | Functions for all 5 API endpoints with proper typing |
| 1.4 | Configure Vite proxy for development | API calls from dev server reach .NET backend |
| 1.5 | Create `Layout` component with navigation | Header with nav links to Gods and Mythologies |
| 1.6 | Create `GodsPage` with `GodList` | Displays all gods fetched from API |
| 1.7 | Create `GodDetailPage` | Displays god details and aliases by ID |
| 1.8 | Create `MythologiesPage` | Displays all mythologies with nested gods |
| 1.9 | Add routing (React Router) | Navigation between pages works |
| 1.10 | Add basic CSS/styling | UI is presentable and responsive |

### Phase 2: Search & CRUD (Estimated: 1-2 days)

| Step | Task | Acceptance Criteria |
|------|------|---------------------|
| 2.1 | Create `GodSearch` component | Search input calls `/api/v1/gods/search/{name}` |
| 2.2 | Add "include aliases" toggle to search | Query parameter `includeAliases` is sent |
| 2.3 | Create `GodForm` component | Form posts to `POST /api/v1/gods` |
| 2.4 | Add form validation | Required fields validated before submission |
| 2.5 | Add success/error feedback | User sees confirmation or error after form submission |

### Phase 3: Testing (Estimated: 1-2 days)

| Step | Task | Acceptance Criteria |
|------|------|---------------------|
| 3.1 | Set up Vitest + React Testing Library | `npm test` runs and passes |
| 3.2 | Write unit tests for API client (mocked) | All API functions have tests |
| 3.3 | Write component tests | All components render correctly with mock data |
| 3.4 | Write page-level tests | Pages fetch data and display correctly |
| 3.5 | Add test coverage reporting | Coverage report generated on `npm run test:coverage` |

### Phase 4: Integration & Deployment (Estimated: 1-2 days)

| Step | Task | Acceptance Criteria |
|------|------|---------------------|
| 4.1 | Update `Program.cs` with static file serving | Frontend served from .NET in production |
| 4.2 | Update `Dockerfile` with frontend build stage | Docker image includes built frontend |
| 4.3 | Update CI workflow with frontend job | Frontend builds and tests in CI |
| 4.4 | Update `.gitignore` | `node_modules/` and `dist/` excluded |
| 4.5 | Update `README.md` with frontend instructions | Development and build instructions documented |

---

## 10. Dependencies & New Packages

### Core Dependencies
```json
{
  "dependencies": {
    "react": "^18.x",
    "react-dom": "^18.x",
    "react-router-dom": "^6.x"
  },
  "devDependencies": {
    "@types/react": "^18.x",
    "@types/react-dom": "^18.x",
    "@vitejs/plugin-react": "^4.x",
    "typescript": "^5.x",
    "vite": "^5.x",
    "vitest": "^1.x",
    "@testing-library/react": "^14.x",
    "@testing-library/jest-dom": "^6.x",
    "eslint": "^8.x",
    "eslint-plugin-react-hooks": "^4.x"
  }
}
```

### No changes to .NET dependencies
The existing `MythApi.csproj` already supports static file serving via the ASP.NET Core framework — no new NuGet packages are required.

---

## 11. Risks & Mitigations

| Risk | Impact | Likelihood | Mitigation |
|------|--------|------------|------------|
| API contract changes break frontend | High | Medium | Define TypeScript types matching C# models; add contract tests |
| CORS issues during development | Low | Low | Vite proxy eliminates CORS in dev; document CORS setup for production |
| Node.js version conflicts in CI | Medium | Low | Pin Node.js version in CI workflow and document in README |
| Frontend build increases Docker image size | Low | High | Multi-stage Docker build keeps runtime image small |
| SPA routing conflicts with API routes | Medium | Low | All API routes are under `/api/`, frontend catches all other routes |

---

## 12. Open Questions

1. **Styling approach:** Should we use a CSS framework (e.g., Tailwind CSS, Bootstrap) or custom CSS?
2. **State management:** Is React's built-in `useState`/`useContext` sufficient, or do we anticipate needing Redux/Zustand?
3. **Authentication:** The API currently has no auth. If auth is added later, the frontend will need login/token management.
4. **SEO requirements:** If SEO matters, consider Next.js (SSR) instead of a plain Vite SPA.
5. **Browser support:** What is the minimum browser version to target?

---

## 13. Summary

Adding a frontend to MythAPI is a moderate effort (estimated **5-9 days** total across all phases). The key decisions are:

- **Technology:** React + Vite + TypeScript (recommended)
- **Location:** `src/frontend/`
- **Serving strategy:** Static files via .NET (`wwwroot/`) for simplicity
- **Infrastructure impact:** Dockerfile update, CI workflow addition, `.gitignore` update, `Program.cs` static file middleware

The existing API is well-structured with clear endpoints and typed models, making frontend integration straightforward. The domain-driven backend structure and versioned API routes (`/api/v1/`) provide a clean separation that maps naturally to frontend API client modules.
