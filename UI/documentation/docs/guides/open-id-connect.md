
# Open Id Connect

Agora does not have any user/account management included, you are expected to configure your own.
This can be a KeyCloak instance, Microsoft, anything that follows the OpenId Connect protocol.

## Expected layout

Agora expects the following layout;

2 clients; one for the backend (`agora-api`) and one for the WPA (`agora`). The `agora` client
should grant an audience to `agora-api`. Permissions are given by granted the correct roles to users; see [Roles and Permissions](./roles-and-permissions)
for more information.

## Known issues

Currently, a lot of your OpenId Connect configuration is hardcoded. We want to open this up, so have more freedom.

### Limitations

Only providers that include roles in the same way as `KeyCloak` are `Azure` will currently work. If you are a developer
and want support for another scheme; checkout [contributing](../dev/contributing) and [Dev Notes on OpenIdConnect](../dev/local-open-id-connect#others)
