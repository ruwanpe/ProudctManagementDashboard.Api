--1) Validate total stock quantity per category.
--SELECT Category, SUM(StockQuantity) as TotalQuantity from Products GROUP BY Category

--2) Validate total number of products added in different time periods
WITH WeekStart AS (
    SELECT date('now', 'weekday 0', '-7 days') AS week_start
),
MonthStart AS (
    SELECT date('now', 'start of month') AS month_start
),
YearStart AS (
    SELECT date('now', 'start of year') AS year_start
)

-- Products added this week
SELECT 
    'This Week' AS Duration, 
    SUM(StockQuantity) AS ProductsAdded
FROM Products, WeekStart
WHERE date(DateAdded) >= week_start

UNION ALL

-- Products added this month
SELECT 
    'This Month' AS Duration,
    SUM(StockQuantity) AS ProductsAdded
FROM Products, MonthStart
WHERE date(DateAdded) >= month_start

UNION ALL

-- Products added this year
SELECT 
    'This Year' AS Duration,
    SUM(StockQuantity) AS ProductsAdded
FROM Products, YearStart
WHERE date(DateAdded) >= year_start;