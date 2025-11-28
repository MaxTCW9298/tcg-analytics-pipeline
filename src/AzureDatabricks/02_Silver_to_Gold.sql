-- -------------------------------------------------------------------------
-- DATABRICKS NOTEBOOK: SILVER TO GOLD (ANALYTICS)
-- -------------------------------------------------------------------------

-- 1. Setup
-- Point to the Delta table created by the Python script
CREATE TABLE IF NOT EXISTS silver_cards 
USING DELTA LOCATION '/mnt/datalake/silver/cards_delta';

-- 2. Business Logic: Daily Volatility
-- Find cards that moved significantly in price compared to yesterday.
CREATE OR REPLACE TABLE gold_market_volatility
USING DELTA
LOCATION '/mnt/datalake/gold/market_volatility'
AS
WITH DailyPrices AS (
    SELECT 
        card_id,
        card_name,
        set_name,
        ingestion_date,
        -- Prioritize Holo price, fallback to Normal
        COALESCE(price_market_holo, price_market_normal, 0) as price
    FROM silver_cards
)
SELECT 
    curr.card_name,
    curr.set_name,
    curr.price as current_price,
    curr.ingestion_date,
    prev.price as yesterdays_price,
    -- Calculate Percentage Change
    ROUND(((curr.price - prev.price) / prev.price) * 100, 2) as pct_change
FROM DailyPrices curr
JOIN DailyPrices prev 
    ON curr.card_id = prev.card_id 
    AND curr.ingestion_date = DATE_ADD(prev.ingestion_date, 1) -- Join with yesterday
WHERE prev.price > 0
ORDER BY pct_change DESC;

-- 3. Optimization
OPTIMIZE gold_market_volatility ZORDER BY (ingestion_date);