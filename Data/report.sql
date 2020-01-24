/***** SYNC CUStoMER ACTIVITIES****/
SELECT *  FROM [accountWa].[dbo].[CustomerActivities]
truncate table 	[accountWa].[dbo].[CustomerActivities]

/***** SYNC CUStOMER ACTIVITIES****/
INSERT INTO [accountWa].[dbo].[CustomerActivities] (
  [Customer_Id]	,
  [CreatedDate],
  [IsActive]
) SELECT [Id], CURRENT_TIMESTAMP, 'True' FROM [accountWa].[dbo].[Customers]

/***** AGING REPORT SELECT ****/
SELECT 
  CUS.[AccountNumber], CUS.[Name], 
  INV.[Customer_Id] AS CustomerId, INV.[Id], INV.[No], DATEDIFF(DAY, INV.[DueDate], '2019-11-30' ) AS DiffDate,    
  CAST(INV.[Subtotal] * (1+INV.[TaxRate]/100) as decimal(10,2)) AS [Amount], INV.[Date], INV.[DueDate],  
  PAY.[Id] AS PayId, PAY.[Amount] AS PayAmount, PAY.[Date] AS PayDate
FROM [accountWa].[dbo].[Invoices] AS INV
LEFT JOIN  [accountWa].[dbo].[Payments] AS PAY
ON PAY.[Invoice_Id] = INV.[Id]
LEFT JOIN [accountWa].[dbo].Customers as CUS
ON CUS.[Id] = INV.[Customer_Id]
LEFT JOIN [accountWa].[dbo].Companies as COM
ON COM.[Id] = INV.[Customer_Id]
WHERE INV.[Company_Id] = 1 AND INV.[DueDate] >= DATEADD(day, 30*4*-1,  '2019-11-30')
--AND CUS.[AccountNumber] = 'CN-001972' 
ORDER BY [DiffDate], [Amount]


/***** CREATE NEW INVOICES WITH DATA BETWEEN X and Y ****/
DECLARE @date_from DATETIME;
DECLARE @date_to DATETIME;
SET @date_from = '2019-12-01';
SET @date_to = '2019-12-31';

INSERT INTO [accountWa].[dbo].[Payments] (
  [CreatedDate],
  [UpdatedDate],
  [Date],
  [Invoice_Id],
  [Amount],
  [Ref]) 
SELECT 
	CURRENT_TIMESTAMP AS 'CreatedDate', 
	CURRENT_TIMESTAMP AS 'UpdatedDate',
	dateadd(day, rand(checksum(newid()))*(1+datediff(day, @date_from, @date_to)), @date_from) AS 'Date',
	INV.[Id] AS 'Invoice_Id',
	INV.[Subtotal] as 'Amount',
	CONCAT('PMT_', INV.Id) AS 'Ref'
FROM [accountWa].[dbo].[Invoices] AS INV
LEFT JOIN [accountWa].[dbo].[Customers] AS CUS
ON CUS.[Id] = INV.[Customer_Id]
WHERE [AccountNumber] NOT IN ('CN-000690',
'CN-000181','CN-000182','CN-000183','CN-000245','CN-000303','CN-000333','CN-000354','CN-000369',
'CN-000424','CN-000448','CN-000455','CN-000465','CN-000469','CN-000478','CN-000513','CN-000525',
'CN-000545','CN-000556','CN-000572','CN-000574','CN-000578','CN-000581','CN-000613','CN-000617',
'CN-000631','CN-000634','CN-000643','CN-000651','CN-000652','CN-000671','CN-000675','CN-000682',
'CN-000684','CN-000694','CN-000712','CN-000753','CN-000772','CN-000783','CN-000818','CN-000845',
'CN-000861','CN-000872','CN-000874','CN-000887','CN-000889','CN-000894','CN-000913','CN-000916',
'CN-000933','CN-000968','CN-001045','CN-001126','CN-001144','CN-001177','CN-001280','CN-001322',
'CN-001334', 'CN-001365','CN-001385', 'CN-001386','CN-001417', 'CN-001437','CN-001485', 'CN-001499',
'CN-001514', 'CN-001574', 'CN-001575')
AND INV.[Date] < '2019-12-1'; 