# GameChatBot

An API exposing game server kubernetes containers and a related chat bot to manipulate game servers.

## GameServerApi

### Environment Variables

* GAME_SERVERS_LIST: A list of game server data objects with these properties:
  * FriendlyName: The name used when interacting with the chat bot
  * DeployName: The deploy name for the game server
  * Namespace: The namespace for the game server

Examples:

```GAME_SERVER_LIST: "[{\"FriendlyName\": \"valheim\", \"DeployName\": \"deploy\", \"Namespace\": \"valheim-server\"}, {\"FriendlyName\": \"cncnet\", \"DeployName\": \"deploy\", \"Namespace\": \"cncnet-server\"}, {\"FriendlyName\": \"satisfactory\", \"DeployName\": \"deploy\", \"Namespace\": \"satisfactory-server\"}]"```

## GameChatBot

### Environment Variables

* BOT_TOKEN: Token for the chat bot
* GAMESERVERAPI_BASEURL: Base url of GameServerApi

## Technologies

* NET 10.0
* Kubernetes-client