var hefestoModulePlugin = (function(_hefesto_inspector, _hefesto_primitives, react, react_jsx_runtime) {
	var LocalizationKeyDrawer_module_default = {
		field: "_field_d2r8m_1",
		row: "_row_d2r8m_8",
		choice: "_choice_d2r8m_14",
		list: "_list_d2r8m_20",
		option: "_option_d2r8m_35",
		search: "_search_d2r8m_52",
		muted: "_muted_d2r8m_56",
		problem: "_problem_d2r8m_61"
	};
	// PropertyDrawers/localizationKeys.ts
	var KEYS_REQUEST = "localization.keys";
	/**
	* One answer for the whole page, since many fields pick keys. Asking again while a question is on its way
	* shares it.
	*/
	function createKeyCache(unity) {
		let answer = null;
		let isOnItsWay = false;
		function ask() {
			const asked = unity.bridge.requestCustom(KEYS_REQUEST);
			answer = asked;
			isOnItsWay = true;
			asked.then(() => {
				if (answer === asked) isOnItsWay = false;
			}, () => {
				if (answer !== asked) return;
				answer = null;
				isOnItsWay = false;
			});
			return asked;
		}
		return {
			keys: () => answer ?? ask(),
			refresh: () => answer !== null && isOnItsWay ? answer : ask()
		};
	}
	/** One field's view of the keys. Only the latest of several requests is kept, so a slow answer never replaces a newer one. */
	function useLocalizationKeys(cache) {
		const [list, setList] = (0, react.useState)({ state: "loading" });
		const latest = (0, react.useRef)(0);
		const keep = (0, react.useCallback)((answer) => {
			const request = ++latest.current;
			answer.then((keys) => request === latest.current && setList({
				state: "loaded",
				keys
			}), (error) => request === latest.current && setList({
				state: "failed",
				problem: error instanceof Error ? error.message : String(error)
			}));
		}, []);
		(0, react.useEffect)(() => {
			keep(cache.keys());
			return () => {
				latest.current++;
			};
		}, [cache, keep]);
		return {
			list,
			refresh: (0, react.useCallback)(() => keep(cache.refresh()), [cache, keep])
		};
	}
	// PropertyDrawers/LocalizationKeyDrawer.tsx
	var NONE = "None";
	var caches = /* @__PURE__ */ new WeakMap();
	function cacheFor(unity) {
		let cache = caches.get(unity);
		if (cache === void 0) caches.set(unity, cache = createKeyCache(unity));
		return cache;
	}
	/**
	* Draws a `[LocalizationKeySelector]` string as a searchable list of the project's localization keys, as the
	* IMGUI popup does, with what the chosen key says in the first language beneath it.
	*/
	function LocalizationKeyDrawer({ node, readOnly, onChange }) {
		const unity = (0, _hefesto_inspector.useUnity)();
		const isConnected = (0, react.useSyncExternalStore)(unity.bridge.subscribeStatus, unity.bridge.getStatus).connected;
		const chosen = typeof node.value === "string" && node.value !== "" ? node.value : null;
		const { list, refresh } = useLocalizationKeys(cacheFor(unity));
		const [isOpen, setIsOpen] = (0, react.useState)(false);
		const [search, setSearch] = (0, react.useState)("");
		const keys = list.state === "loaded" ? list.keys : [];
		const chosenKey = keys.find((key) => key.key === chosen);
		useLookAgainWhenTheChoiceIsMissing({
			isMissing: list.state === "failed" || list.state === "loaded" && chosen !== null && chosenKey === void 0,
			choice: chosen ?? "",
			isConnected,
			refresh
		});
		const shown = keys.filter((key) => matches(key, search));
		const chosenName = chosen ?? NONE;
		function changeOpen(isOpening) {
			setIsOpen(isOpening);
			setSearch("");
			if (isOpening) refresh();
		}
		function pick(key) {
			onChange(key);
			setIsOpen(false);
		}
		function pickTheOnlyMatch(event) {
			const onlyMatch = shown.length === 1 ? shown[0] : void 0;
			if (event.key === "Enter" && onlyMatch !== void 0) pick(onlyMatch.key);
		}
		return /* @__PURE__ */ (0, react_jsx_runtime.jsxs)("span", {
			className: LocalizationKeyDrawer_module_default["field"],
			children: [/* @__PURE__ */ (0, react_jsx_runtime.jsxs)("span", {
				className: LocalizationKeyDrawer_module_default["row"],
				children: [/* @__PURE__ */ (0, react_jsx_runtime.jsx)(_hefesto_primitives.Popover, {
					open: isOpen,
					onOpenChange: changeOpen,
					trigger: /* @__PURE__ */ (0, react_jsx_runtime.jsx)(_hefesto_primitives.Button, {
						className: LocalizationKeyDrawer_module_default["choice"],
						disabled: readOnly,
						"aria-label": `${node.label}: ${chosenName}`,
						children: chosenName
					}),
					children: /* @__PURE__ */ (0, react_jsx_runtime.jsxs)("div", {
						className: LocalizationKeyDrawer_module_default["list"],
						children: [/* @__PURE__ */ (0, react_jsx_runtime.jsx)("input", {
							className: LocalizationKeyDrawer_module_default["search"],
							"aria-label": `Search ${node.label}`,
							placeholder: "Search keys and text",
							value: search,
							onChange: (event) => setSearch(event.target.value),
							onKeyDown: pickTheOnlyMatch
						}), list.state === "failed" ? /* @__PURE__ */ (0, react_jsx_runtime.jsxs)("p", {
							role: "alert",
							className: LocalizationKeyDrawer_module_default["problem"],
							children: [
								"Unity didn't list the keys: ",
								list.problem,
								" ",
								/* @__PURE__ */ (0, react_jsx_runtime.jsx)(_hefesto_primitives.Button, {
									variant: "quiet",
									onClick: refresh,
									children: "Retry"
								})
							]
						}) : /* @__PURE__ */ (0, react_jsx_runtime.jsxs)("ul", {
							role: "listbox",
							"aria-label": "Localization keys",
							children: [
								/* @__PURE__ */ (0, react_jsx_runtime.jsx)(Option, {
									isChosen: chosen === null,
									name: NONE,
									onPick: () => pick(""),
									children: NONE
								}),
								shown.map((key) => /* @__PURE__ */ (0, react_jsx_runtime.jsxs)(Option, {
									isChosen: key === chosenKey,
									name: key.key,
									onPick: () => pick(key.key),
									children: [/* @__PURE__ */ (0, react_jsx_runtime.jsx)("span", { children: key.key }), /* @__PURE__ */ (0, react_jsx_runtime.jsx)("span", {
										className: LocalizationKeyDrawer_module_default["muted"],
										children: key.text
									})]
								}, key.key)),
								list.state === "loading" && /* @__PURE__ */ (0, react_jsx_runtime.jsx)("li", {
									role: "presentation",
									className: LocalizationKeyDrawer_module_default["muted"],
									children: "Asking Unity…"
								})
							]
						})]
					})
				}), chosenKey !== void 0 && /* @__PURE__ */ (0, react_jsx_runtime.jsx)(_hefesto_primitives.Button, {
					variant: "quiet",
					"aria-label": `Open ${chosenKey.key}`,
					onClick: () => unity.openAsset(chosenKey.id),
					children: "►"
				})]
			}), /* @__PURE__ */ (0, react_jsx_runtime.jsx)(KeyNote, {
				chosen,
				chosenKey,
				isLoaded: list.state === "loaded"
			})]
		});
	}
	/**
	* An unknown key was most likely added in Unity after the list was read, and a failed list was most likely
	* asked for while Unity was compiling. Either is asked again once per key, and again each time Unity
	* reconnects.
	*/
	function useLookAgainWhenTheChoiceIsMissing({ isMissing, choice, isConnected, refresh }) {
		const lookedAgainFor = (0, react.useRef)(null);
		(0, react.useEffect)(() => {
			if (!isConnected) {
				lookedAgainFor.current = null;
				return;
			}
			if (!isMissing || lookedAgainFor.current === choice) return;
			lookedAgainFor.current = choice;
			refresh();
		}, [
			isMissing,
			choice,
			isConnected,
			refresh
		]);
	}
	function Option({ isChosen, name, onPick, children }) {
		return /* @__PURE__ */ (0, react_jsx_runtime.jsx)("li", {
			role: "option",
			"aria-selected": isChosen,
			"aria-label": name,
			tabIndex: 0,
			className: LocalizationKeyDrawer_module_default["option"],
			onClick: onPick,
			onKeyDown: (event) => {
				if (event.key === "Enter" || event.key === " ") {
					event.preventDefault();
					onPick();
				}
			},
			children
		});
	}
	function KeyNote({ chosen, chosenKey, isLoaded }) {
		if (chosen === null || !isLoaded) return null;
		if (chosenKey === void 0) return /* @__PURE__ */ (0, react_jsx_runtime.jsxs)("span", {
			role: "note",
			className: LocalizationKeyDrawer_module_default["problem"],
			children: [
				"No localization item has the key ",
				chosen,
				"."
			]
		});
		const languageSuffix = chosenKey.language !== null ? ` (${chosenKey.language})` : "";
		const emptyNote = chosenKey.language !== null ? `No text in ${chosenKey.language} yet.` : "No text yet.";
		return /* @__PURE__ */ (0, react_jsx_runtime.jsx)("span", {
			role: "note",
			className: LocalizationKeyDrawer_module_default["muted"],
			children: chosenKey.text === "" ? emptyNote : `“${chosenKey.text}”${languageSuffix}`
		});
	}
	function matches(key, search) {
		const wanted = search.trim().toLowerCase();
		return key.key.toLowerCase().includes(wanted) || key.text.toLowerCase().includes(wanted);
	}
	return (0, _hefesto_inspector.definePlugin)({
		id: "arcaneonyx.localization",
		drawers: [{
			id: "localization-key",
			attribute: "LocalizationKeySelector",
			component: LocalizationKeyDrawer
		}]
	});
})(hefesto.inspector, hefesto.primitives, hefesto.react, hefesto.jsxRuntime);
