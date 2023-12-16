# Script.ps1

# Change to the directory containing the docker-compose file
cd .\OperationsManager\

# Run docker-build
docker build . -t operationsmanager

# Run docker run container
docker run --name OperationsManager -p 9020:443 -p 8020:80 -d operationsmanager  
