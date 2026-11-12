import type { UnityQuestions } from '@hefesto/inspector';
import { useCallback, useEffect, useRef, useState } from 'react';

/** One key, as this module's localization.keys request answers it. */
export interface LocalizationKey {
  key: string;
  /** The item holding the key's text in the first language, which the web inspector opens it by. */
  id: string;
  text: string;
  language: string | null;
}

export const KEYS_REQUEST = 'localization.keys';

/** The project's keys as one field last heard them. */
export type KeyList =
  | { state: 'loading' }
  | { state: 'loaded'; keys: LocalizationKey[] }
  | { state: 'failed'; problem: string };

/**
 * One answer for the whole page, since many fields pick keys. Asking again while a question is on its way
 * shares it.
 */
export function createKeyCache(unity: UnityQuestions) {
  let answer: Promise<LocalizationKey[]> | null = null;
  let isOnItsWay = false;

  function ask(): Promise<LocalizationKey[]> {
    const asked = unity.bridge.requestCustom<LocalizationKey[]>(KEYS_REQUEST);
    answer = asked;
    isOnItsWay = true;
    asked.then(
      () => {
        if (answer === asked) isOnItsWay = false;
      },
      () => {
        // A newer answer may already be kept; only this one is forgotten.
        if (answer !== asked) return;
        answer = null;
        isOnItsWay = false;
      },
    );
    return asked;
  }

  return {
    keys: (): Promise<LocalizationKey[]> => answer ?? ask(),
    refresh: (): Promise<LocalizationKey[]> => (answer !== null && isOnItsWay ? answer : ask()),
  };
}

export type KeyCache = ReturnType<typeof createKeyCache>;

/** One field's view of the keys. Only the latest of several requests is kept, so a slow answer never replaces a newer one. */
export function useLocalizationKeys(cache: KeyCache): { list: KeyList; refresh(): void } {
  const [list, setList] = useState<KeyList>({ state: 'loading' });
  const latest = useRef(0);

  const keep = useCallback((answer: Promise<LocalizationKey[]>) => {
    const request = ++latest.current;
    answer.then(
      (keys) => request === latest.current && setList({ state: 'loaded', keys }),
      (error: unknown) =>
        request === latest.current &&
        setList({
          state: 'failed',
          problem: error instanceof Error ? error.message : String(error),
        }),
    );
  }, []);

  useEffect(() => {
    keep(cache.keys());
    return () => {
      latest.current++;
    };
  }, [cache, keep]);

  const refresh = useCallback(() => keep(cache.refresh()), [cache, keep]);
  return { list, refresh };
}
