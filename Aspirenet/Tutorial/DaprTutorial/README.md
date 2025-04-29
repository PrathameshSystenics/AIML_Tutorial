# Dapr tutorial with Aspire

This is a sample project to demonstrate how to use Process Framework along with Dapr and Integration with .NET Aspire

## Running Locally

Before running the Project install the Dapr Cli.

Hit the below command to work with dapr locally. Refer [Init Dapr Locally](https://docs.dapr.io/getting-started/install-dapr-selfhost/)

```bash
dapr init
```

Create new `appsettings.json` file.

To Run these Project you need to Add the below Configuration in the `appsettings.json`

```json
"GeminiModel": {
    "ModelName": "modelname",
    "ApiKey": "api_key"
}
```
Also used SignalR to display the data from Each Step. 

