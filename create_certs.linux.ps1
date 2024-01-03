Set-StrictMode -Version 2.0

$CrtPassword = "Password123!"

Write-Output "Generating certificate"
dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p $CrtPassword
# dotnet dev-certs https --trust
