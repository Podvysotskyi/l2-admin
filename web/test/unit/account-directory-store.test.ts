import { createPinia, setActivePinia } from 'pinia'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { searchAccounts } from '../../app/services/admin-api'
import { useAccountDirectoryStore } from '../../app/stores/account-directory'
import type { AccountDirectoryResponse } from '../../app/types/responses/account-directory-response'

vi.mock('../../app/services/admin-api', () => ({ searchAccounts: vi.fn() }))

describe('Account directory store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.mocked(searchAccounts).mockReset()
  })

  it('loads account data for its current view state', async () => {
    vi.mocked(searchAccounts).mockResolvedValue({
      items: [
        {
          id: 'account-1',
          username: 'PlayerOne',
          email: 'player@example.com',
          createdAt: '2026-08-10T12:00:00Z',
          lastSuccessfulLoginAt: null,
          hasActiveLoginSession: true
        }
      ],
      total: 1,
      page: 2,
      pageSize: 50
    })
    const store = useAccountDirectoryStore()
    store.query = 'Player'
    store.page = 2
    store.pageSize = 50

    await store.load()

    expect(searchAccounts).toHaveBeenCalledWith({
      query: 'Player',
      page: 2,
      pageSize: 50
    })
    expect(store.items).toHaveLength(1)
    expect(store.total).toBe(1)
    expect(store.loading).toBe(false)
    expect(store.error).toBeUndefined()
  })

  it('uses the directory-specific message for a not-found response', async () => {
    vi.mocked(searchAccounts).mockRejectedValue({ statusCode: 404 })
    const store = useAccountDirectoryStore()

    await store.load()

    expect(store.error).toContain('disabled outside Development')
    expect(store.loading).toBe(false)
  })

  it('ignores a stale response', async () => {
    let resolveFirst: (value: AccountDirectoryResponse) => void = () => {}
    let resolveSecond: (value: AccountDirectoryResponse) => void = () => {}
    vi.mocked(searchAccounts)
      .mockReturnValueOnce(
        new Promise<AccountDirectoryResponse>(resolve => {
          resolveFirst = resolve
        })
      )
      .mockReturnValueOnce(
        new Promise<AccountDirectoryResponse>(resolve => {
          resolveSecond = resolve
        })
      )
    const store = useAccountDirectoryStore()

    const firstLoad = store.load()
    store.query = 'latest'
    const secondLoad = store.load()
    resolveSecond({ items: [], total: 2, page: 1, pageSize: 25 })
    await secondLoad
    resolveFirst({ items: [], total: 1, page: 1, pageSize: 25 })
    await firstLoad

    expect(store.total).toBe(2)
  })
})
