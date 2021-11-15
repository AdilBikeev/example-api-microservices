# Stock-Api-Microservices

### Архитектура решения
![Схема](https://github.com/AdilBikeev/stock-api-microservices/blob/master/images/Solution-Architecture.png)

### Архитектура Kubernetes
![Схема](https://github.com/AdilBikeev/stock-api-microservices/blob/master/images/Kubernetes-Architecture.png)

### Архитектура сервисов

1. PlatformService
![Схема](https://github.com/AdilBikeev/stock-api-microservices/blob/master/images/Platform-Service-Architecture.png)

2. CommandService
![Схема](https://github.com/AdilBikeev/stock-api-microservices/blob/master/images/Command-Service-Architecture.png)

## Стех технологий
* .NET 5
* ASP.NET Core Web Api
* Entity Framework Core
* gPRC
* RabbitMQ
* AutoMapper
* Docker
* Kubernetes


## Get Started
1.1 Заходим  в папку PlatformService и выполняем комнду ```docker build -t <dockerHubID>/platformservice```

1.2 Заходим  в папку CommandService и выполняем комнду ```docker build -t <dockerHubID>/commandservice```

1.3 Заходим в папку K8S и выполняем подряд 4 команды

```kubectl apply -f mssql-depl```

```kubectl apply -f rabbitmq-depl```

```kubectl apply -f commands-depl```

```kubectl apply -f platforms-depl```
