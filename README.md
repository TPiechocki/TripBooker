# TripBooker

[![CI](https://github.com/TPiechocki/TripBooker/actions/workflows/docker_CI.yml/badge.svg?branch=develop)](https://github.com/TPiechocki/TripBooker/actions/workflows/docker_CI.yml)

## Deploy locally using docker-compose
Whole application together with its databases can be deployed for development locally using single command:
```
docker-compose -f .\docker-compose.developer.yml up -d --build
```

Option build is optional, but recomennded and this option forces to build all applications in composition.
