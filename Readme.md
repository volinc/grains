## Setup environment

1. Grand access docker to your Project folder
2. Create shared network for databases/other services 
`docker network create --attachable grains-backend --subnet=174.10.1.0/16`
3. Start databases 
`docker-compose -f docker-compose-standby.yml up -d`

## Build and run

Start from VS or `docker-compose up --build`

## How to remove

`docker `
`docker network rm grains-backend`

## K8s

 `docker build -t volinc/grains:selfhosted -f ./SelfHosted/Dockerfile .`
 `docker build -t volinc/grains:cohosted -f ./CoHosted/Dockerfile .`
 