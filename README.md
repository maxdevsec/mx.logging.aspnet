# ASP.Net Logging Library
Contains middleware to use for the global handling of exceptions.

## Manually publishing a new version of the NuGet package

````
dotnet pack --configuration 'RELEASE'

dotnet nuget list source

dotnet nuget push bin/Release/Setrans.Logging.AspNet.1.2.0.nupkg -s SeTransLocalFeed -k <API Key>
````

    

 