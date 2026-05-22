# ContainrBot

<p align="center">
  <a href="https://github.com/josolanes/containrbot/releases/latest"><img src="https://img.shields.io/github/v/release/josolanes/containrbot?label=latest%20release&color=blue" alt="Latest Release"></a>
  <a href="https://github.com/josolanes/containrbot/actions/workflows/docker-publish.yml"><img src="https://img.shields.io/github/actions/workflow/status/josolanes/containrbot/docker-publish.yml?label=build" alt="Build Status"></a>
  <a href="https://opensource.org/license/gpl-3.0"><img src="https://img.shields.io/badge/license-%20%20GNU%20GPLv3%20-green" alt="License: GPLv3"></a>
  <a href="https://github.com/josolanes/containrbot/stargazers"><img src="https://img.shields.io/github/stars/josolanes/containrbot?style=flat" alt="GitHub Stars"></a>
  <a href="https://github.com/josolanes/containrbot/graphs/contributors"><img src="https://img.shields.io/github/contributors/josolanes/containrbot" alt="GitHub Contributors"></a>
</p>

An extensible system that allows one of various container orchestrators to be controlled from one of various chat platforms.

ContainrBot is made up of the ContainrBot chatbot and the ContainrBotApi.

The ContainrBot is designed to be easily extended and currently supports the following chat platforms:
* Discord
* Slack

ContainrBotApi is also designed to be easily extended and currently supports the following container orchestrators:
* Docker
* Kubernetes

Common use cases:
* Enabling a group of friends to control a game server
* Restarting a finicky container
* Starting a high-load container only when needed

If you'd like ContainrBot to support additional chat services and orchestrators, please [create an issue](https://github.com/josolanes/ContainrBot/issues).

**All [contributions](https://github.com/josolanes/ContainrBot?tab=contributing-ov-file#introduction) are welcome!** Please feel free to [ask questions](https://github.com/josolanes/ContainrBot/discussions), submit [issues](https://github.com/josolanes/ContainrBot/issues), and [make code changes](https://github.com/josolanes/ContainrBot?tab=contributing-ov-file#introduction) yourself!

## Chatbot Commands
* List
  * `/containr list`
  * Lists the configured containers and their status: running, not running, not deployed
* Start
  * `/containr start {container}`
  * Starts the `{container}`
* Stop
  * `/containr stop {container}`
  * Stops the `{container}`
* Restart
  * `/containr restart {container}`
  * Restarts the `{container}`

## The ContainrBotApi

The ContainrBotApi is responsible for interacting with the chosen orchestrator (Docker, Kubernetes, etc.).

### ContainrBotApi Environment Variables

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
```yaml
ORCHESTRATOR: docker
```

```yaml
CONTAINER_LIST: "[{\"FriendlyName\": \"valheim\", \"ContainerName\": \"valheim\"}, {\"FriendlyName\": \"cncnet\", \"ContainerName\": \"cncnet\"}, {\"FriendlyName\": \"satisfactory\", \"ContainerName\": \"satisfactory\"}]"
```

## The ContainrBot

The ContainrBot is responsible for interacting with the chat platform and is what you'll often use directly. The ContainrBot
calls the ContainrBotApi asking it to interact with containers based on your chatbot commands.

### ContainrBot Environment Variables

#### CHATBOT
The chat platform the ContainrBot will interact with. Currently supported options are:
* Discord
* Slack

#### CONTAINRBOTAPI_BASEURL
The base URL of ContainrBotApi. <u>**_This should be a local IP address_**</u> as ContainrBotApi is not
secured or intended to be reached externally. Instead, external access to your containers should be through the ContainrBot
chatbot

### Secret Variables

#### bot-token
This is the token used to interact with the chosen chat platform

## Technologies In Use

* NET 10.0

### Chat Platform Libraries
* [NetCord](https://github.com/NetCordDev/NetCord) for setting up the bot interactions with Discord
* [SlackNet](https://github.com/soxtoby/SlackNet) for setting up the bot interactions with Slack

### Container Orchestration Libraries
* [Docker.DotNet](https://github.com/dotnet/Docker.DotNet) for Docker management
* [Kubernetes-client](https://github.com/kubernetes-client/csharp) for Kubernetes management

## Docker Compose Example

```yaml
services:
  containrbot:
    image: ghcr.io/josolanes/containrbot/containrbot:latest
    environment:
      CHATBOT: discord
      CONTAINRBOTAPI_BASEURL: "http://containrbotapi:8080/"
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
