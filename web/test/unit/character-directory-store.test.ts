import { createPinia, setActivePinia } from 'pinia'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { searchCharacters } from '../../app/services/admin-api'
import { useCharacterDirectoryStore } from '../../app/stores/character-directory'

vi.mock('../../app/services/admin-api', () => ({ searchCharacters: vi.fn() }))

describe('Character directory store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.mocked(searchCharacters).mockReset()
  })

  it('loads character data for its current view state', async () => {
    vi.mocked(searchCharacters).mockResolvedValue({
      items: [
        {
          id: 'character-1',
          name: 'Hero',
          accountId: 'account-1',
          username: 'PlayerOne',
          raceId: 0,
          raceName: 'Human',
          sexId: 0,
          sexName: 'Male',
          baseClassId: 0,
          baseClassName: 'Fighter',
          activeClassId: 0,
          activeClassName: 'Fighter',
          level: 20,
          experience: 1000,
          createdAt: '2026-08-10T12:00:00Z',
          deleteAfter: null,
          status: 'active'
        }
      ],
      total: 1,
      page: 3,
      pageSize: 10
    })
    const store = useCharacterDirectoryStore()
    store.query = 'Hero'
    store.page = 3
    store.pageSize = 10

    await store.load()

    expect(searchCharacters).toHaveBeenCalledWith({
      query: 'Hero',
      page: 3,
      pageSize: 10
    })
    expect(store.items[0]?.name).toBe('Hero')
    expect(store.total).toBe(1)
    expect(store.loading).toBe(false)
  })

  it('exposes a stable error and clears loading when a request fails', async () => {
    vi.mocked(searchCharacters).mockRejectedValue(new Error('Unavailable'))
    const store = useCharacterDirectoryStore()

    await store.load()

    expect(store.error).toBe(
      'Player characters could not be loaded from the Admin API.'
    )
    expect(store.loading).toBe(false)
  })
})
