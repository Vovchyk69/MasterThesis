FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build

ENV APP_HOME /opt/app
RUN mkdir $APP_HOME
WORKDIR $APP_HOME

COPY SchedulerService/*.csproj $APP_HOME/SchedulerService/
RUN  cd $APP_HOME/ProductService && dotnet restore

COPY SchedulerService $APP_HOME/SchedulerService/
RUN cd $APP_HOME/ProductService && dotnet build

FROM build AS publish
WORKDIR $APP_HOME/SchedulerService
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:3030
ENV ASPNETCORE_ENVIRONMENT=docker
COPY --from=publish /opt/app/SchedulerService/out ./
ENTRYPOINT ["dotnet", "SchedulerService.dll"]