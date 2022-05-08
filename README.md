# TripBooker

[![CI](https://github.com/TPiechocki/TripBooker/actions/workflows/docker_CI.yml/badge.svg?branch=develop)](https://github.com/TPiechocki/TripBooker/actions/workflows/docker_CI.yml)

## Deploy locally using docker-compose
Whole application together with its databases can be deployed for development locally using single command:
```
docker-compose -f .\docker-compose.developer.yml up -d --build
```

Option build is optional, but recomennded and this option forces to build all applications in composition.


## Build and deploy on KASK cluster
Deploy stack:
```
sudo docker stack deploy -c docker-compose.yml RSWW_175641
```

Tunneling:
```
ssh -L 17564:actina15.maas:17564 -L 17565:actina15.maas:17565 -N -f rsww@172.20.83.101
```