FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

# Copia somente o arquivo do projeto para a etapa de restore.
COPY *.csproj ./
RUN dotnet restore

#Copia todo o restante do código (subpastas incluídas) para dentro do container.
COPY . ./

#gera somente os binários necessários para rodar a aplicação, não os arquivos fonte.
RUN dotnet publish -c Release -o /app/publish


#Imagem base para rodar aplicações ASP.NET Core, 
#mas não contém o SDK do .NET, apenas o runtime.
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# multi-stage build: copia os binários necessários para a aplicação rodar na nova imagem aspnet
COPY --from=build /app/publish .


EXPOSE 5000
ENTRYPOINT ["dotnet", "costumersApi.dll"]