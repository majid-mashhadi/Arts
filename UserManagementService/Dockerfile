# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ./UserManagementService/UserManagementService/UserManagementService.csproj ./UserManagementService/UserManagementService/UserManagementService.csproj 
COPY ./M2Store.Common/M2Store.Common/M2Store.Common.csproj ./M2Store.Common/M2Store.Common/M2Store.Common.csproj 
COPY ./M2Store.Contract/M2Store.Contract.csproj ./M2Store.Contract/M2Store.Contract.csproj

RUN dotnet restore "./UserManagementService/UserManagementService/UserManagementService.csproj"
RUN dotnet restore "./M2Store.Common/M2Store.Common/M2Store.Common.csproj"
RUN dotnet restore "./M2Store.Contract/M2Store.Contract.csproj"

COPY ./UserManagementService/UserManagementService ./UserManagementService/UserManagementService
COPY ./M2Store.Common/M2Store.Common ./M2Store.Common/M2Store.Common
COPY ./M2Store.Contract ./M2Store.Contract

RUN dotnet build "./UserManagementService/UserManagementService/UserManagementService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./UserManagementService/UserManagementService/UserManagementService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "UserManagementService.dll"]
