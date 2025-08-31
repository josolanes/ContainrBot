# GameChatBot

An API exposing game server docker containers and a related chat bot to manipulate game servers.

## GameServerApi

### Environment Variables

* GAME_SERVERS_LIST: A list of game server data objects with these properties:
  * FriendlyName: The name used when interacting with the chat bot
  * ContainerName: The container name
* TOKEN: The token to enable clients to reach the API

Examples:

```"GAME_SERVER_LIST": '[{"FriendlyName": "valheim", "ContainerName": "valheim-valheim-crossplay-1"}, {"FriendlyName": "cncnet", "ContainerName": "cncnet-server"}, {"FriendlyName": "satisfactory", "ContainerName": "satisfactory-server"}]'```

## GameChatBot

### Environment Variables

* GAMESERVERAPI_TOKEN: Token for the GameServerApi
* BOT_TOKEN: Token for the chat bot

## Technologies

* NET 9.0
* NetCord