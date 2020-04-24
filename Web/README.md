# Portal

Install-Package Microsoft.EntityFrameworkCore.Sqlite
Install-Package Microsoft.EntityFrameworkCore.Tools

Remote Desktop Connection
89.108.99.44

Administrator 
S5Oh!Eyo79!goA


# Synchronize Customer "CreatedDate" field with first field of customer activity date (CustomerActivity)
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