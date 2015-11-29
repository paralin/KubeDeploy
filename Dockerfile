FROM microsoft/aspnet

COPY . /app
WORKDIR /app/src/KubeDeploy/
RUN dnu restore

EXPOSE 8080
ENTRYPOINT dnx -p project.json KubeDeploy
