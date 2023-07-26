# Company.Dapr

This a minimalist template for building microservice architectures, using the [IDesign Method](http://www.idesign.net/), using .NET 7.0, Dapr, gRPC, RESTful API, and Swagger. It is heavily influenced by code samples from [Blair Moir](https://github.com/BlairMoir), [Remco Blok](https://github.com/RemcoBlok), and the [IDesign website](http://www.idesign.net/Downloads).

The solution demonstrates how to use polymorphism on your DTOs via gRPC to achieve maximum flexibility for smaller APIs.

It requires a local installation of:

- [chocolatey](https://chocolatey.org/) for application installation
- [Seq](https://community.chocolatey.org/packages/seq) for logging
- [Docker Desktop](https://community.chocolatey.org/packages/docker-desktop)
- [Dapr](https://docs.dapr.io/getting-started/install-dapr-cli/)
- [Project Tye](https://github.com/dotnet/tye)

## Set up

To run the solution in dapr, perform the following steps:

1. Run Docker Desktop.
1. From powershell run `dapr init` to initialize dapr.
1. From powershell build the entire solution from the root directory using `dotnet build`.
1. From powershell run `tye run` to run the individual services using dapr sidecars.
1. The API should now be visible via Swagger and monitoring can be done from [Seq](http://localhost:5341/) and the [Tye Dashboard](http://localhost:8000/).

To run the solution InProc, simply run debug on the `Company.Microservice.Membership` project from Visual Studio.

The dapr-oriented solution includes all component, framework and configuration projects. The InProc solution includes only the component interfaces and implementations - this is to demonstrate just one possible way of separating business code from plumbing in order to make development and testing easier.
