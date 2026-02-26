# 1. Aşama: Derleme (Build)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Proje dosyasını kopyalayıp bağımlılıkları yükleyelim
COPY ["ProjectNexus.csproj", "./"]
RUN dotnet restore
# Tüm dosyaları kopyalayıp uygulamayı derleyelim
COPY . .
RUN dotnet publish -c Release -o /app

# 2. Aşama: Çalıştırma (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
# Uygulamanın çalışacağı portu belirtelim
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ProjectNexus.dll"]