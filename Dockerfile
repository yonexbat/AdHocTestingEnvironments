FROM docker.tools.pnet.ch/base/dotnet:5.0-alpine

# This command assumes that the current directory is the 'project/publish'
# directory that contains all files that have to be copied into the image
COPY ./. /app

RUN chmod +x /app/AdHocTestingEnvironments

WORKDIR /app

USER baseuser

ENTRYPOINT ["./AdHocTestingEnvironments"]