# Aspire.Hosting.SqlServer

https://aspire.dev/integrations/databases/sql-server/sql-server-host/?aspire-lang=csharp
`````````
NuGet\Install-Package Aspire.Hosting.SqlServer -Version 13.4.6
`````````

- Add Entity Framework Core SQL Server provider to the project: Infrastructure

- [ ] Microsoft.EntityFrameworkCore.SqlServer
- [ ] Update the Dependency Injection Class 
```` 
options.UseSqlServer(connectionString);
````

## Option 1: Run using SQL Server Container require a pre configure docker environment
- Recomendded: In that case we need to pass the database info to web api project by Environment Variable. Aspire framework provide a built in support for environment variable, you can set the environment variable in the docker-compose file or in the command line when running the container.
- Connect to SQL Server - https://aspire.dev/integrations/databases/sql-server/sql-server-connect/
    - For Raw SQL - This package provide additional feature you can direct access the SQLClient to access the db - Install the Aspire.Microsoft.Data.SqlClient NuGet package to the project: WebApi (but in clean architecture it will be Infrastructure)
    - For EFCore - This package provide additional feature you can direct access the EFcore SQL - Aspire.Microsoft.EntityFrameworkCore.SqlServer - https://aspire.dev/integrations/databases/efcore/sql-server/sql-server-connect/
    - This is the by default connection method for SQL Server, you can also use the connection string in appsettings.json or secret manager, but using environment variable is more secure and recommended for containerized applications.
        - 
        ````````
        builder.Services.AddDbContext<ExampleDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("database")
        ?? throw new InvalidOperationException("Connection string 'database' not found.")));
        ````````
        - I have used thed third option in `DependencyInjection` class, which is to use the connection string from the configuration, but you can also use the environment variable directly in the code, but it is not recommended for security reasons.
    - I have used `builder.AddParameter("password", secret: true);` becuase with out it after closing docker the password used it was generate random and that passowrd stored in docker volume, as a result in next run it generate a new password and don't matched with the previous one hence the solution is not run.
    - https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-10.0&tabs=windows%2Cpowershell
- ### Now I,m trying to publish the project
    -  1st Docker Publish: https://aspire.dev/get-started/deploy-first-app/?aspire-lang=csharp
     - Configure & Install Aspire CLI - https://aspire.dev/reference/cli/overview/
     - Install script (Windows) >> `irm https://aspire.dev/install.ps1 | iex`  or >>  `winget install Microsoft.Aspire`
     - `aspire --version`
     - Here is a loop hole which cause me some delay, when I run the command
     `aspire add docker` give me an error '❌ Unable to stop one or more running Aspire AppHost instances. Please stop the application and try again.'
     after some look I found that 
       - one reason may be the difference aspire version in CLI and the Aspire Hosting project `<Project Sdk="Aspire.AppHost.Sdk/13.4.6">`
       - and next was the obvious reason for me 'The problem is persistent socket file at C:\Users\imamu\.aspire\cli\bch\BlkyNIS9LD0T3kIM90C.34048. This orphaned file blocks all aspire add docker attempts.'
       - After that I got a successfull message like `✅ The package Aspire.Hosting.Docker::13.4.6 was added successfully.`
       - And you can do that by simply adding a nuget package to the project `Aspire.Hosting.Docker`  to add the docker support to the project.
    - Then follow Update your AppHost section
    - Then run `aspire deploy`
        - Eventually it will asked a paassword for parameter which mentioned in the aspire hosting project : StrongPassw0rd
        - After 3m it will automatically deployed three containers in docker and obviously group them as one
            - aspire-env-388a2de1
                -  dbserver: No public endpoints
                - webapi: http://localhost:49802
                - env-dashboard: http://localhost:49801
        - And it worked fine, and also created a folder aspire-output
        - `aspire destroy`
- ### Aspire deploy in Azure
    - https://aspire.dev/deployment/azure/#deployment-targets
    - I have not tried it yet but I will try it soon, but the process is pretty much same as docker, you need to configure the connection string in Azure App Service or Azure SQL Database and then deploy the application using Aspire CLI.

## Option 2: Run using Local SQL Server
- https://aspire.dev/integrations/databases/sql-server/sql-server-host/?aspire-lang=csharp#connect-to-an-existing-sql-server-instance
- Edit - Manage User Secrets on Visual Studio or use the command line to add the connection string to the secret manager.
- Observer an error: A connection was successfully established with the server, but then an error occurred during the login process. (provider: SSL Provider, error: 0 - The certificate chain was issued by an authority that is not trusted.)'
    - Solution: Add this in connection string -> Encrypt=true;TrustServerCertificate=true;
    - Running ok and work good.
## Option 3: Run using Azure SQL Server
https://aspire.dev/integrations/cloud/azure/azure-sql-database/azure-sql-database-host/?aspire-lang=csharp
- aspire add azure-sql
    - Once again same erorr :  Unable to stop one or more running Aspire AppHost instances. Please stop the application and try again.
    - Deleted the file in following location : C:\Users\imamu\.aspire\cli\bch
- Now it installed the package Aspire.Hosting.AzureSqlServer::13.4.6 successfully.
- Added two info in secret manager
    -   "DatabaseServer": "fazle.database.windows.net",
    - "TechAssessmentDb": "free-sql-db-TechAssessmentDb"
    - And following two line in program.cs
    ````
    var sql = builder.AddAzureSqlServer(Services.DatabaseServer);
    var db = sql.AddDatabase(Services.Database);
    ````
    - Aspire Dashboard ask : Azure provisioningThe model contains Azure resources that require an Azure Subscription.
        - According to the documentation, you need to configure the Azure Subscription in Aspire Dashboard, and then you can deploy the application to Azure.
        - https://aspire.dev/integrations/cloud/azure/local-provisioning/#configuration
        - Added following in secret manager
        ```` 
        "Azure": {
            "SubscriptionId": "ac47110f-6189bc.....",
            "Location": "Southeast Asia"
        }
        ````
        - And It failed to provision the instance : Log message hast 401 status code.
        - I have changed the secret and added the authentiation process
        ````
        "Azure:TenantId": "7097764f-9eea-4c0f-8df5-3b7768536564",
        "Azure:SubscriptionId": "ac47110f-6189-49d1-ba49-f666d612acbc",
        "Azure:ResourceGroup": "DefaultResourceGroup-SEA",
        "Azure:Location": "Southeast Asia",
        "Azure:CredentialSource": "InteractiveBrowser",
        "Azure:AllowResourceGroupCreation": false,
        ````
        - After that it popup browser for authticate but it does not use my existing database
        - after i enter a db server to fazle it provision a new instance and lot of new resources incluing azure container instance, storage, database, etc.
        - So stop at this stage also it failed at Entity framework initiazliaation
    ###  Now I moved to this 'Connect to an existing Azure SQL server' section 
    - because i don't want to provision a new resources
    ````
    var existingName = builder.AddParameter(Services.DatabaseServer);

    var existingResourceGroup = builder.AddParameter("resourceGroup");

    var sql = builder.AddAzureSqlServer("sql")
    .AsExisting(existingName, existingResourceGroup)
    .AddDatabase(Services.Database);
    ````
    - Sql Server Name (sql) is an arbitary name does not require to meet any azure resource.
    - existing name is the server of sql, in my case it is just `fazle` not `fazle.database.windows.net`
    - In secret it auto generate some additional settings when perform a login
    - And database connection is now ok
        
        



Configure the connection in secret manager or appsettings.json.

https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-10.0&tabs=windows%2Cpowershell
