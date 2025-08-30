# GameChatBot

An API exposing game server docker containers and a related chat bot to manipulate game servers.

## Environment Variables

* GAME_SERVERS_LIST: A list of game server data objects with these properties:
  * FriendlyName: The name used when interacting with the chat bot
  * ContainerName: The container name
  * GameIdentifier: [Optional] The property name of the game specific identifier
  * IdentifierValue: [Optional] The value of the game identifier to match to
  * Password: The game server password if relevant

## Technologies

* NET 9.0
* NetCord