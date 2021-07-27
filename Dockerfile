FROM docker.tools.post.ch/base/dotnet-self:1.0.1-2

COPY AdHocTestingEnvironments/publish/. /app

RUN chmod +x /app/AdHocTestingEnvironments

WORKDIR /app

USER baseuser

ENTRYPOINT ["./AdHocTestingEnvironments"]