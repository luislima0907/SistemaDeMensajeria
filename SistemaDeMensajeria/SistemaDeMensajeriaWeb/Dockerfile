FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SistemaDeMensajeriaWeb.csproj", "SistemaDeMensajeriaWeb/"]
RUN dotnet restore "SistemaDeMensajeriaWeb/SistemaDeMensajeriaWeb.csproj"
COPY . ./SistemaDeMensajeriaWeb
WORKDIR "/src/SistemaDeMensajeriaWeb"
RUN dotnet build "SistemaDeMensajeriaWeb.csproj" -c Release -o /app/build
RUN dotnet publish "SistemaDeMensajeriaWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SistemaDeMensajeriaWeb.dll"]