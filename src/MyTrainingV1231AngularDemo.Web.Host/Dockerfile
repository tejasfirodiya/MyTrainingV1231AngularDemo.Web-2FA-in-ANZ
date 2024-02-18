#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/MyTrainingV1231AngularDemo.Web.Host/MyTrainingV1231AngularDemo.Web.Host.csproj", "src/MyTrainingV1231AngularDemo.Web.Host/"]
COPY ["src/MyTrainingV1231AngularDemo.Web.Core/MyTrainingV1231AngularDemo.Web.Core.csproj", "src/MyTrainingV1231AngularDemo.Web.Core/"]
COPY ["src/MyTrainingV1231AngularDemo.Application/MyTrainingV1231AngularDemo.Application.csproj", "src/MyTrainingV1231AngularDemo.Application/"]
COPY ["src/MyTrainingV1231AngularDemo.Application.Shared/MyTrainingV1231AngularDemo.Application.Shared.csproj", "src/MyTrainingV1231AngularDemo.Application.Shared/"]
COPY ["src/MyTrainingV1231AngularDemo.Core.Shared/MyTrainingV1231AngularDemo.Core.Shared.csproj", "src/MyTrainingV1231AngularDemo.Core.Shared/"]
COPY ["src/MyTrainingV1231AngularDemo.Core/MyTrainingV1231AngularDemo.Core.csproj", "src/MyTrainingV1231AngularDemo.Core/"]
COPY ["src/MyTrainingV1231AngularDemo.EntityFrameworkCore/MyTrainingV1231AngularDemo.EntityFrameworkCore.csproj", "src/MyTrainingV1231AngularDemo.EntityFrameworkCore/"]
COPY ["src/MyTrainingV1231AngularDemo.GraphQL/MyTrainingV1231AngularDemo.GraphQL.csproj", "src/MyTrainingV1231AngularDemo.GraphQL/"]
RUN dotnet restore "src/MyTrainingV1231AngularDemo.Web.Host/MyTrainingV1231AngularDemo.Web.Host.csproj"
COPY . .
WORKDIR "/src/src/MyTrainingV1231AngularDemo.Web.Host"
RUN dotnet build "MyTrainingV1231AngularDemo.Web.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyTrainingV1231AngularDemo.Web.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyTrainingV1231AngularDemo.Web.Host.dll"]
