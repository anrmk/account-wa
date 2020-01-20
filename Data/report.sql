/****** Script for SelectTopNRows command from SSMS  ******/
SELECT *  FROM [accountWa].[dbo].[Payments]
SELECT *  FROM [accountWa].[dbo].[Invoices]
SELECT *  FROM [accountWa].[dbo].[Customers] WHERE Id=15  

select * from Invoices 
left join Customers
on Invoices.Customer_Id = Customers.Id;

SELECT 
  CUS.[AccountNumber], CUS.[Name], 
  INV.[Customer_Id] AS CustomerId, INV.[Id], INV.[No], DATEDIFF(DAY, INV.[DueDate], CURRENT_TIMESTAMP ) AS DateDiff,    
  CAST(INV.[Subtotal] * (1+INV.[TaxRate]/100) as decimal(10,2)) AS [Amount], INV.[Date], INV.[DueDate],  
  PAY.[Id] AS PayId, PAY.[Amount] AS PayAmount, PAY.[Date] AS PayDate
FROM [accountWa].[dbo].[Invoices] AS INV
LEFT JOIN  [accountWa].[dbo].[Payments] AS PAY
ON PAY.[Invoice_Id] = INV.[Id]
LEFT JOIN [accountWa].[dbo].Customers as CUS
ON CUS.[Id] = INV.[Customer_Id]
WHERE INV.[Company_Id] = 1 AND INV.[DueDate] >= '2020-12-05' --AND CUS.[AccountNumber] = 'CN-000152' 
ORDER BY [CustomerId], [Date]