#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ToDoApp_Challenge/ToDoApp_Challenge.csproj", "ToDoApp_Challenge/"]
RUN dotnet restore "ToDoApp_Challenge/ToDoApp_Challenge.csproj"
COPY . .
WORKDIR "/src/ToDoApp_Challenge"
RUN dotnet build "ToDoApp_Challenge.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ToDoApp_Challenge.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDoApp_Challenge.dll"]