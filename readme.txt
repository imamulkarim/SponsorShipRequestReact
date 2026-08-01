Starting with version 8, this specific template was completely streamlined to focus exclusively on Web APIs using the FastEndpoints library. It deliberately removed all server-rendered pages, MVC views, and front-end framework configurations from the core output

Use Jason Taylor's Clean Architecture Template

1. Install the template:
dotnet new install Clean.Architecture.Solution.Template

	For Angular: dotnet new ca-sln --ui Angular -o YourProjectName
	For React: dotnet new ca-sln --ui React -o YourProjectName
But this cli command throws a error that there is no `--ui` command
So I create a project using visual studio Template wizard of project creation.


2. Manually Connect an SPA 
npm create vite@latest frontend-react -- --template react-ts


# NSwag Configuration
 dotnet tool install --global NSwag.ConsoleCore
nswag version

Create or update an NSwag Configuration
nswag.json
review the : swagger or openapi url path ../wwwroot/openapi/v1.json

Step 4: Generate the Client
nswag run nswag.json