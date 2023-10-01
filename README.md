# Urll
Url shortening service

## Architecture
```mermaid
flowchart TB
internet([Internet])
telegram([Telegram])
forwarder[Forwarder]
bot[Telegram Bot]
api[Links Service]
redis[(Redis)]

internet --> forwarder
telegram --> bot

subgraph Urll
forwarder --> api
bot --> api
api --> redis
end
```


## Telegram bot commands

### /start
Show help

### {url} {code}
Add Link

### /list
List all Links

### /get {code}
Return Link by code

### /delete {code}
Delete Link