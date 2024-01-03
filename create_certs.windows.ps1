Set-StrictMode -Version 2.0

$CrtPassword = "Password123!"

Write-Output "Generating certificate and password for Company.Microservice.Membership"
dotnet dev-certs https -ep $env:APPDATA\ASP.NET\https\company.microservice.membership.pfx -p $CrtPassword
dotnet user-secrets -p src\Company.Microservice.Membership\Company.Microservice.Membership.csproj set "Kestrel:Certificates:Development:Password" "$CrtPassword"

Write-Output "Generating certificate and password for Company.Microservice.Membership.Service"
dotnet dev-certs https -ep $env:APPDATA\ASP.NET\https\company.microservice.membership.service.pfx -p $CrtPassword
dotnet user-secrets -p src\Company.Microservice.Membership.Service\Company.Microservice.Membership.Service.csproj set "Kestrel:Certificates:Development:Password" "$CrtPassword"

Write-Output "Generating certificate and password for Company.Manager.Membership.Service"
dotnet dev-certs https -ep $env:APPDATA\ASP.NET\https\company.manager.membership.service.pfx -p $CrtPassword
dotnet user-secrets -p src\Company.Manager.Membership.Service\Company.Manager.Membership.Service.csproj set "Kestrel:Certificates:Development:Password" "$CrtPassword"

Write-Output "Generating certificate and password for Company.Engine.Registration.Service"
dotnet dev-certs https -ep $env:APPDATA\ASP.NET\https\company.engine.registration.service.pfx -p $CrtPassword
dotnet user-secrets -p src\Company.Engine.Registration.Service\Company.Engine.Registration.Service.csproj set "Kestrel:Certificates:Development:Password" "$CrtPassword"

Write-Output "Generating certificate and password for Company.Access.User.Service"
dotnet dev-certs https -ep $env:APPDATA\ASP.NET\https\company.access.user.service.pfx -p $CrtPassword
dotnet user-secrets -p src\Company.Access.User.Service\Company.Access.User.Service.csproj set "Kestrel:Certificates:Development:Password" "$CrtPassword"

Write-Output "Generating certificate and password for Company.Utility.Encryption.Service"
dotnet dev-certs https -ep $env:APPDATA\ASP.NET\https\company.utility.encryption.service.pfx -p $CrtPassword
dotnet user-secrets -p src\Company.Utility.Encryption.Service\Company.Utility.Encryption.Service.csproj set "Kestrel:Certificates:Development:Password" "$CrtPassword"

Write-Output "Generating certificate and password for Company.Utility.Cache.Service"
dotnet dev-certs https -ep $env:APPDATA\ASP.NET\https\company.utility.cache.service.pfx -p $CrtPassword
dotnet user-secrets -p src\Company.Utility.Cache.Service\Company.Utility.Cache.Service.csproj set "Kestrel:Certificates:Development:Password" "$CrtPassword"
