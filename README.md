# AutoMate
AutoMate is a sample web application that forms the starting point for Dylan Beattie's training sessions about building distributed systems with C# and .NET.

## Getting Started 

1. Clone the Git repo
2. Open `dotnet\AutoMate.sln` in Visual Studio
3. Run the `AutoMate.WebApp` project

That should bring up the AutoMate homepage; it's been tested in Visual Studio 2019 and 2022, and needs the .NET Framework 4.6.2 installed.

AutoMate is a very simple web app based around the idea of listing used cars for sale. The app includes a web interface and a REST API.

#### AutoMate.WebApp

The web app project includes a set of ASP.NET MVC controllers which manage the web frontend, and a set of WebAPI controllers that provide the REST API we'll use during the workshop to query and modify records within the system.

The web app uses the Autofac IoC container; services are configured in the `Application_Start` method in `global.asax.cs`

#### AutoMate.Data

The "database" in this app is actually a fake in-memory data store based on a set of CSV files. **Nothing is actually saved to disk**; any new records, updates, etc. will persist until you restart the application, but every time you start the app, the database is reset to the same starting state. (This makes it really easy to get back to a known working state if things go weird.)

#### AutoMate.WebApp.Tests

There's a set of unit tests covering the web and API controllers. These are built using XUnit, and should work with Visual Studio's built-in test runner, and third party test runners including Resharper and NCrunch.

To run the tests from the command line, you can either use the xUnit console runner that's installed with the NuGet package:

```bash
cd dotnet\AutoMate\AutoMate.WebApp.Tests

~\.nuget\packages\xunit.runner.console\2.4.1\tools\net452\xunit.console.exe bin\debug\AutoMate.WebApp.Tests.dll
```

There's also a MSBuild target in the `AutoMate.WebApp.Tests.csproj` file which should invoke the xUnit console runner, which you can use if you've got the `dotnet` command line tools available:

```
cd dotnet\AutoMate\AutoMate.WebApp.Tests
dotnet build -t:Test
```

 



