# tcg-analytics-pipeline
Data engineering project using C#, Azure Event Hub, Databricks, and pandas to analyze trading card game price and card data. Includes ETL pipeline, analytics notebooks, and a .NET API for querying insights.

## Architecture
1. **Ingestion (C#):** Azure Function triggers to fetch JSON from `api.pokemontcg.io` and saves to Data Lake(Bronze Layer).
2. **Processing (PySpark):** Databricks jobs flatten nested JSON and enforce schema types (Silver Layer).
3. **Analytics (SparkSQL):** Window functions calculate daily price volatility and moving averages (Gold Layer).
4. **Serving (.NET API):** REST API exposes top market movers to frontend dashboards.

## Techstack
*   **Azure Databricks** (PySpark, SQL, Delta Lake)
*   **Azure Functions** (C# .NET 8 Isolated Worker)
*   **Azure Data Lake Storage Gen2**
*   **ASP.NET Core Web API**

## Setup
1. Clone repository.
2. Configure `local.settings.json` with Azure Storage Connection Strings.
3. Deploy function to Azure.
4. Import `src/Databricks` to the Databricks Workspace.

## Repository Structure

src/
    IngestionEngine/    # Azure Function (C#) for API Polling
    Databricks/         # ETL Notebooks (Python & SQL)  AnalyticsAPI/       # REST API (ASP.NET Core) serving Gold data
docs/
    architecture.md     # System Design Diagrams