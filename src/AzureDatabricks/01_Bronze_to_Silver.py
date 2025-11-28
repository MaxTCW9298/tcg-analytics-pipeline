# -------------------------------------------------------------------------
# DATABRICKS NOTEBOOK: BRONZE TO SILVER (CLEANING)
# -------------------------------------------------------------------------

from pyspark.sql.functions import col, explode, current_timestamp, to_date, from_json
from pyspark.sql.types import StructType, StructField, StringType, DoubleType, ArrayType

# 1. Define Paths
# Widgets/Parameters
bronze_path = "/mnt/datalake/bronze/cards/*/*/*/*.json"
silver_path = "/mnt/datalake/silver/cards_delta"

# 2. Read Raw JSON
# Use recursiveFileLookup to grab all partition folders
raw_df = spark.read.format("json").option("recursiveFileLookup", "true").load(bronze_path)

# 3. Transformation Logic
# The Pokemon API returns a "data" array. Need to get rid of it
# Flatten the nested price structure (holofoil vs normal).
processed_df = (raw_df
    .withColumn("card", explode(col("data"))) # 1 Row per card, not per file
    .select(
        col("card.id").alias("card_id"),
        col("card.name").alias("card_name"),
        col("card.set.name").alias("set_name"),
        col("card.rarity").alias("rarity"),
        # Cast prices to numbers for math
        col("card.tcgplayer.prices.holofoil.market").cast("double").alias("price_market_holo"),
        col("card.tcgplayer.prices.normal.market").cast("double").alias("price_market_normal"),
        col("card.tcgplayer.updatedAt").alias("last_updated_api"),
        current_timestamp().alias("ingestion_timestamp"),
        to_date(current_timestamp()).alias("ingestion_date")
    )
)

# 4. Write to Silver (Delta Lake)
# Partition by set_name to make downstream queries faster
(processed_df.write
    .format("delta")
    .mode("append")
    .partitionBy("set_name")
    .option("mergeSchema", "true") # Allow schema evolution if API changes
    .save(silver_path)
)

print(f"ETL Job Complete. Data written to {silver_path}")