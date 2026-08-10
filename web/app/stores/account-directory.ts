import { defineStore } from 'pinia'
import { ref } from 'vue'
import { searchAccounts } from '../services/admin-api'
import type { AccountSummary } from '../types/models/account-summary'
import { httpStatusCode } from '../utils/http-error'

export const useAccountDirectoryStore = defineStore(
  'account-directory',
  () => {
    const items = ref<AccountSummary[]>([])
    const total = ref(0)
    const query = ref('')
    const page = ref(1)
    const pageSize = ref(25)
    const loading = ref(true)
    const error = ref<string>()
    let requestVersion = 0

    async function load() {
      const version = ++requestVersion
      loading.value = true
      error.value = undefined

      try {
        const response = await searchAccounts({
          query: query.value,
          page: page.value,
          pageSize: pageSize.value
        })
        if (version !== requestVersion) return
        items.value = response.items
        total.value = response.total
      } catch (cause) {
        if (version !== requestVersion) return
        error.value =
          httpStatusCode(cause) === 404
            ? 'The directory is disabled outside Development until administrator authentication is available.'
            : 'Player accounts could not be loaded from the Admin API.'
      } finally {
        if (version === requestVersion) loading.value = false
      }
    }

    return {
      items,
      total,
      query,
      page,
      pageSize,
      loading,
      error,
      load
    }
  }
)
