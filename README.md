# Product Classification Project

## Overview  
This project utilizes a dataset from [Kaggle](https://www.kaggle.com/competitions/retail-products-classification/data), which includes various categories, product titles, and product descriptions. The objective is to accurately determine the product category based on the description while providing multiple options for selecting LLM models. The evaluation results for each LLM model are stored in a PostgreSQL database.

## New Features  
- **RAG Implementation:** Introduces a new chatbot that leverages the dataset to provide contextual, data-driven responses.

## Setup  
1. To run the project locally, ensure that you have Docker Desktop or Docker Engine installed on your system.  
2. Edit the `appsettings.json` file with your OpenAI API key and Gemini Key. If you wish to remove any LLM model, delete it from the configuration file and remove its references in `Services/AIConnectorService.cs`.  
3. Run the following commands to add the user secrets to the `ProductClassification` project:
    ```bash
    dotnet user-secrets set "Parameters:postgres-password" user
    dotnet user-secrets set "parameters:postgres-user" user
    ```
4. Open Visual Studio, select the **ProductClassification.AppHost** project as the startup project, and run the solution. Aspire will automatically download the images and establish the necessary connections for you.