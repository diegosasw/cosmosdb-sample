# cosmosdb-sample
CosmosDb Sample

## Pre-Requirements
- SDK .NET 8
- Docker for Windows

## Steps
Run CosmosDb emulator as a docker container
```
docker run \
    --publish 8081:8081 \
    --publish 10250-10255:10250-10255 \
    --env AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE=127.0.0.1 \
    --name linux-cosmosdb-emulator \
    --detach \
    mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest
```

Download the certificate from the container
```
curl -k https://localhost:8081/_explorer/emulator.pem > emulatorcert.crt
```

With elevated permissions, add the certificate to the Trusted Root Certification Authorities
```
certutil.exe -addstore root emulatorcert.crt
```

This should have added the certificate issued to localhost

Now open the URL https://localhost:8081/_explorer/index.html which should be secured

NOTE: If the URL shows as insecure, clear cache, review installed certificate and make sure it's not a browser issue by
trying to access the URL with incognito mode.