import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { searchAccounts, searchCharacters } from '../services/admin-api'
import { useSystemStore } from './system'

export const useDashboardStore = defineStore('dashboard', () => {
  const accountCount = ref(0)
  const characterCount = ref(0)
  const loading = ref(true)
  const error = ref<string>()
  let requestVersion = 0

  const totalEntities = computed(
    () => accountCount.value + characterCount.value
  )

  async function load() {
    const version = ++requestVersion
    loading.value = true
    error.value = undefined

    try {
      const systemStore = useSystemStore()
      const [, accounts, characters] = await Promise.all([
        systemStore.load(),
        searchAccounts({ page: 1, pageSize: 1 }),
        searchCharacters({ page: 1, pageSize: 1 })
      ])
      if (version !== requestVersion) return
      accountCount.value = accounts.total
      characterCount.value = characters.total
    } catch {
      if (version !== requestVersion) return
      error.value =
        'Operational data could not be loaded. Player directories are available only in Development until administrator authentication is implemented.'
    } finally {
      if (version === requestVersion) loading.value = false
    }
  }

  return {
    accountCount,
    characterCount,
    loading,
    error,
    totalEntities,
    load
  }
})
