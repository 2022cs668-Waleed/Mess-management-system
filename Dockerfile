FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["2022-CS-668.csproj", "./"]
RUN dotnet restore "2022-CS-668.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "2022-CS-668.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "2022-CS-668.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}

ENTRYPOINT ["dotnet", "2022-CS-668.dll"]
