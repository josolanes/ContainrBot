# ContainrBot

An API exposing docker and kubernetes deploys and a related chatbot to manipulate deploys.

## ContainrBotApi

### Environment Variables

* CONTAINER_LIST: A list of container data objects with these properties:
	* FriendlyName: The name used when interacting with the chatbot
	* ContainerName: The container name for the deployable
	* Namespace: The namespace for the deployable (kubernetes only)

Examples:

```
CONTAINER_LIST: "[{\"FriendlyName\": \"valheim\", \"ContainerName\": \"deploy\", \"Namespace\": \"valheim-server\"}, {\"FriendlyName\": \"cncnet\", \"ContainerName\": \"deploy\", \"Namespace\": \"cncnet-server\"}, {\"FriendlyName\": \"satisfactory\", \"ContainerName\": \"deploy\", \"Namespace\": \"satisfactory-server\"}]"
```

## ContainrBot

### Environment Variables

* BOT_TOKEN: Token for the chatbot (Discord Bot Token, etc.)
* CONTAINRBOTAPI_BASEURL: Base URL of GameServerApi

## Technologies

* NET 10.0
* Kubernetes-client for Kubernetes management
* NetCord for setting up the bot interactions with Discord