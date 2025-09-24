# Timora API

**Dotnet Version:** 9.0.304  
**Deployed at:** [*TimoraBE on Azure*](https://timorabe.azurewebsites.net/)  
**Timora FE:** [*TimoraFE*](https://github.com/kollarm226/timoraFE)

## Getting Started

### Prerequisites
- .NET 9.0 SDK or later

### CLI Commands

#### Build the solution
```bash
dotnet build Timora.sln
```

#### Run the API (Development)
```bash
dotnet run --project Timora.Api
```
The API will be available at `https://localhost:5001` and `http://localhost:5000`. 
Swagger UI will be accessible at `https://localhost:5001/swagger`.

#### Run Tests
```bash
dotnet test Timora.Tests
```

#### Run All Tests with Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```
---
