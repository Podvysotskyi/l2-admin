import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import {
  getAdminServiceInfo,
  searchAccounts,
  searchCharacters
} from '../../app/services/admin-api'

describe('Admin API service', () => {
  const fetchMock = vi.fn()

  beforeEach(() => vi.stubGlobal('$fetch', fetchMock))
  afterEach(() => {
    fetchMock.mockReset()
    vi.unstubAllGlobals()
  })

  it('loads service information through the Nuxt proxy', async () => {
    fetchMock.mockResolvedValue({
      service: 'l2-admin-api',
      buildVersion: '1.0.0',
      environment: 'Testing'
    })

    await getAdminServiceInfo()

    expect(fetchMock).toHaveBeenCalledWith('/api/system/info')
  })

  it('normalizes account directory query parameters', async () => {
    fetchMock.mockResolvedValue({ items: [], total: 0, page: 2, pageSize: 50 })

    await searchAccounts({
      query: ' Player One ',
      page: 2,
      pageSize: 50
    })

    expect(fetchMock).toHaveBeenCalledWith('/api/accounts', {
      query: { query: 'Player One', page: 2, pageSize: 50 }
    })
  })

  it('uses defaults and omits an empty character query', async () => {
    fetchMock.mockResolvedValue({ items: [], total: 0, page: 1, pageSize: 25 })

    await searchCharacters({ query: '   ' })

    expect(fetchMock).toHaveBeenCalledWith('/api/characters', {
      query: { page: 1, pageSize: 25 }
    })
  })
})
