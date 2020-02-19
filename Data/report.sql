﻿/***** SYNC CUStOMER ACTIVITIES****/
--INSERT INTO [accountWa].[dbo].[CustomerActivities] (
--  [Customer_Id]	,
--  [CreatedDate],
--  [IsActive]
--) SELECT [Id], [CreatedDate], 'True' FROM [accountWa].[dbo].[Customers] where [Company_Id] = 1

/***** AGING REPORT SELECT ****/
DECLARE @DATETO DATETIME;
DECLARE @DATEFROM DATETIME;
DECLARE @COMPANYID INT;
DECLARE @daysPerPeriod INT;
DECLARE @numberOfPeriod INT;

SET @daysPerPeriod 	= 30;
SET @numberOfPeriod	= 4;

SET @DATETO = '2019-12-31';
SET @DATEFROM = DATEADD(day, @daysPerPeriod*@numberOfPeriod*-1,  @DATETO);
SET @COMPANYID = 1;

SELECT 
  CUS.[AccountNumber], CUS.[Name], 
  INV.[Customer_Id] AS CustomerId, INV.[Id], INV.[No], 
  DATEDIFF(DAY, INV.[DueDate], @DATEFROM ) AS DiffDate,     
  INV.[Subtotal], INV.[TaxRate],
  CAST(INV.[Subtotal] * (1+INV.[TaxRate]/100) as decimal(10,2)) AS [Amount], INV.[Date], INV.[DueDate],  
  PAY.[Id] AS PayId, PAY.[Amount] AS PayAmount, PAY.[Date] AS PayDate
FROM [accountWa].[dbo].[Invoices] AS INV
LEFT JOIN  [accountWa].[dbo].[Payments] AS PAY
ON PAY.[Invoice_Id] = INV.[Id] AND PAY.[Date] <= @DATETO
LEFT JOIN [accountWa].[dbo].Customers as CUS
ON CUS.[Id] = INV.[Customer_Id]
LEFT JOIN [accountWa].[dbo].Companies as COM
ON COM.[Id] = INV.[Customer_Id]
WHERE INV.[Company_Id] = @COMPANYID AND INV.[DueDate] >= @DATEFROM AND INV.[Date] <= @DATETO
ORDER BY INV.[Subtotal] DESC


--delete [accountWa].[dbo].[Invoices] where IsDraft =1

--update [accountWa].[dbo].[Invoices] set CreatedBy='nov_2019', Company_Id=10002 where IsDraft = 1
--update [accountWa].[dbo].[Invoices] set IsDraft = 0	where [Company_Id] = 10002

--SELECT INV.[No], INV.[Subtotal], INV.[Customer_Id], INV.[IsDraft], CUS.[Company_Id], CUS.[AccountNumber] 
--FROM [accountWa].[dbo].[Invoices] AS INV
--LEFT JOIN [accountWa].[dbo].[Customers] AS CUS
--ON CUS.[Id] = INV.[Customer_Id]
--WHERE INV.[Date] > '2019-10-30' AND  INV.[Date] < '2019-11-30' 
--AND CUS.[Company_Id] = 2

/***** INSERT CUSTOMERS ACTIVITIES ****/
--INSERT INTO [accountWa].[dbo].[CustomerActivities] (
  --[Customer_Id],
  --[CreatedDate],
  --[IsActive])
SELECT 
	[Id],
	[CreatedDate],
	'True'
FROM [accountWa].[dbo].[Customers] 
WHERE [Company_Id] = 2


/***** INSERT PAYMENTS BETWEE TWO DATES ****/
SELECT * FROM [accountWa].[dbo].[Payments] 

DECLARE @pdate_from DATETIME;
DECLARE @pdate_to DATETIME;
SET @pdate_from = '2019-11-1';
SET @pdate_to = '2019-11-30';
INSERT INTO [accountWa].[dbo].[Payments] (
	[Invoice_Id],
	[No],
	[Amount],
	[Date],
	[CreatedDate],
	[UpdatedDate],
	[IsDraft]
)
SELECT 
	[Id] AS 'Invoice_Id', 
	[No] AS 'Ref',
	[Subtotal] AS 'Amount',
	dateadd(day, rand(checksum(newid()))*(1+datediff(day, @pdate_from, @pdate_to)), @pdate_from) AS 'Date',
	CURRENT_TIMESTAMP,
	CURRENT_TIMESTAMP,
	'True' AS 'IsDraft'
FROM [accountWa].[dbo].[Invoices] WHERE [Date] <= @pdate_from AND
[Customer_Id] IN (
	SELECT [Id] FROM [accountWa].[dbo].[Customers]
	WHERE [AccountNumber] IN (
'168144',
'168145',
'169779',
'169780')
)



use accountWa;

SELECT CUS.[Id], INV.[Total], CUS.[AccountNumber], CUS.[Name], 
CUS.[Description], CUS.[Terms], CUS.[CreditLimit], 
CUS.[CreditUtilized], CUS.[Company_Id]

FROM [dbo].[Customers] AS CUS 
LEFT JOIN (SELECT Customer_Id, COUNT(*) AS [Total] FROM [dbo].[Invoices]
WHERE [Date] > '2020-02-11' AND [DATE] <= '2020-03-12'
group by [Customer_Id]) AS INV 
ON CUS.[Id] = INV.[Customer_Id]
WHERE CUS.[Company_Id] = 2 order by [Name]	