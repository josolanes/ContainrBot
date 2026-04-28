# ContainrBot

An extensible system allowing one of various container orchestrators to be controlled from one of various chat platforms.

ContainrBot is made up of the ContainrBot chatbot and the ContainrBotApi.

The ContainrBot is designed to be easily extended and currently supports the following chat platforms:
* Discord

ContainrBotApi is also designed to be easily extended and currently supports the following container orchestrators:
* Docker
* Kubernetes
 
If you'd like ContainrBot to support additional chat services and orchestrators, please create an [create an issue](https://github.com/josolanes/ContainrBot/issues).

**All [contributions](https://github.com/josolanes/ContainrBot?tab=contributing-ov-file#introduction) are welcome!** Please feel free to [ask questions](https://github.com/josolanes/ContainrBot/discussions), submit [issues](https://github.com/josolanes/ContainrBot/issues), and [make code changes](https://github.com/josolanes/ContainrBot?tab=contributing-ov-file#introduction) yourself!

## The ContainrBotApi

The ContainrBotApi is responsible for interacting with the chosen orchestrator (Docker, Kubernetes, etc.).

### Environment Variables

#### ORCHESTRATOR
The orchestrator the ContainrBotApi will interact with. Currently supported options are:
* Docker
* Kubernetes

#### CONTAINER_LIST
A JSON array of container objects with these properties:
* FriendlyName: The name used when interacting with the ContainrBot
* ContainerName: The container name for the deployable
* Namespace: The namespace for the deployable (_optional_, orchestrator-dependent)

Examples:
```
ORCHESTRATOR: docker
```

```
CONTAINER_LIST: "[{\"FriendlyName\": \"valheim\", \"ContainerName\": \"deploy\", \"Namespace\": \"valheim-server\"}, {\"FriendlyName\": \"cncnet\", \"ContainerName\": \"deploy\", \"Namespace\": \"cncnet-server\"}, {\"FriendlyName\": \"satisfactory\", \"ContainerName\": \"deploy\", \"Namespace\": \"satisfactory-server\"}]"
```

## The ContainrBot

The ContainrBot is responsible for interacting with the chat platform and is what you'll often use directly. The ContainrBot
calls the ContainrBotApi asking it to interact with containers based on your chatbot commands.

### Environment Variables

#### CHATBOT
The chat platform the ContainrBot will interact with. Currently supported options are:
* Discord

#### CONTAINRBOTAPI_BASEURL
The base URL of GameServerApi. <u>**_This should be a local IP address_**</u> as ContainrBotApi is not
secured or intended to be reached externally. Instead, external access to your containers should be through the ContainrBot
chatbot

### Secret Variables

#### bot-token
This is the token used to interact with the chosen chat platform

## Technologies In Use

* NET 10.0
* Kubernetes-client for Kubernetes management
* Docker.DotNet for Docker management
* NetCord for setting up the bot interactions with Discord

# Docker Compose Example

```yaml
services:
  containrbot:
    image: ghcr.io/josolanes/containrbot/containrbot:latest
    environment:
      CHATBOT: discord
      CONTAINRBOTAPI_BASEURL: containerbotapi
    secrets:
      - bot-token
          
  containrbotapi:
    image: ghcr.io/josolanes/containrbot/containrbotapi:latest
    ports:
      - "8080:8080"
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock
    environment:
      ORCHESTRATOR: docker
      CONTAINER_LIST: "[{\"FriendlyName\": \"hello\", \"ContainerName\": \"hello-world\", \"Namespace\": \"\"}]"
      
  hello-world:
    container_name: hello-world
    image: nginx

secrets:
  bot-token:
    file: ./bot-token.txt
# ./bot-token.txt will contain your chat platform token
# In this example, it would be in the same path as your docker-compose.yaml
```