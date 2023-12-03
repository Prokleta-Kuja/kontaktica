FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY out ./

EXPOSE 8080

ENV LOCALE=en-US \
    TZ=America/Chicago

ENTRYPOINT ["dotnet", "kontaktica.dll"]