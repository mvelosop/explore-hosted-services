docker container stop $(docker ps -aqf "name=hostedservices")
docker container rm $(docker ps -aqf "name=hostedservices")
docker build -t hostedservices -f .\HostedServices\Dockerfile .
docker run --name hostedservices hostedservices
