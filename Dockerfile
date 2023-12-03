FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY out ./

EXPOSE 50505

ENV ASPNETCORE_URLS=http://*:50505 \
    LOCALE=en-US \
    TZ=America/Chicago

ENTRYPOINT ["dotnet", "kontaktica.dll"]