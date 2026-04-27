FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Directory.Packages.props", "./"]
COPY ["global.json", "./"]
COPY ["radio-data.sln", "./"]
COPY ["radiodata-ui/radiodata-ui.csproj", "radiodata-ui/"]
COPY ["ukrepeaterlib/ukrepeaterlib.csproj", "ukrepeaterlib/"]
COPY ["chirpcsvlib/chirpcsvlib.csproj", "chirpcsvlib/"]
COPY ["DotNetCoords/DotNetCoords.csproj", "DotNetCoords/"]

RUN dotnet restore "radiodata-ui/radiodata-ui.csproj"

COPY . .
RUN dotnet publish "radiodata-ui/radiodata-ui.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "radiodata-ui.dll"]
