FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY *.csproj ./rapide-shortener/
RUN ls
RUN dotnet restore "rapide-shortener/rapide-shortener-service.csproj" 

# copy everything else and build app
COPY . ./rapide-shortener/
WORKDIR /source/rapide-shortener
RUN dotnet publish -c release -o /app -r linux-musl-x64 --self-contained true /p:PublishTrimmed=true /p:PublishReadyToRun=false
RUN ls
# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY --from=build /app ./
EXPOSE 3333
EXPOSE 5001
ENTRYPOINT ["./rapide-shortener-service"]