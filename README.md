# Product Classification Project

## Overview  
This project utilizes a dataset from [Kaggle](https://www.kaggle.com/competitions/retail-products-classification/data), which includes various categories, product titles, and product descriptions. The objective is to accurately determine the product category based on the description, while providing multiple options for selecting LLM models. The evaluation results for each LLM model are stored in a PostgreSQL database.

## Setup  
1. To run the project locally, ensure that you have Docker Desktop or Docker Engine installed on your system.  
2. Edit the `appsettings.json` file with your OpenAI API key and Gemini Key. If you wish to remove any LLM model, delete it from the configuration file and remove its references in `Services/AIConnectorService.cs`.  
3. Run the project using Docker Compose:
    ```bash
    docker-compose up -d
    ```

## Note  
A better approach to run the project locally is available in the **aspire_integration_productclassification** branch.