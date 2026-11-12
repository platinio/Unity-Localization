import { createRendererRegistry, PropertyForm, type PropertyNode } from '@hefesto/form';
import {
  rendererRegistrationsOf,
  UnityQuestionsProvider,
  type UnityQuestions,
} from '@hefesto/inspector';
import { builtInWidgets } from '@hefesto/widgets';
import { act, render, renderHook, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';

import plugin from '../plugin.ts';
import {
  KEYS_REQUEST,
  useLocalizationKeys,
  type KeyCache,
  type LocalizationKey,
} from './localizationKeys.ts';

const registry = createRendererRegistry([...builtInWidgets, ...rendererRegistrationsOf([plugin])]);

const BURNING: LocalizationKey = {
  key: 'Statuses/Burning',
  id: 'id-1',
  text: 'Burning',
  language: 'English',
};
const FROZEN: LocalizationKey = {
  key: 'Statuses/Frozen',
  id: 'id-2',
  text: 'Frozen solid',
  language: 'English',
};
const KEYS = [BURNING, FROZEN];

/** A Unity that answers the keys request, and whose connection a test can drop and bring back. */
function fakeUnity(requestCustom = vi.fn(async () => KEYS), isConnected = false) {
  let status = { connected: isConnected, playing: false, compiling: false };
  const listeners = new Set<() => void>();
  const unity = {
    openAsset: vi.fn(),
    bridge: {
      requestCustom,
      subscribeStatus: (listener: () => void) => {
        listeners.add(listener);
        return () => listeners.delete(listener);
      },
      getStatus: () => status,
    },
    connect(connected: boolean) {
      status = { ...status, connected };
      act(() => listeners.forEach((listener) => listener()));
    },
  };
  return unity as unknown as UnityQuestions & typeof unity;
}

function keyField(value: string, overrides: Partial<PropertyNode> = {}): PropertyNode {
  return {
    path: 'nameKey',
    label: 'Name Key',
    kind: 'string',
    value,
    hints: { attributes: ['LocalizationKeySelector'] },
    ...overrides,
  };
}

function draw(unity: UnityQuestions, ...fields: PropertyNode[]) {
  const onChange = vi.fn();
  render(
    <UnityQuestionsProvider value={unity}>
      <PropertyForm
        tree={{ path: '', label: 'Asset', kind: 'object', value: null, children: fields }}
        onChange={onChange}
        registry={registry}
      />
    </UnityQuestionsProvider>,
  );
  return onChange;
}

async function openList() {
  await userEvent.click(screen.getByRole('button', { name: /^Name Key:/ }));
  return within(await screen.findByRole('listbox', { name: 'Localization keys' }));
}

describe('LocalizationKeySelector', () => {
  it('names the picker by its field and key, and says what the key says in the first language', async () => {
    const unity = fakeUnity();
    draw(unity, keyField('Statuses/Burning'));

    expect(screen.getByRole('button', { name: 'Name Key: Statuses/Burning' })).toBeInTheDocument();
    expect(await screen.findByRole('note')).toHaveTextContent('“Burning” (English)');
    expect(unity.bridge.requestCustom).toHaveBeenCalledWith(KEYS_REQUEST);
  });

  it('says so when no item has the key', async () => {
    draw(fakeUnity(), keyField('Statuses/Renamed'));

    expect(await screen.findByRole('note')).toHaveTextContent(
      'No localization item has the key Statuses/Renamed.',
    );
  });

  it('shows None and no note for an empty key', async () => {
    const unity = fakeUnity();
    draw(unity, keyField(''));
    await waitFor(() => expect(unity.bridge.requestCustom).toHaveBeenCalled());

    expect(screen.getByRole('button', { name: 'Name Key: None' })).toBeInTheDocument();
    expect(screen.queryByRole('note')).not.toBeInTheDocument();
  });

  it('writes the picked key, and an empty key for None', async () => {
    const onChange = draw(fakeUnity(), keyField(''));

    await userEvent.click(
      await (await openList()).findByRole('option', { name: 'Statuses/Frozen' }),
    );
    await userEvent.click((await openList()).getByRole('option', { name: 'None' }));

    expect(onChange.mock.calls).toEqual([
      ['nameKey', 'Statuses/Frozen'],
      ['nameKey', ''],
    ]);
  });

  it('finds a key by its text as well as by its name, and Enter picks the only match', async () => {
    const onChange = draw(fakeUnity(), keyField(''));
    const list = await openList();
    await list.findByRole('option', { name: 'Statuses/Frozen' });

    await userEvent.type(screen.getByLabelText('Search Name Key'), 'solid');
    expect(list.queryByRole('option', { name: 'Statuses/Burning' })).not.toBeInTheDocument();
    await userEvent.keyboard('{Enter}');

    expect(onChange).toHaveBeenCalledWith('nameKey', 'Statuses/Frozen');
  });

  it("opens the item holding the key's text", async () => {
    const unity = fakeUnity();
    draw(unity, keyField('Statuses/Burning'));

    await userEvent.click(await screen.findByRole('button', { name: 'Open Statuses/Burning' }));

    expect(unity.openAsset).toHaveBeenCalledWith('id-1');
  });

  it('asks Unity once for every key field on the page', async () => {
    const unity = fakeUnity();
    draw(
      unity,
      keyField('Statuses/Burning'),
      keyField('Statuses/Frozen', { path: 'descriptionKey', label: 'Description Key' }),
    );

    await screen.findAllByRole('note');

    expect(unity.bridge.requestCustom).toHaveBeenCalledTimes(1);
  });

  it('asks again when a list opens, so a key added in Unity shows up', async () => {
    const requestCustom = vi.fn().mockResolvedValueOnce([BURNING]).mockResolvedValue(KEYS);
    draw(fakeUnity(requestCustom), keyField(''));
    await waitFor(() => expect(requestCustom).toHaveBeenCalled());

    const list = await openList();

    expect(await list.findByRole('option', { name: 'Statuses/Frozen' })).toBeInTheDocument();
  });

  it('asks again for a key the list read earlier does not have yet, and again after Unity reconnects', async () => {
    const requestCustom = vi
      .fn()
      .mockResolvedValueOnce([BURNING])
      .mockResolvedValueOnce([BURNING])
      .mockResolvedValue(KEYS);
    const unity = fakeUnity(requestCustom, true);
    draw(unity, keyField('Statuses/Frozen'));
    await waitFor(() => expect(requestCustom).toHaveBeenCalledTimes(2));

    unity.connect(false);
    unity.connect(true);

    expect(await screen.findByRole('note')).toHaveTextContent('“Frozen solid”');
  });

  it("can't be changed when the field is read-only", () => {
    draw(
      fakeUnity(),
      keyField('', { hints: { attributes: ['LocalizationKeySelector'], readOnly: true } }),
    );

    expect(screen.getByRole('button', { name: 'Name Key: None' })).toBeDisabled();
  });

  it('says why when Unity fails, and asks again on Retry', async () => {
    const requestCustom = vi
      .fn()
      .mockRejectedValueOnce(new Error('No handler for localization.keys'))
      .mockRejectedValueOnce(new Error('No handler for localization.keys'))
      .mockResolvedValue(KEYS);
    draw(fakeUnity(requestCustom), keyField(''));

    await userEvent.click(screen.getByRole('button', { name: 'Name Key: None' }));
    await userEvent.click(await screen.findByRole('button', { name: 'Retry' }));

    expect(await screen.findByRole('option', { name: 'Statuses/Frozen' })).toBeInTheDocument();
  });
});

describe("one field's keys", () => {
  it('keep the latest answer when an older one arrives after it', async () => {
    let answerTheFirst: (keys: LocalizationKey[]) => void = () => undefined;
    const slow = new Promise<LocalizationKey[]>((resolve) => (answerTheFirst = resolve));
    const cache: KeyCache = { keys: () => slow, refresh: () => Promise.resolve([FROZEN]) };
    const { result } = renderHook(() => useLocalizationKeys(cache));

    act(() => result.current.refresh());
    await waitFor(() => expect(result.current.list).toEqual({ state: 'loaded', keys: [FROZEN] }));
    await act(async () => answerTheFirst([]));

    expect(result.current.list).toEqual({ state: 'loaded', keys: [FROZEN] });
  });
});
