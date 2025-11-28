# System Architecture

The following diagram illustrates the data flow from the external Pokemon TCG API through the Azure Data Platform to the user-facing API.

```mermaid
graph LR
    subgraph "External"
        API[Pokemon TCG API]
    end

    subgraph "Azure Cloud"
        AF[Azure Functions] -->|Raw JSON| DL_B[(ADLS Gen2\nBronze)]
        
        subgraph "Azure Databricks"
            DL_B -->|PySpark Cleaning| DL_S[(Delta Lake\nSilver)]
            DL_S -->|SparkSQL Aggregation| DL_G[(Delta Lake\nGold)]
        end
        
        DL_G -->|JDBC/ODBC| WEB[ASP.NET Core API]
    end

    WEB -->|JSON Response| USER((End User))
    
    style AF fill:#f9f,stroke:#333,stroke-width:2px
    style DL_S fill:#bbf,stroke:#333,stroke-width:2px
    style DL_G fill:#bfb,stroke:#333,stroke-width:2px