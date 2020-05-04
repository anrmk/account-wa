# Portal
Simple accounting software for internal use in the company

- Install-Package Microsoft.EntityFrameworkCore.Sqlite
- Install-Package Microsoft.EntityFrameworkCore.Tools

## Remote Desktop Connection
89.108.99.44

## Get Aging Invoices
	DECLARE @DATEFROM DATETIME;
	SET @DATEFROM = '2019-2-12';
	DECLARE @DATETO DATETIME;
	SET @DATETO = '2020-3-31';
	DECLARE @COMPANYID INT;
	SET @COMPANYID = 10007;

	SELECT INV.[Id], INV.[No], INV.[Subtotal], INV.[TaxRate], INV.[Date], INV.[DueDate], INV.[IsDraft], 
	PAY.[Id] AS PayId, PAY.[No] AS PayNo, PAY.[Amount] AS PayAmount, PAY.[Date] AS PayDate, 
	CUS.[Id] AS CustomerId, CUS.[AccountNumber] AS CustomerAccountNumber, CUS.[Name] AS CustomerName, CUS.[PhoneNumber] AS CustomerPhoneNumber, CUS.[Terms] AS CustomerTerms,  CUS.[CustomerType_Id] AS CustomerTypeId, CUST.[Name] AS CustomerTypeName,  
	CACT.[Id] AS ActivityId, CACT.[CreatedDate] AS ActivityDate, CACT.[IsActive] AS ActivityStatus, 
	CLIM.[Id] AS CreditLimitId, CLIM.[Value] AS CreditLimit, CLIM.[CreatedDate] AS CreditLimitDate, 
	CUTIL.[Id] AS CreditUtilizedId, CUTIL.[Value] AS CreditUtilized, CUTIL.[CreatedDate] AS CreditUtilizedDate, 
	ADDR.[Id] as CustomerAddressId, ADDR.[Address] as CustomerAddress, ADDR.[Address2] as CustomerAddress2, ADDR.[City] as CustomerCity, ADDR.[State] as CustomerState, ADDR.[ZipCode] as CustomerZipCode, ADDR.[Country] as CustomerCountry, 
	COM.[Id] AS CompanyId, COM.[No] AS CompanyNo, COM.[Name] AS CompanyName, COM.[PhoneNumber] AS CompanyPhoneNumber, 
	DATEDIFF(DAY, INV.[DueDate], @DATEFROM ) AS DiffDate  
	FROM [accountWa].[dbo].[Invoices] AS INV  
	LEFT JOIN [accountWa].[dbo].[Payments] AS PAY ON PAY.[Invoice_Id] = INV.[Id] AND PAY.[Date] <= @DATETO 
	LEFT JOIN [accountWa].[dbo].[Customers] as CUS ON CUS.[Id] = INV.[Customer_Id]  
	LEFT JOIN [accountWa].[dbo].[nsi.CustomerType] as CUST ON CUS.[CustomerType_Id] = CUST.[Id]  
	OUTER APPLY (SELECT TOP 1 * FROM [accountWa].[dbo].[CustomerActivities]  
	   "WHERE [Customer_Id] = CUS.[Id] AND [IsActive] = 'TRUE' AND [CreatedDate] <= @DATETO 
	   "ORDER BY [CreatedDate] DESC) AS CACT 
	OUTER APPLY (SELECT TOP 1 * FROM [accountWa].[dbo].[CustomerCreditLimit] 
	   "WHERE [Customer_Id] = CUS.[Id] AND [CreatedDate] <= @DATETO 
	   "ORDER BY [CreatedDate] DESC) AS CLIM 
	OUTER APPLY (SELECT TOP 1 * FROM [accountWa].[dbo].[CustomerCreditUtilized] 
	   "WHERE [Customer_Id] = CUS.[Id] AND [CreatedDate] <= @DATETO 
	   "ORDER BY [CreatedDate] DESC) AS CUTIL 
	LEFT JOIN [accountWa].[dbo].[CustomerAddresses] as ADDR ON ADDR.[Id] = CUS.[CustomerAddress_Id]  
	LEFT JOIN [accountWa].[dbo].[Companies] as COM ON COM.[Id] = INV.[Company_Id]  
	WHERE INV.[Company_Id] = @COMPANYID AND CACT.[Id] IS NOT NULL AND INV.[Date] <= @DATETO 
	ORDER BY CUS.[AccountNumber] ASC

## Synchronize Customer "CreatedDate" field with first field of customer activity date (CustomerActivity)
	SELECT CACT.[Customer_Id], CUS.[Id], CACT.[CreatedDate], CUS.[CreatedDate], CUS.[AccountNumber], CUS.[Name]
	FROM [accountWa].[dbo].[Customers] AS CUS 
		LEFT JOIN (SELECT Customer_Id, Count(IsActive) AS TotalActive, MIN([CreatedDate]) AS CreatedDate 
				   FROM [accountWa].[dbo].[CustomerActivities]
				   GROUP BY [Customer_Id]) AS CACT 
		ON CACT.[Customer_Id] = CUS.[Id]
	WHERE CACT.[CreatedDate] != CUS.[CreatedDate]

	UPDATE [accountWa].[dbo].[Customers] SET 
		   [accountWa].[dbo].[Customers].[CreatedDate]  = CACT.[CreatedDate]
	FROM [accountWa].[dbo].[Customers] AS CUS 
		LEFT JOIN (SELECT Customer_Id, Count(IsActive) AS TotalActive, MIN([CreatedDate]) AS CreatedDate 
				   FROM [accountWa].[dbo].[CustomerActivities]
				   GROUP BY [Customer_Id]) AS CACT 
		ON CACT.[Customer_Id] = CUS.[Id]
	WHERE CACT.[CreatedDate] != CUS.[CreatedDate]
	
## Remove dublicates from "Payments"
	WITH cte AS (
	    SELECT [Id], [No], [Invoice_Id], [Amount],
		ROW_NUMBER() OVER (
		    PARTITION BY [No], [Invoice_Id], [Amount]
		    ORDER BY [No]) [RowNum]
	    FROM 
		[accountWa].[dbo].[Payments]
	) 
	--DELETE FROM cte WHERE [RowNum] > 1
	SELECT * FROM cte WHERE [RowNum] > 1 ORDER BY [Id];
