# Auth quick

Quick recap of what I did with KeyCloak so I don't forget. And other can copy

Docker command
```zsh
docker compose -f compose.auth.yaml up -d
```

The setup is as follows:

2 clients, one for the frontend (agora) and one for the backend (agora-api).
The client for the frontend should give a role that grants an audience to agora-api.

Quick steps;

- Loging (admin, admin)
- Create new realm
- Create agora client
  - LEAVE client auth off
  - root: http://localhost:4200
  - redirect: http://localhost:4200/callback
  - origin: http://localhost:4200
- Create api client
- Create client scope 
  - name doesn't matter, I use `agora-api-audience`
  - add mapper
    - by configuration
    - audience
    - name: `agora-api` (can be whatever)
    - include Client Audience: `agora-api`
- In `agora` client
  - Client scopes
  - Add client scope
  - Select agora-api-audience
  - Set default
- Edit DB and set in ServerSettings Key 0, Value: `http://localhost:8080/realm/dev-realm`
- Roles for authorization
  - In agora-api -> roles
  - Add roles we have
    - admin
  - To your user, add the role