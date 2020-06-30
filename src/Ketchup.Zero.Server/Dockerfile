FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
ENV ASPNETCORE_URLS http://+:5004
WORKDIR /zero
EXPOSE 5004
COPY . .
ENTRYPOINT ["dotnet", "Ketchup.Zero.Server.dll"]