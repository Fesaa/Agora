

export type OpenIdConnectInfo = {
  authority: string;
  provider: OpenIdProvider;
}

export enum OpenIdProvider {
  KeyCloak = 0,
  AzureAd = 1,
}
