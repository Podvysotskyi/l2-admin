import { createPinia, setActivePinia } from 'pinia'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { getAdminServiceInfo } from '../../app/services/admin-api'
import { useSystemStore } from '../../app/stores/system'
import type { AdminServiceInfo } from '../../app/types/responses/admin-service-info'

vi.mock('../../app/services/admin-api', () => ({ getAdminServiceInfo: vi.fn() }))

describe('System store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.mocked(getAdminServiceInfo).mockReset()
  })

  it('deduplicates concurrent loads and caches service information', async () => {
    let resolveRequest: (value: AdminServiceInfo) => void = () => {}
    vi.mocked(getAdminServiceInfo).mockReturnValue(
      new Promise(resolve => {
        resolveRequest = resolve
      })
    )
    const store = useSystemStore()

    const firstLoad = store.load()
    const secondLoad = store.load()
    resolveRequest({
      service: 'l2-admin-api',
      buildVersion: '1.0.0',
      environment: 'Testing'
    })
    await Promise.all([firstLoad, secondLoad])
    await store.load()

    expect(getAdminServiceInfo).toHaveBeenCalledTimes(1)
    expect(store.serviceState).toBe('connected')
    expect(store.info?.environment).toBe('Testing')
    expect(store.serviceDescription).toBe('Testing · 1.0.0')
  })

  it('reports a failed service request', async () => {
    vi.mocked(getAdminServiceInfo).mockRejectedValue(new Error('Unavailable'))
    const store = useSystemStore()

    await expect(store.load()).rejects.toThrow('Unavailable')

    expect(store.serviceState).toBe('error')
    expect(store.error).toBe(
      'Admin API service information could not be loaded.'
    )
    expect(store.loading).toBe(false)
  })
})
