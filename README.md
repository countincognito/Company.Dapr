# Company.Dapr

This a minimalist template for building microservice architectures, using the [IDesign Method](http://www.idesign.net/), using .NET 8.0, Dapr, gRPC, RESTful API, Docker-Compose, and Swagger. It is heavily influenced by code samples from [Blair Moir](https://github.com/BlairMoir), [Remco Blok](https://github.com/RemcoBlok), and the [IDesign website](http://www.idesign.net/Downloads).

The solution demonstrates how to use polymorphism on your DTOs via gRPC to achieve maximum flexibility for smaller APIs.

It requires a local installation of:

- [chocolatey](https://chocolatey.org/) for application installation
- [Seq](https://community.chocolatey.org/packages/seq) for logging
- [PostgreSQL](https://www.postgresql.org/download/)
- [Docker Desktop](https://community.chocolatey.org/packages/docker-desktop)
- Windows Subsystem for Linux (WSL)
<!-- - [Dapr](https://docs.dapr.io/getting-started/install-dapr-cli/) -->

## Set up (InProc with Visual Studio)

To run the solution InProc with Visual Studio, simply run debug on the `Company.Microservice.Membership` project.

The InProc solution includes only the component interfaces and implementations - this is to demonstrate just one possible way of separating business code from plumbing in order to make development and testing easier.

The API should now be visible via Swagger and monitoring can be done from Seq via [http://localhost:5341/](http://localhost:5341/).

Calls made with the Web.RegisterRequest will be persisted in Postgres and can be viewed via pgAdmin.

## Set up (Docker-Compose with Visual Studio)

To run the solution in Docker-Compose with Visual Studio, simply run debug on the `docker-compose` project.

The Docker-Compose solution includes all component, framework and configuration projects. It also includes Dapr, Redis, Zipkin, and Seq as docker containers within the cluster.

The API should now be visible via Swagger, logs can be checked from Seq via [http://localhost:81/](http://localhost:81/), and telemetry can be checked from Zipkin via [http://localhost:6499/](http://localhost:6499/).

## Set up (Docker-Compose with VS Code - Windows)

To run the solution in Docker-Compose with VS Code on Windows, perform the following steps:

1. Run Docker Desktop.
1. From powershell (in the root directory) run `create_certs.windows.ps1` to install the development certificates for the service containers.
1. From powershell build the entire solution from the root directory using `docker compose -f ./docker-compose.yml -f ./docker-compose.override.yml build`.
1. From powershell run the solution from the root directory using `docker compose -f ./docker-compose.yml -f ./docker-compose.override.yml up`.

The API should now be visible via Swagger, logs can be checked from Seq via [http://localhost:81/](http://localhost:81/), and telemetry can be checked from Zipkin via [http://localhost:6499/](http://localhost:6499/).

## Set up (Docker-Compose with VS Code - DevContainers)

A general overview to set up DevContainers can be found here: [https://code.visualstudio.com/docs/remote/remote-overview](https://code.visualstudio.com/docs/remote/remote-overview)

To run the solution in Docker-Compose with DevContainers VS Code, perform the following steps:

1. Run Docker Desktop.
1. In Windows Subsystem for Linux (WSL), set the default distribution to Ubuntu with `wsl --set-default Ubuntu`. This ensures that the default container for DevContainer environment is usable (more details can be found here: [https://docs.docker.com/desktop/wsl/](https://docs.docker.com/desktop/wsl/))
1. In VS Code, install the Remote Development extension [https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.vscode-remote-extensionpack](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.vscode-remote-extensionpack)
1. Use the VS Code Command Palette to select `Dev Containers: Clone Repository in Container Volume...` and select this repository.
1. From the VS Code terminal (in the root directory) run `chmod 755 ./create_certs.linux.ps1 && ./create_certs.linux.ps1` to install the development certificates for the service containers.
1. From the VS Code terminal build the entire solution from the root directory using `docker compose -f ./docker-compose.yml -f ./docker-compose.linux.yml build`.
1. From the VS Code terminal run the solution from the root directory using `docker compose -f ./docker-compose.yml -f ./docker-compose.linux.yml up`.

You will need to allow port forwarding to the localhost for the relevant service you wish to explore.

The API should now be visible via Swagger, logs can be checked from Seq via [http://localhost:81/](http://localhost:81/), and telemetry can be checked from Zipkin via [http://localhost:6499/](http://localhost:6499/).

**Note**: when debugging run Docker-Compose as described above, then from the `Run and Debug` tab in VS Code select the `Docker .NET Attach (Preview)` option. Select the container you wish to debug and follow the instructions to activate the debugger.

## Note

You can normally find the address for `company.microservice.membership.service` in the Docker Desktop container dashboard, when used directly on your development machine. When using DevContainers, this information can be found in the Docker extension tab. In both cases, the same information can be found using the command `docker ps -a`.

Be sure to use the port that is mapped to the port 443, and when the link opens ensure the protocol is listed as `https` and the address is pointing at `/swagger` (neither of these will be set by default). Also, depending on the order of services spinning up, you may need to restart the containers for the services that need to run database migrations (i.e. `company.access.user.service` and `company.utility.encryption.service`, followed by their respective Dapr services). This is because the migrations will fail if the database is not already running.










<!-- 
## Set up (Local Kubernetes)

To run the solution in a local Kubernetes cluster, perform the following steps:

1. Run Docker Desktop.
1. In Docker Desktop settings, enable the local Kubernetes cluster.
1. From powershell initialize Dapr in the Kubernetes cluster using `dapr init -k --runtime-version 1.11.2`.
1. From powershell check the status of the Dapr initialization using `dapr status -k`.
1. From powershell build the entire solution from the root directory using `docker compose build`.
1. From powershell apply the deployment configurations from the root directory using `kubectl apply -f ".\k8s\postgres\",".\k8s\seq\",".\k8s\redis\",".\k8s\zipkin\",".\k8s\otel\",".\k8s\"`.
1. From powershell forward the `http` port from `company.microservice.membership.service` to `localhost` using `kubectl port-forward service/company-microservice-membership-service 8080:80`.

The API should now be visible via Swagger via [http://localhost:8080/swagger/](http://localhost:8080/swagger/), logs can be checked from Seq via [http://localhost:31081/](http://localhost:31081/), and telemetry can be checked from Zipkin via [http://localhost:31623/](http://localhost:31623/).

**Note**: the `file` output for the Open Telemetry Collector is disabled in this version as there appears to be issues around access permissions to the host file system. Also, depending on the order of services spinning up, you may need to restart the containers for the services that need to run database migrations (i.e. `company.access.user.service` and `company.utility.encryption.service`). This is because the migrations will fail if the database is not already running.

I am not a Kubernetes expert, so apologies for the flakiness of this example. -->
