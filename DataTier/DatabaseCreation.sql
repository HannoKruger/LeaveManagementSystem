IF EXISTS (SELECT [name] FROM sysobjects WHERE name = 'Create' AND type = 'P')
DROP PROCEDURE [Create]
GO

CREATE PROCEDURE [Create] @DBName varchar(128), @Name varchar(128), @Params varchar(2000)
AS
BEGIN
declare @table varchar(128),@DB varchar(128),@sql varchar(1000)
select @table = @Name
select @DB = @DBName

select @sql = 'use '+ quotename(@DB)+';
create table ' + quotename(@DB) +'.'+ QUOTENAME('dbo') +'.'+ quotename(@table) + @Params
--print @sql
exec (@sql)
END
GO

Declare @DBName varchar(200);
DECLARE @RelDir varchar(1000);
DECLARE @sql varchar(1500);


--Change this to the location where the database will be created, remember to add a backslash at the end 
SET @RelDir = 'D:\Apps\Microsoft-SQL_Additional_Tools\MSSQL15.MSSQLSERVER\MSSQL\DATA\';							------------------CAN CHANGE---------------

SET @DBName = 'LeaveManagement';                                                                                ------------------CAN CHANGE---------------

--ON
SELECT @sql = 'CREATE DATABASE '+ quotename(@DBName) + ' 
CONTAINMENT = NONE
ON
 (
 NAME = ''' + @DBName + '_DB'', 
 FILENAME = ''' + @RelDir + @DBName + '.mdf'', 
 SIZE = 1MB , MAXSIZE = UNLIMITED, 
 FILEGROWTH = 2MB
 ) 
LOG ON
 (
 NAME = '''+ @DBName + '_Log'', 
 FILENAME = '''+ @RelDir + @DBName  + 'log.ldf'', 
 SIZE = 1MB , MAXSIZE = UNLIMITED , FILEGROWTH = 2MB
 )'



IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = @DBName)
BEGIN
	EXEC (@sql);
END
GO


USE [LeaveManagement]																									------------------CAN CHANGE---------------
GO


Declare @TableName varchar(200);
Declare @FileName varchar(200);
Declare @Cnt int;
Declare @sql varchar(1500);
Declare @RelDir varchar(1000);
Declare @DBName varchar(500);
declare @s nvarchar(1000);
SET DATEFORMAT YMD

SET @DBName = db_name();
--for csv insert
SET @RelDir = 'C:\Users\hanno\OneDrive\Documents\Coding\Github\LeaveManagementSystem\DataTier\csv\'						------------------CAN CHANGE---------------

exec sp_MSforeachtable "declare @name nvarchar(max); set @name = parsename('?', 1); exec sp_MSdropconstraints @name";

----------------------------------------------------------------------------------------------------------------------------------------------------------------------
BEGIN
	set @TableName = 'LeaveType'																		                ------------------CAN CHANGE---------------

	IF EXISTS (SELECT * FROM sysobjects WHERE name = @TableName and xtype='U')
	BEGIN	
		select @sql = 'DROP TABLE ' + @TableName
		exec (@sql)	
	END	
		EXEC [Create] @DBName, @TableName,
		'(							
			LeaveType varchar(500) PRIMARY KEY
		 )';

	set @s = 'select @cnt = (select count(*) from ' + @TableName +')';
	exec sp_executesql @s, N'@cnt nvarchar(20) output', @cnt output;

	IF(@Cnt < 2)
	BEGIN	
		set @FileName = @RelDir+@TableName+'.csv'
		print @filename
		set @sql = 'BULK INSERT ' + @TableName + ' FROM ''' + @FileName + ''' WITH (FIRSTROW = 2, FIELDTERMINATOR ='','', ROWTERMINATOR =''\n'', TABLOCK)';
		EXEC(@sql);
	END
END
----------------------------------------------------------------------------------------------------------------------------------------------------------------------
BEGIN
	set @TableName = 'Employee'																				  ------------------CAN CHANGE---------------

	IF EXISTS (SELECT * FROM sysobjects WHERE name = @TableName and xtype='U')
	BEGIN	
		select @sql = 'DROP TABLE ' + @TableName
		exec (@sql)	
	END	
		EXEC [Create] @DBName, @TableName,
		'(
			EmployeeID INT PRIMARY KEY IDENTITY(1, 1),
			LeaveDaysLeft INT DEFAULT 20,
			FirstName varchar(300),
			LastName varchar(300)
		 )';

	set @s = 'select @cnt = (select count(*) from ' + @TableName +')';
	exec sp_executesql @s, N'@cnt nvarchar(20) output', @cnt output;

	IF(@Cnt < 2)
	BEGIN	
		set @FileName = @RelDir+@TableName+'.csv'
		print @filename
		set @sql = 'BULK INSERT ' + @TableName + ' FROM ''' + @FileName + ''' WITH (FIRSTROW = 2, FIELDTERMINATOR ='','', ROWTERMINATOR =''\n'', TABLOCK)';
		EXEC(@sql);
	END
END
----------------------------------------------------------------------------------------------------------------------------------------------------------------------
BEGIN
	set @TableName = 'Leave'																		                ------------------CAN CHANGE---------------

	IF EXISTS (SELECT * FROM sysobjects WHERE name = @TableName and xtype='U')
	BEGIN	
		select @sql = 'DROP TABLE ' + @TableName
		exec (@sql)	
	END	
		EXEC [Create] @DBName, @TableName,
		'(	
			LeaveID INT PRIMARY KEY IDENTITY(1, 1),
			EmployeeID INT FOREIGN KEY REFERENCES Employee(EmployeeID),
			LeaveType varchar(500) FOREIGN KEY REFERENCES LeaveType(LeaveType),
			StartDate DATE,
			EndDate DATE,	
			DaysTaken INT,
			Reason varchar(5000)
			
		 )';

	set @s = 'select @cnt = (select count(*) from ' + @TableName +')';
	exec sp_executesql @s, N'@cnt nvarchar(20) output', @cnt output;

	IF(@Cnt < 2)
	BEGIN	
		set @FileName = @RelDir+@TableName+'.csv'
		print @filename
		set @sql = 'BULK INSERT ' + @TableName + ' FROM ''' + @FileName + ''' WITH (FIRSTROW = 2, FIELDTERMINATOR ='','', ROWTERMINATOR =''\n'', TABLOCK)';
		EXEC(@sql);
	END
END
----------------------------------------------------------------------------------------------------------------------------------------------------------------------