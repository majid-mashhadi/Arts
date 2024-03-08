
docker build -t m2-store-user-management -f Dockerfile .
docker run -d -p 5001:5001 -p 5000:5000 --name m2-store-user-management m2-store-user-management

run docker-compose build
after docker-compose up



http://localhost:5000/api/v1/services
https://localhost:5001/api/v1/services




# docker exec -it m2-store-user-management /bin/bash


#Create the Certificate:
#openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365
#openssl pkcs12 -export -out certificate.pfx -inkey key.pem -in cert.pem
# dotnet dev-certs https --trust



#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["UserManagementService/UserManagementService.csproj", "UserManagementService/"]
COPY ["../M2Store.Common/M2Store.Common/M2Store.Common.csproj", "../M2Store.Common/M2Store.Common/"]
COPY ["../M2Store.Contract/M2Store.Contract.csproj", "../M2Store.Contract/"]
RUN dotnet restore "UserManagementService/UserManagementService.csproj"
COPY . .
WORKDIR "/src/UserManagementService"
RUN dotnet build "UserManagementService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserManagementService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .



kubectl logs <pod_name> -c <container_name>


kubectl logs m2-store-user-management-deployment-6b86d748d9-t5d2d

kubectl describe pod m2-store-user-management-deployment-555cc8d56f-prfkq 
kubectl logs m2-store-user-management-deployment-555cc8d56f-prfkq


 http://m2-store-user-management-lb-service.default.svc.cluster.local:4000




 /Applications/Visual Studio.app/Contents/MonoBundle/AddIns/MonoDevelop.Docker/MSbuild/Sdks/Microsoft.Docker.Sdk/build/Microsoft.VisualStudio.Docker.Compose.targets(5,5):
 Error: Visual Studio Container Tools requires Docker to be running before building, debugging or running a containerized project.

For more info, please see: https://aka.ms/DockerToolsTroubleshooting (docker-compose)