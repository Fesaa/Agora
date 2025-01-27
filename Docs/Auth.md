# Auth quick

Quick recap of what I did with KeyCloak so I don't forget. And other can copy

Docker command
```zsh
docker run -p 8080:8080 -e KEYCLOAK_ADMIN=admin -e KEYCLOAK_ADMIN_PASSWORD=admin quay.io/keycloak/keycloak:latest start-dev
```

- Login
- Make new realm (top left)
- Create new client
  - Be sure to set redirect urls without https!! Caused lots of probmlems for me
  - Redirect: http://localhost:5050/signin-oidc
  - Logout: http://localhost:5050/signout-callback-oidc
  - Origin: http://localhost:5050
- Enable client auth => Cred. tab appears
- Copy secrety
- Add user
