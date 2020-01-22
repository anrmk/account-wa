/***** SYNC CUStoMER ACTIVITIES****/
SELECT *  FROM [accountWa].[dbo].[CustomerActivities]
truncate table 	[accountWa].[dbo].[CustomerActivities]

INSERT INTO [accountWa].[dbo].[CustomerActivities] (
  [Customer_Id]	,
  [CreatedDate],
  [IsActive]
) SELECT [Id], CURRENT_TIMESTAMP, 'True' FROM [accountWa].[dbo].[Customers]
/***** SYNC CUStoMER ACTIVITIES****/

/****** Script for SelectTopNRows command from SSMS  ******/
SELECT *  FROM [accountWa].[dbo].[Payments]
SELECT *  FROM [accountWa].[dbo].[Invoices]

SELECT * FROM [accountWa].[dbo].[Customers] WHERE [Company_Id] = 1 AND [AccountNumber] = 'CN-001972'

SELECT 
  CUS.[AccountNumber], CUS.[Name], 
  INV.[Customer_Id] AS CustomerId, INV.[Id], INV.[No], DATEDIFF(DAY, INV.[DueDate], '2019-11-30' ) AS DateDiff,    
  CAST(INV.[Subtotal] * (1+INV.[TaxRate]/100) as decimal(10,2)) AS [Amount], INV.[Date], INV.[DueDate],  
  PAY.[Id] AS PayId, PAY.[Amount] AS PayAmount, PAY.[Date] AS PayDate
FROM [accountWa].[dbo].[Invoices] AS INV
LEFT JOIN  [accountWa].[dbo].[Payments] AS PAY
ON PAY.[Invoice_Id] = INV.[Id]
LEFT JOIN [accountWa].[dbo].Customers as CUS
ON CUS.[Id] = INV.[Customer_Id]
LEFT JOIN [accountWa].[dbo].Companies as COM
ON COM.[Id] = INV.[Customer_Id]
WHERE INV.[Company_Id] = 1 AND INV.[DueDate] >= DATEADD(day, 30*4*-1,  '2019-11-30') --AND CUS.[AccountNumber] = 'CN-001972' 
ORDER BY [CustomerId]

1118	 / 1824