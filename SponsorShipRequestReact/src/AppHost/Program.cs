using TechAssessment.Shared;

var builder = DistributedApplication.CreateBuilder(args);

//"Hey Aspire, when you deploy this application to the cloud, deploy it to Azure Container Apps,
//and I'm naming the container environment 'aca-env'."
//builder.AddAzureContainerAppEnvironment("aca-env");

//var databaseServer = builder
//    .AddAzurePostgresFlexibleServer(Services.DatabaseServer)
//    .WithPasswordAuthentication()
//    .RunAsContainer(container => 
//        container.WithLifetime(ContainerLifetime.Persistent))
//    .AddDatabase(Services.Database);

var serviceBusConnection = builder.AddConnectionString(Services.ServiceBusConnection);

#region example of using a SQL server container

/*
    var password = builder.AddParameter("password", secret: true);

    var sql = builder.AddSqlServer(Services.DatabaseServer)
        .WithImageTag("2025-RTM-ubuntu-24.04-preview")
        .WithPassword(password)
        .WithDataVolume()
        .WithLifetime(ContainerLifetime.Persistent);


    var creationScript = $$"""
    IF DB_ID('{{Services.Database}}') IS NULL
        CREATE DATABASE [{{Services.Database}}];
    GO
    """;

    var db = sql.AddDatabase(Services.Database)
        .WithCreationScript(creationScript);

    builder.AddDockerComposeEnvironment("env");

    var web = builder.AddProject<Projects.Web>(Services.WebApi)
        .WithReference(db)
        //.WithEnvironment( context =>
        //{
        //    context.EnvironmentVariables["SQL_HOST"] = sql.Resource.PrimaryEndpoint.Property(EndpointProperty.Host);
        //    context.EnvironmentVariables["SQL_PORT"] = sql.Resource.PrimaryEndpoint.Property(EndpointProperty.Port);
        //    context.EnvironmentVariables["SQL_PASSWORD"] = sql.Resource.PasswordParameter;
        //    context.EnvironmentVariables["SQL_DATABASE"] = sql.Resource.Databases.FirstOrDefault();
        //})
        .WaitFor(db)
        .WithExternalHttpEndpoints()
        .WithAspNetCoreEnvironment()
        .WithUrlForEndpoint("http", url =>
        {
            url.DisplayText = "Scalar API Reference";
            url.Url = "/scalar";
        });

    if (builder.ExecutionContext.IsRunMode)
    {
        builder.AddJavaScriptApp(Services.WebFrontend, "./../Web/ClientApp")
            .WithRunScript("start")
            .WithReference(web)
            .WaitFor(web)
            .WithHttpEndpoint(env: "PORT")
            .WithExternalHttpEndpoints();
    }*/
#endregion

#region  example of using a SQL connection string from Aspire Host application configuration
var sql = builder.AddConnectionString("TechAssessmentDb");

var web = builder.AddProject<Projects.Web>(Services.WebApi)
    .WithExternalHttpEndpoints()
    .WithReference(sql)
    .WithReference(serviceBusConnection)
    .WithAspNetCoreEnvironment()
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar API Reference";
        url.Url = "/scalar";
    });

if (builder.ExecutionContext.IsRunMode)
{
    builder.AddJavaScriptApp(Services.WebFrontend, "./../Web/ClientApp")
        .WithRunScript("start")
        .WithReference(web)
        .WaitFor(web)
        .WithHttpEndpoint(env: "PORT")
        .WithExternalHttpEndpoints();
}

#endregion

#region  example of using a Azure SQL connection  from Aspire Host application configuration

//This provision or create new resources
//var sql = builder.AddAzureSqlServer(builder.Configuration[Services.DatabaseServer]!);
//var db = sql.AddDatabase(builder.Confiuration[Services.Database]!);

//use existing
//var existingName = builder.AddParameter(Services.DatabaseServer);

//var existingResourceGroup = builder.AddParameter("resourceGroup");

//var sql = builder.AddAzureSqlServer("sql")
//    .AsExisting(existingName, existingResourceGroup)
//    .AddDatabase(Services.Database);

//var web = builder.AddProject<Projects.Web>(Services.WebApi)
//    .WithExternalHttpEndpoints()
//    .WithReference(sql)
//    .WithReference(serviceBusConnection)
//    .WithAspNetCoreEnvironment()
//    .WithUrlForEndpoint("http", url =>
//    {
//        url.DisplayText = "Scalar API Reference";
//        url.Url = "/scalar";
//    });

//if (builder.ExecutionContext.IsRunMode)
//{
//    builder.AddJavaScriptApp(Services.WebFrontend, "./../Web/ClientApp")
//        .WithRunScript("start")
//        .WithReference(web)
//        .WaitFor(web)
//        .WithHttpEndpoint(env: "PORT")
//        .WithExternalHttpEndpoints();
//}

#endregion

//var web = builder.AddProject<Projects.Web>(Services.WebApi)
//    .WithReference(databaseServer)
//    .WaitFor(databaseServer)
//    .WithExternalHttpEndpoints()
//    .WithAspNetCoreEnvironment()
//    .WithUrlForEndpoint("http", url =>
//    {
//        url.DisplayText = "Scalar API Reference";
//        url.Url = "/scalar";
//    });

//if (builder.ExecutionContext.IsRunMode)
//{
//    builder.AddJavaScriptApp(Services.WebFrontend, "./../Web/ClientApp")
//        .WithRunScript("start")
//        .WithReference(web)
//        .WaitFor(web)
//        .WithHttpEndpoint(env: "PORT")
//        .WithExternalHttpEndpoints();
//}

builder.Build().Run();
