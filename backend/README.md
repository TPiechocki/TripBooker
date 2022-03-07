# Backend components

## Docker

### Building docker images
In order to build local docker images, any Dockerfile should be used from `backend folder`. So building the image would for `tripbooker-example` would look like

```
docker build . -t local/tripbooker-example -f TripBooker.Example/Dockerfile
```

where `local/tripbooker-example` is image name and and `TripBooker.Example/Dockerfile` is path to Dockerfile.

After this image can be run using standard docker run command like

```
docker run -d -p 8081:80 --name tripbooker-example local/tripbooker-example
```

which would expose the example app on 8081 port.