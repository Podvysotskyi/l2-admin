import { createPinia, setActivePinia } from 'pinia'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import {
  getAdminServiceInfo,
  searchAccounts,
  searchCharacters
} from '../../app/services/admin-api'
import { useAccountDirectoryStore } from '../../app/stores/account-directory'
import { useDashboardStore } from '../../app/stores/dashboard'

vi.mock('../../app/services/admin-api', () => ({
  getAdminServiceInfo: vi.fn(),
  searchAccounts: vi.fn(),
  searchCharacters: vi.fn()
}))

describe('Dashboard store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.mocked(getAdminServiceInfo).mockReset()
    vi.mocked(searchAccounts).mockReset()
    vi.mocked(searchCharacters).mockReset()
  })

  it('loads summary counts without mutating directory state', async () => {
    vi.mocked(getAdminServiceInfo).mockResolvedValue({
      service: 'l2-admin-api',
      buildVersion: '1.0.0',
      environment: 'Testing'
    })
    vi.mocked(searchAccounts).mockResolvedValue({
      items: [],
      total: 12,
      page: 1,
      pageSize: 1
    })
    vi.mocked(searchCharacters).mockResolvedValue({
      items: [],
      total: 34,
      page: 1,
      pageSize: 1
    })
    const accountStore = useAccountDirectoryStore()
    accountStore.query = 'Player'
    accountStore.page = 3
    accountStore.pageSize = 50
    const dashboardStore = useDashboardStore()

    await dashboardStore.load()

    expect(dashboardStore.accountCount).toBe(12)
    expect(dashboardStore.characterCount).toBe(34)
    expect(dashboardStore.totalEntities).toBe(46)
    expect(accountStore.query).toBe('Player')
    expect(accountStore.page).toBe(3)
    expect(accountStore.pageSize).toBe(50)
  })

  it('reports an aggregate loading failure', async () => {
    vi.mocked(getAdminServiceInfo).mockRejectedValue(new Error('Unavailable'))
    vi.mocked(searchAccounts).mockResolvedValue({
      items: [],
      total: 0,
      page: 1,
      pageSize: 1
    })
    vi.mocked(searchCharacters).mockResolvedValue({
      items: [],
      total: 0,
      page: 1,
      pageSize: 1
    })
    const store = useDashboardStore()

    await store.load()

    expect(store.error).toContain('Operational data could not be loaded')
    expect(store.loading).toBe(false)
  })
})
