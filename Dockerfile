FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Encurtador.sln .
COPY src/Encurtador.Web/Encurtador.Web.csproj src/Encurtador.Web/
COPY tests/Encurtador.Tests/Encurtador.Tests.csproj tests/Encurtador.Tests/
RUN dotnet restore Encurtador.sln

COPY src/Encurtador.Web/ src/Encurtador.Web/
COPY tests/Encurtador.Tests/ tests/Encurtador.Tests/
RUN dotnet publish src/Encurtador.Web/Encurtador.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Encurtador.Web.dll"]
