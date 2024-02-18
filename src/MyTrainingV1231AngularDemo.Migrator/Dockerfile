#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/MyTrainingV1231AngularDemo.Migrator/MyTrainingV1231AngularDemo.Migrator.csproj", "src/MyTrainingV1231AngularDemo.Migrator/"]
COPY ["src/MyTrainingV1231AngularDemo.EntityFrameworkCore/MyTrainingV1231AngularDemo.EntityFrameworkCore.csproj", "src/MyTrainingV1231AngularDemo.EntityFrameworkCore/"]
COPY ["src/MyTrainingV1231AngularDemo.Core/MyTrainingV1231AngularDemo.Core.csproj", "src/MyTrainingV1231AngularDemo.Core/"]
COPY ["src/MyTrainingV1231AngularDemo.Core.Shared/MyTrainingV1231AngularDemo.Core.Shared.csproj", "src/MyTrainingV1231AngularDemo.Core.Shared/"]
RUN dotnet restore "src/MyTrainingV1231AngularDemo.Migrator/MyTrainingV1231AngularDemo.Migrator.csproj"
COPY . .
WORKDIR "/src/src/MyTrainingV1231AngularDemo.Migrator"
RUN dotnet build "MyTrainingV1231AngularDemo.Migrator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyTrainingV1231AngularDemo.Migrator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyTrainingV1231AngularDemo.Migrator.dll", "-s"]
