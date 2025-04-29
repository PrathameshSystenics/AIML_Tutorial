# Aspire Tutorial

This is the Sample Project for using Aspire Related Tutorial or Experiment new things on these project.

## Contents
1. How to Add the Custom Docker Container
2. Adding User Secrets without Storing it into the `appsettings.json`.
3. Dapr PUb sub Resource
4. Adding .NET Projects for Aspire Orchestration
5. Adding Postgres Database with PG Admin Support.
6. Custom Resource Events
7. Custom Container with Volume, Entrypoint scripts.
8. Migration Service
9. LifeCycle Hook.
10. Adding Custom Environment Variable
11. Dapr SideCar to the Project.
12. Creating the Docker-compose.yml file from the Aspire project using Aspirat8. 

## Running Locally
Just need to config the User Secrets.
```json
{
  "Parameters:postgres-username": "user",
  "Parameters:postgres-password": "user",
  "AppHost:OtlpApiKey": "2cf2298f966b5944b8e8af8af934f288"
}
```
Just Paste the Above Json File into the user secrets.

Select the `TinyShop.AppHost` as the Startup project and run the project. 
It will automatically pull the image and create the container but make sure you have docker desktop installed in your PC. 