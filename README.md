# Unity-Localization

## Web inspector

In Hefesto's web inspector, a `[LocalizationKeySelector]` string field is a searchable list of the project's localization keys, with what the chosen key says in the first language beneath it, and a ► button that opens the item holding that text. The list comes from this module's plugin in `Web~/Inspector`, and the keys from its `localization.keys` request in `Editor/Hefesto`, which needs Hefesto in the project.
