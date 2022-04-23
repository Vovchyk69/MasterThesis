FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build

ENV APP_HOME /opt/app
RUN mkdir $APP_HOME
WORKDIR $APP_HOME

COPY Shared/*.csproj $APP_HOME/Shared/
RUN  cd $APP_HOME/Shared && dotnet restore

COPY Shared $APP_HOME/Shared/
RUN cd $APP_HOME/Shared && dotnet build

COPY FileMananagementService/*.csproj $APP_HOME/FileMananagementService/
RUN  cd $APP_HOME/FileMananagementService && dotnet restore

COPY FileMananagementService $APP_HOME/FileMananagementService/
RUN cd $APP_HOME/FileMananagementService && dotnet build

FROM build AS publish
WORKDIR $APP_HOME/FileMananagementService
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5030
ENV ASPNETCORE_ENVIRONMENT=docker
COPY --from=publish /opt/app/FileMananagementService/out ./
ENTRYPOINT ["dotnet", "FileMananagementService.dll"]