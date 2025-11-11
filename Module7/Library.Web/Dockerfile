FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Library.Web/Library.Web.csproj", "Library.Web/"]
COPY ["Library.Contracts/Library.Contracts.csproj", "Library.Contracts/"]
COPY ["Library.SharedKernel/Library.SharedKernel.csproj", "Library.SharedKernel/"]
COPY ["Library.Data.PostgreSql/Library.Data.PostgreSql.csproj", "Library.Data.PostgreSql/"]
COPY ["Library.Domain/Library.Domain.csproj", "Library.Domain/"]
RUN dotnet restore "Library.Web/Library.Web.csproj"
COPY . .
WORKDIR "/src/Library.Web"
RUN dotnet build "Library.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Library.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Library.Web.dll"]
