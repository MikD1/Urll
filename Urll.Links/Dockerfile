FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /App
COPY Urll.Links ./Urll.Links
COPY Urll.Links.Contracts ./Urll.Links.Contracts
WORKDIR /App/Urll.Links

RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=build /App/Urll.Links/out .
ENTRYPOINT ["dotnet", "Urll.Links.dll"]