Here are step-by-step instructions for running your full application, including backend, frontend, Docker, and tests.
---
1. Prerequisites
•	.NET 9 SDK
•	Docker Desktop
•	Node.js & npm
•	PostgreSQL if not using Docker for DB
---

2. Backend (API)
Run locally:
    - cd src/Retail.Api
    - dotnet run

3. Frontend
position into /retail-ui
    - npm start

 4. Docker 
 From the solution root (where docker-compose.yml is located)
    - docker-compose up --build

5. Running Tests
From the solution root:
    - dotnet test src/Retail.UnitTests
    - dotnet test src/Retail.IntegrationTests

After running the app, Log in with credentials: Username: "RetailAdmin", password: "password"