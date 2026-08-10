import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { getAdminServiceInfo } from '../services/admin-api'
import type { ServiceState } from '../types/models/service-state'
import type { AdminServiceInfo } from '../types/responses/admin-service-info'

export const useSystemStore = defineStore('system', () => {
  const info = ref<AdminServiceInfo>()
  const serviceState = ref<ServiceState>('connecting')
  const loading = ref(false)
  const error = ref<string>()
  let pendingRequest: Promise<AdminServiceInfo> | undefined
  let requestVersion = 0

  const serviceDescription = computed(() =>
    info.value
      ? `${info.value.environment} · ${info.value.buildVersion}`
      : '/api via Nuxt'
  )

  async function load(force = false): Promise<AdminServiceInfo> {
    if (!force && info.value) return info.value
    if (!force && pendingRequest) return pendingRequest

    const version = ++requestVersion
    loading.value = true
    serviceState.value = 'connecting'
    error.value = undefined
    const request = getAdminServiceInfo()
    pendingRequest = request

    try {
      const response = await request
      if (version === requestVersion) {
        info.value = response
        serviceState.value = 'connected'
      }
      return response
    } catch (cause) {
      if (version === requestVersion) {
        serviceState.value = 'error'
        error.value = 'Admin API service information could not be loaded.'
      }
      throw cause
    } finally {
      if (pendingRequest === request) pendingRequest = undefined
      if (version === requestVersion) loading.value = false
    }
  }

  return {
    info,
    serviceState,
    loading,
    error,
    serviceDescription,
    load
  }
})
