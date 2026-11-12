# Localization inspector plugin

Localization's drawers for Hefesto's web inspector: a `[LocalizationKeySelector]` string picks from
the project's localization keys, and shows what the chosen key says in the first language. The keys
come from this module's `localization.keys` request, in `Editor/Hefesto`. Unity finds this folder by
its `hefesto-plugin.json` and the inspector page loads `dist/plugin.js`, so the module brings its
own look without the project's inspector app changing. See
[Custom web editors](https://github.com/platinio/Unity-Hefesto/blob/main/Documentation~/guides/custom-web-editors.md#a-modules-own-plugin).

| Command                  | What it does                                                    |
| ------------------------ | --------------------------------------------------------------- |
| `npm test`               | Runs the drawers' tests                                         |
| `npm run build`          | Checks the types and builds `dist/plugin.js` and `plugin.css`   |
| `npm run watch`          | Builds on every save; the open inspector page reloads by itself |
| `npm run update-hefesto` | Rebuilds Hefesto's packages after updating it                   |

| File                  | What it holds                                                   |
| --------------------- | --------------------------------------------------------------- |
| `hefesto-plugin.json` | The plugin's id and the plugin API it is built for              |
| `plugin.ts`           | What the plugin adds, passed to `definePlugin`                  |
| `PropertyDrawers/`    | Drawers: each draws one field, wherever it appears              |
| `dist/`               | The build. Commit it, so people who use the module need no Node |
