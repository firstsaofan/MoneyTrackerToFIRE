FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RecordMoney.WebApi/RecordMoney.WebApi.csproj", "RecordMoney.WebApi/"]
RUN dotnet restore "RecordMoney.WebApi/RecordMoney.WebApi.csproj"
COPY . .
WORKDIR "/src/RecordMoney.WebApi"
RUN dotnet build "RecordMoney.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RecordMoney.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RecordMoney.WebApi.dll"]
