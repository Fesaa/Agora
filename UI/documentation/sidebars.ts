import type {SidebarsConfig} from '@docusaurus/plugin-content-docs';

// This runs in Node.js - Don't use client-side code here (browser APIs, JSX...)

/**
 * Creating a sidebar enables you to:
 - create an ordered group of docs
 - render a sidebar for each doc of that group
 - provide next/previous navigation

 The sidebars can be generated from the filesystem, or explicitly defined here.

 Create as many sidebars as you want.
 */
const sidebars: SidebarsConfig = {
  docsSidebar: [
    {
      type: 'doc',
      id: 'quick-start',
      label: 'Quick Start'
    },
    {
      type: "category",
      label: "Guides",
      collapsed: false,
      items: [
        {
          type: 'doc',
          id: 'guides/setup',
          label: 'Setup'
        },
        {
          type: 'doc',
          id: 'guides/open-id-connect',
          label: 'OpenId Connect'
        },
        {
          type: 'doc',
          id: 'guides/roles-and-permissions',
          label: 'Roles and Permissions'
        },
      ]
    },
    {
      type: 'category',
      label: 'Branding',
      items: [
        {
          type: 'doc',
          id: 'branding/themes',
          label: 'Themes'
        }
      ]
    },
    {
      type: 'category',
      label: 'Development',
      items: [
        {
          type: 'doc',
          id: 'dev/contributing',
          label: 'Contributing'
        },
        {
          type: 'doc',
          id: 'dev/local-open-id-connect',
          label: 'OpenIdConnect Locally'
        },
      ],
    },
  ],
};

export default sidebars;
