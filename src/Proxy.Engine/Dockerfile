# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

WORKDIR /chrome
RUN curl -LO https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb

WORKDIR /app
# Copy everything else and build
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /chrome
COPY --from=build-env ./chrome .
# Install chrome
RUN apt-get update -y
# RUN apt-get install -y libstdc++6 openssl libncurses5 locales curl
RUN apt-get -f install -y ./google-chrome-stable_current_amd64.deb
RUN rm -rf google-chrome-stable_current_amd64.deb
RUN apt-get clean -y

WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "Proxy.Engine.dll"]
