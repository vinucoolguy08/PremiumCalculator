
# scm-destinationservices-deliveryplanning

## Delivery planning capability in destination services

### Dependencies
#### Common
* Docker (Maersk advises Rancher Desktop)
* If using Rancher ensure you Enable Kubernetes version with dockered(moby) option.
* A sql client for Postgresql, eg, PgAdmin4 or Jetbrains DataGrip

#### Macos Specific
##### Docker Host configured for test containers
If you are using macos with rancher desktop you will need to tell testcontainers where to find the docker host socket, to do this,
1. run the following command to find the docker sockets in your environment
```bash
docker context list
```
2. set an environment variable DOCKER_HOST to the socket eg
``` .zshrc 
...
export DOCKER_HOST=unix:///Users/david/.rd/docker.sock
...
```
#### Windows Specific
- For Windows machine, install _Windows Subsystem for Linux (WSL)_ with _Ubuntu OS_.
   ```cmd
   wsl --install
   ```


### Setup Steps (Local)
1. The repo for CCA is now under [GitHub Repo](https://github.com/Maersk-Global/scm-destinationservices-v2), clone it into local machine with below command.
```bash
cd ~/repos # or your local repo directory
git clone https://github.com/Maersk-Global/scm-destinationservices-v2.git
```
	 >This contains solutions for the 3 bound contexts that we have defined.  
	>- Destination Integration
	>- Deliverable Item
	>- Deliverable Planning
	
2. Add Configuration Files
> **_NOTE: Because the Isolated function runtime is being used, all function app configuration is loaded from a launchSettings.json file and not local.host.json._**

**copy the templates into the respective projects**
```bash
cd src
mkdir ./DeliveryPlanning/Maersk.DeliveryPlanning.Api/Properties
cp ./.templates/Api.launchSettings.template.json ./DeliveryPlanning/Maersk.DeliveryPlanning.Api/Properties/launchSettings.json
mkdir ./DeliveryPlanning/Maersk.DeliveryPlanning.Functions/Properties
cp ./.templates/Function.launchSettings.template.json ./DeliveryPlanning/Maersk.DeliveryPlanning.Functions/Properties/launchSettings.json
mkdir ./DeliverableItem/Maersk.DeliverableItem.Functions/Properties
cp ./.templates/Function.launchSettings.template.json ./DeliverableItem/Maersk.DeliverableItem.Functions/Properties/launchSettings.json
```
**Add the env vars defined in the config-template.env to each of the launchSettings.json files (we will only maintain a single list).**
- Add the relevent section, the DOTNET_ vars are for the Api, the DP_FN_ vars are for the DeliveryPlanning function app and the DI_FN_ vars are for the DeliverableItem function app.
- Ensure that you set the hosts to localhost for the urls - for running within a docker compose environment you will use the service names in docker compose. 
```env
DI_FN_OpenTelemetry__CollectorUrl="http://localhost:4317" #for running in VS or Rider or dotnet run
DI_FN_OpenTelemetry__CollectorUrl="http://otel-collector:4317" #for running in docker compose (aka docker compose up difn)
```
3. Build all three projects `Destination Integration`, `Deliverable Item` and `Deliverable Item`.
4. Run `docker-compose up` from the `/src` folder from git - this will spin up Azurite (for Functions App), Zipkin (local tracing) and Postgresql db server (which has a volume mount)
```bash
cd ~/repos/scm-destinationservices-deliveryplanning/src/
docker-compose up
```
5. Open _pgAdmin_ or _DataGrip_ and connect with local postgresql server. `Servers -> Register -> Server`
	```
	General:Name - local
	Connection:Hostname - localhost
	Connection:Port - 5455
	Connection:Username - dev
	```
	If using PgAdmin, enter a master password and save it.
6. Connect with local postgresql server and create 2 empty databases: `deliverable_item` and `delivery_planning`
7. Run all test cases are run successfully.
8. Create your own ServiceBus in Azure subscription (Azure Basic tier pricing model does not support Topics, so you will need to use Standard tier or above).
9. Update the value of `ServiceBusConnectionString` in `C:\git\scm-destinationservices-v2\src\DeliveryPlanning\Maersk.DeliveryPlanning.Functions\local.settings.json`
10. F5 Run from Visual Studio or Rider for the relevant service - i.e. Function App or API - You can use the postman scripts to call the endpoints.
11. For now, run Delivery Planning project.
13. Import `scm-destinationservices-v2\postman\Destination V2.postman_collection.json` file into Postman.
14. Go to Postman, check Environment quick look option and add `DeliveryPlanningFunctionsBaseUrl` variable with initial and current values both as `http://localhost:7278`
15. Send POST request from Postman Collections - Destination V2/Delivery Planning/DeliverableItemCreatedListener.
16. You can check DeliverableItem into PostgreSQL database via pgAdmin v6 with below query.
 ```sql
	select * from mt_events;
	select * from mt_streams;

	select * from mt_doc_deliveryplanlistviewprojection;
	select data->'Status' from mt_doc_deliveryplanlistviewprojection;

	select * from mt_doc_deliveryplanlistviewprojection
	where data->>'Status' = 'DRAFT';
```

### Setup Steps (Docker)
#### Setup environment
from the `/src` directory
1. Copy the env file template
```bash
cp ./.docker/config-template.env ./config.env
cp ./.docker/config-tests.env ./config-tests.env
```
2. Set local variables in the file 
3. Using docker compose:

*only if you do not have these env vars already set on your system set them:*
MacOS/Linux
```bash
cd /src
export Nuget_CustomFeedUserName=####
export Nuget_CustomFeedPassword=####
```
Windows (powershell)
```ps
$env:Nuget_CustomFeedUserName=####
$env:Nuget_CustomFeedPassword=####
```
Then start the dpapi, dpfn or difn services
```bash
# start infra
docker-compose up 
# start dpapi
docker-compose up dpapi # or dpfn or difn depending on what service you want 
```
Alternatively build the image:
```bash
docker build --build-arg Nuget_CustomFeedUserName --build-arg Nuget_CustomFeedPassword . -f deliveryplanning.api.dockerfile --tag dpapi
```

### To run tests (beta)
1. Start services: `docker compose up`
2. Run tests: `docker compose up dptests` or `docker compose up ditests`


## Migrations
*Requirements:*
* 7.0.X version of dotnet-ef tools, installed with:
`dotnet tool update dotnet-ef -g`

### PRODUCTION
The production migration setup uses a self contained efbundle executable. This is how it's built and executed in the pipeline.
#### 1. Create migrations exe
Migrations are created using an ef migrations bundle. 
From /src
```bash 
export DOTNET_Postgresql__BaseConnection="PLACEHOLDER_CONN"
export DOTNET_Postgresql__Password="PLACEHOLDER_CONN"
dotnet ef migrations bundle \
 -p ./DeliveryPlanning/Maersk.DeliveryPlanning.Infrastructure/Maersk.DeliveryPlanning.Infrastructure.csproj \
 --self-contained  \
 -o ./efbundle.exe \
 -r linux-x64 \
 --force
```

#### 2. Apply the migrations
or using the environement variable
```bash
export DOTNET_Postgresql__BaseConnection="PLACE_CONNECTION_STRING_HERE"
export DOTNET_Postgresql__Password="PLACE_POSTGRES_PASSWORD_HERE"
.\efbundle 	-v
```

### LOCAL 

Running the following will pickup your current project configuration:
```bash
dotnet ef database update \
 -p ./DeliveryPlanning/Maersk.DeliveryPlanning.Infrastructure/Maersk.DeliveryPlanning.Infrastructure.csproj \
 -s ./DeliveryPlanning/Maersk.DeliveryPlanning.Api/Maersk.DeliveryPlanning.Api.csproj
```

### How to add a new migration
Run the following command replacing MyMigration with your migration name:
```bash
dotnet ef migrations add MyMigration \
 -p ./DeliveryPlanning/Maersk.DeliveryPlanning.Infrastructure/Maersk.DeliveryPlanning.Infrastructure.csproj \
 -s ./DeliveryPlanning/Maersk.DeliveryPlanning.Api/Maersk.DeliveryPlanning.Api.csproj

```
