# ContainrBot

An API exposing docker and kubernetes deploys and a related chat bot to manipulate deploys.

## ContainrBotApi

### Environment Variables

* CONTAINER_LIST: A list of container data objects with these properties:
  * FriendlyName: The name used when interacting with the chat bot
  * ContainerName: The container name for the deployable
  * Namespace: The namespace for the deployable (kubernetes only)

Examples:

```
CONTAINER_LIST: "[{\"FriendlyName\": \"valheim\", \"Container\": \"deploy\", \"Namespace\": \"valheim-server\"}, {\"FriendlyName\": \"cncnet\", \"Container\": \"deploy\", \"Namespace\": \"cncnet-server\"}, {\"FriendlyName\": \"satisfactory\", \"Container\": \"deploy\", \"Namespace\": \"satisfactory-server\"}]"
```

## ContainrBot

### Environment Variables

* BOT_TOKEN: Token for the chat bot (Discord Bot Token, etc)
* CONTAINRBOTAPI_BASEURL: Base url of GameServerApi

## Technologies

* NET 10.0
* Kubernetes-client for Kubernetes management
* NetCord for setting up the bot interactions with Discord