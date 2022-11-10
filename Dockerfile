#dotnet core because there is not .net framework for linux :(
FROM mono:latest AS base
WORKDIR /app
EXPOSE 8080

#FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
FROM mono:latest AS build

WORKDIR /app/src
#COPY ["Kontur.ImageTransformer/Kontur.ImageTransformer.csproj", "./"]
COPY ["Kontur.ImageTransformer/packages.config", "./"]
#RUN dotnet restore "Kontur.ImageTransformer.csproj"
RUN nuget restore -PackagesDirectory ./packages
COPY ./Kontur.ImageTransformer .

WORKDIR /app/build
#RUN dotnet build -c Release -o app/build -f net461
RUN msbuild /p:outputdir=build /p:Configuration=Release ../src/Kontur.ImageTransformer.csproj 

FROM base AS runtime
WORKDIR /app
COPY --from=build /app/src/bin/Release .
#ENTRYPOINT ["dotnet", "Kontur.ImageTransformer.dll"]
ENTRYPOINT ["mono", "Kontur.ImageTransformer.exe"]

#FROM mcr.microsoft.com/dotnet/runtime AS runtime
#WORKDIR app/
#COPY --from=build /app/build .
#ENTRYPOINT dotnet run Kontur.ImageTransformer.dll