
# Locally hosted OpenId Connect server

Agora requires you to set up an OpenId Connect provider,
this is also true for development as local account are not a thing.

While you can set up any you want, as long as it follows the structure as explained [here](../guides/open-id-connect).

## KeyCloak

We've used a keycloak (dev) instance in docker during development. 
```yaml
services:
  keycloak:
    image: quay.io/keycloak/keycloak
    container_name: keycloak
    environment:
      - KEYCLOAK_ADMIN=admin
      - KEYCLOAK_ADMIN_PASSWORD=admin
      - KEYCLOAK_HTTP_PORT=8080
    ports:
      - "8080:8080"
    command:
      - start-dev
    volumes:
      - keycloak_h2_data:/opt/keycloak/data/
    restart: always

volumes:
  keycloak_h2_data:
```

### Setup

With the following setup steps, 
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
- Choose `KeyCloak` as provider, and set url to `http://localhost:8080/realm/dev-realm` on first setup screen
- Roles for authorization
    - In agora-api -> roles
    - Add wanted roles -> See [Roles and permissions](../guides/roles-and-permissions)
    - To your user, add the role

## Others

As mentioned above, other providers should work fine if they include roles like `KeyCloak` or `Azure` in the JWT token.
You can find the ClaimsTransformers [here](https://github.com/Fesaa/Agora/tree/master/API/Helpers/RoleClaimTransformers). If you've got a working configuration; don't hesitate to edit this page.