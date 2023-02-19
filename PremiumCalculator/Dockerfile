FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PremiumCalculator/PremiumCalculator.csproj", "PremiumCalculator/"]
RUN dotnet restore "PremiumCalculator/PremiumCalculator.csproj"
COPY . .
WORKDIR "/src/PremiumCalculator"
RUN dotnet build "PremiumCalculator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PremiumCalculator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PremiumCalculator.dll"]
