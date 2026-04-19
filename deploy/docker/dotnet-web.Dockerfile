FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG PROJECT_PATH

WORKDIR /src
COPY . .

RUN dotnet restore "$PROJECT_PATH"
RUN dotnet publish "$PROJECT_PATH" --configuration Release --no-restore --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

ARG APP_DLL
ENV APP_DLL=$APP_DLL
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["sh", "-c", "dotnet \"$APP_DLL\""]

