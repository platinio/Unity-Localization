import type { RendererProps } from '@hefesto/form';
import { useUnity, type UnityQuestions } from '@hefesto/inspector';
import { Button, Popover } from '@hefesto/primitives';
import {
  useEffect,
  useRef,
  useState,
  useSyncExternalStore,
  type KeyboardEvent,
  type ReactNode,
} from 'react';

import styles from './LocalizationKeyDrawer.module.css';
import {
  createKeyCache,
  useLocalizationKeys,
  type KeyCache,
  type LocalizationKey,
} from './localizationKeys.ts';

const NONE = 'None';

// One cache per page, shared by every key field.
const caches = new WeakMap<UnityQuestions, KeyCache>();

function cacheFor(unity: UnityQuestions): KeyCache {
  let cache = caches.get(unity);
  if (cache === undefined) caches.set(unity, (cache = createKeyCache(unity)));
  return cache;
}

/**
 * Draws a `[LocalizationKeySelector]` string as a searchable list of the project's localization keys, as the
 * IMGUI popup does, with what the chosen key says in the first language beneath it.
 */
export function LocalizationKeyDrawer({ node, readOnly, onChange }: RendererProps) {
  const unity = useUnity();
  const isConnected = useSyncExternalStore(
    unity.bridge.subscribeStatus,
    unity.bridge.getStatus,
  ).connected;
  const chosen = typeof node.value === 'string' && node.value !== '' ? node.value : null;
  const { list, refresh } = useLocalizationKeys(cacheFor(unity));
  const [isOpen, setIsOpen] = useState(false);
  const [search, setSearch] = useState('');

  const keys = list.state === 'loaded' ? list.keys : [];
  const chosenKey = keys.find((key) => key.key === chosen);
  useLookAgainWhenTheChoiceIsMissing({
    isMissing:
      list.state === 'failed' ||
      (list.state === 'loaded' && chosen !== null && chosenKey === undefined),
    choice: chosen ?? '',
    isConnected,
    refresh,
  });

  const shown = keys.filter((key) => matches(key, search));
  const chosenName = chosen ?? NONE;

  function changeOpen(isOpening: boolean) {
    setIsOpen(isOpening);
    setSearch('');
    if (isOpening) refresh();
  }

  function pick(key: string) {
    onChange(key);
    setIsOpen(false);
  }

  function pickTheOnlyMatch(event: KeyboardEvent<HTMLInputElement>) {
    const onlyMatch = shown.length === 1 ? shown[0] : undefined;
    if (event.key === 'Enter' && onlyMatch !== undefined) pick(onlyMatch.key);
  }

  return (
    <span className={styles['field']}>
      <span className={styles['row']}>
        <Popover
          open={isOpen}
          onOpenChange={changeOpen}
          trigger={
            <Button
              className={styles['choice']}
              disabled={readOnly}
              aria-label={`${node.label}: ${chosenName}`}
            >
              {chosenName}
            </Button>
          }
        >
          <div className={styles['list']}>
            <input
              className={styles['search']}
              aria-label={`Search ${node.label}`}
              placeholder="Search keys and text"
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              onKeyDown={pickTheOnlyMatch}
            />
            {list.state === 'failed' ? (
              <p role="alert" className={styles['problem']}>
                Unity didn't list the keys: {list.problem}{' '}
                <Button variant="quiet" onClick={refresh}>
                  Retry
                </Button>
              </p>
            ) : (
              <ul role="listbox" aria-label="Localization keys">
                <Option isChosen={chosen === null} name={NONE} onPick={() => pick('')}>
                  {NONE}
                </Option>
                {shown.map((key) => (
                  <Option
                    key={key.key}
                    isChosen={key === chosenKey}
                    name={key.key}
                    onPick={() => pick(key.key)}
                  >
                    <span>{key.key}</span>
                    <span className={styles['muted']}>{key.text}</span>
                  </Option>
                ))}
                {list.state === 'loading' && (
                  <li role="presentation" className={styles['muted']}>
                    Asking Unity…
                  </li>
                )}
              </ul>
            )}
          </div>
        </Popover>
        {chosenKey !== undefined && (
          <Button
            variant="quiet"
            aria-label={`Open ${chosenKey.key}`}
            onClick={() => unity.openAsset(chosenKey.id)}
          >
            ►
          </Button>
        )}
      </span>
      <KeyNote chosen={chosen} chosenKey={chosenKey} isLoaded={list.state === 'loaded'} />
    </span>
  );
}

/**
 * An unknown key was most likely added in Unity after the list was read, and a failed list was most likely
 * asked for while Unity was compiling. Either is asked again once per key, and again each time Unity
 * reconnects.
 */
function useLookAgainWhenTheChoiceIsMissing({
  isMissing,
  choice,
  isConnected,
  refresh,
}: {
  isMissing: boolean;
  choice: string;
  isConnected: boolean;
  refresh(): void;
}) {
  const lookedAgainFor = useRef<string | null>(null);
  useEffect(() => {
    if (!isConnected) {
      lookedAgainFor.current = null;
      return;
    }
    if (!isMissing || lookedAgainFor.current === choice) return;
    lookedAgainFor.current = choice;
    refresh();
  }, [isMissing, choice, isConnected, refresh]);
}

function Option({
  isChosen,
  name,
  onPick,
  children,
}: {
  isChosen: boolean;
  name: string;
  onPick(): void;
  children: ReactNode;
}) {
  return (
    <li
      role="option"
      aria-selected={isChosen}
      aria-label={name}
      tabIndex={0}
      className={styles['option']}
      onClick={onPick}
      onKeyDown={(event) => {
        if (event.key === 'Enter' || event.key === ' ') {
          event.preventDefault();
          onPick();
        }
      }}
    >
      {children}
    </li>
  );
}

function KeyNote({
  chosen,
  chosenKey,
  isLoaded,
}: {
  chosen: string | null;
  chosenKey: LocalizationKey | undefined;
  isLoaded: boolean;
}) {
  if (chosen === null || !isLoaded) return null;
  if (chosenKey === undefined) {
    return (
      <span role="note" className={styles['problem']}>
        No localization item has the key {chosen}.
      </span>
    );
  }
  return (
    <span role="note" className={styles['muted']}>
      “{chosenKey.text}”{chosenKey.language !== null && ` (${chosenKey.language})`}
    </span>
  );
}

function matches(key: LocalizationKey, search: string): boolean {
  const wanted = search.trim().toLowerCase();
  return key.key.toLowerCase().includes(wanted) || key.text.toLowerCase().includes(wanted);
}
