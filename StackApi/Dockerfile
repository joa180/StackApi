# U�yj obrazu SDK .NET Core w wersji 8.0.202 jako bazowego obrazu
FROM mcr.microsoft.com/dotnet/sdk:8.0.202 AS build

# Ustaw katalog roboczy na '/app'
WORKDIR /app

# Skopiuj zawarto�� bie��cego katalogu do katalogu '/app' w kontenerze
COPY . .

# Przywr�� zale�no�ci projektu
RUN dotnet restore

# Zbuduj aplikacj�
RUN dotnet build -c Release -o /app/build

# Ustaw katalog roboczy na '/app/StackApi'
WORKDIR /app/StackApi

# Uruchom testy jednostkowe i pokrycie kodu (opcjonalnie)
# RUN dotnet test

# Publikuj aplikacj�
RUN dotnet publish -c Release -o /app/publish

# Drugi etap: U�yj obrazu uruchomieniowego .NET Core w wersji 8.0.202
FROM mcr.microsoft.com/dotnet/aspnet:8.0.202 AS runtime

# Ustaw katalog roboczy na '/app'
WORKDIR /app

# Skopiuj skompilowan� aplikacj� do obrazu uruchomieniowego
COPY --from=build /app/publish .

# Poinformuj kontener o tym, �e aplikacja nas�uchuje na porcie 80
ENV ASPNETCORE_URLS=http://+:80

# Otw�rz port 80 na kontenerze
EXPOSE 80

# Uruchom aplikacj� po uruchomieniu kontenera
ENTRYPOINT ["dotnet", "StackApi.dll"]
