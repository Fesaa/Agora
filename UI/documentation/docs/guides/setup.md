# Setup

Getting started with Agora is super easy! Agora runs in a single Docker container, it uses sqlite as database, so you don't have to worry about maintaining one.

The following docker compose will work nicely.

```yaml
services:
  agora:
    image: ghcr.io/fesaa/agora:latest
    restart: always
    volumes:
      - /path/to/config:/agora/config
```

### Authentication

Agora does require access to an OAuth provider, you may have to change the docker compose depending on where you host this.
See the [OpenID Connect guide](./open-id-connect.md) for more information.