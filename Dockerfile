FROM dxpprivate.azurecr.io/ahi-build:6.0-alpine3.19 AS build-env
WORKDIR .
COPY NuGet.Config ./
COPY src/IdpServer/*.csproj         ./src/IdpServer/
COPY src/IdpServer.Application/*.csproj ./src/IdpServer.Application/
COPY src/IdpServer.Domain/*.csproj      ./src/IdpServer.Domain/
COPY src/IdpServer.Persistence/*.csproj ./src/IdpServer.Persistence/
RUN dotnet restore ./src/IdpServer/*.csproj /property:Configuration=Release -nowarn:msb3202,nu1503
# Copy csproj and restore as distinct layers
COPY src ./src

# Copy everything else and build
RUN dotnet publish -r linux-x64 src/IdpServer/*.csproj -c Release --no-self-contained --no-restore -o app/out
# Build runtime image
FROM dxpprivate.azurecr.io/ahi-runtime:6.0-alpine3.19
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "IdpServer.dll"]