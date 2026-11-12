import { definePlugin } from '@hefesto/inspector';

import { LocalizationKeyDrawer } from './PropertyDrawers/LocalizationKeyDrawer.tsx';

export default definePlugin({
  id: 'arcaneonyx.localization',
  drawers: [
    {
      id: 'localization-key',
      attribute: 'LocalizationKeySelector',
      component: LocalizationKeyDrawer,
    },
  ],
});
