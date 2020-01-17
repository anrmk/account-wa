/****** Script for SelectTopNRows command from SSMS  ******/
SELECT *  FROM [accountWa].[dbo].[Payments]
SELECT *  FROM [accountWa].[dbo].[Invoices]
SELECT *  FROM [accountWa].[dbo].[Customers] WHERE Id=15  

SELECT CUS.[AccountNumber], CUS.[Name], INV.[Customer_Id] AS CustomerId, INV.[Id], INV.[No], 
CAST(INV.[Subtotal] * (1+INV.[TaxRate]/100) as decimal(10,2)) AS [Amount], INV.[Date], INV.[DueDate], PAY.[Id] AS PayId, PAY.[Amount] AS PayAmount, PAY.[Date] AS PayDate,
DATEDIFF(DAY, INV.[DueDate], CURRENT_TIMESTAMP ) AS DateDiff 
FROM [accountWa].[dbo].[Invoices] AS INV
LEFT JOIN  [accountWa].[dbo].[Payments] AS PAY
ON PAY.[Invoice_Id] = INV.[Id]
LEFT JOIN [accountWa].[dbo].Customers as CUS
ON CUS.[Id] = INV.[Customer_Id]
WHERE INV.[Company_Id] = 1 AND INV.[DueDate] >= '2019-09-16'
ORDER BY [CustomerId], [Date]