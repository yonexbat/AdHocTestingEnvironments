FROM docker.tools.pnet.ch/base/dotnet:5.0-alpine

COPY AdHocTestingEnvironments/publish/. /app

#RUN chmod +x /app/AdHocTestingEnvironments

WORKDIR /app

USER baseuser

#ENTRYPOINT ["./AdHocTestingEnvironments"]