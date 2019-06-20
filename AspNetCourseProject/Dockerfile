FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["BookCreatorApp/BookCreatorApp.csproj", "BookCreatorApp/"]
COPY ["BookCreator.Services/BookCreator.Services.csproj", "BookCreator.Services/"]
COPY ["BookCreator.ViewModels/BookCreator.ViewModels.csproj", "BookCreator.ViewModels/"]
COPY ["BookCreator.Data/BookCreator.Data.csproj", "BookCreator.Data/"]
COPY ["BookCreator.Models/BookCreator.Models.csproj", "BookCreator.Models/"]

RUN dotnet restore "BookCreatorApp/BookCreatorApp.csproj"
COPY . .
WORKDIR "/src/BookCreatorApp"
RUN dotnet build "BookCreatorApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BookCreatorApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BookCreatorApp.dll"]