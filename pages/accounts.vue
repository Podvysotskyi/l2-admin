<script setup lang="ts">
import type { TableColumn } from '@nuxt/ui'
import { onBeforeUnmount, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  accountsUrl,
  formatAccountDate,
  pageFromQuery,
  type AccountDirectoryPage,
  type AccountSummary
} from '../lib/accounts'

const pageSize = 25
const config = useRuntimeConfig()
const route = useRoute()
const router = useRouter()
const search = ref(
  typeof route.query.query === 'string' ? route.query.query : ''
)
const page = ref(pageFromQuery(route.query.page))
const result = ref<AccountDirectoryPage>()
const loading = ref(false)
const error = ref<string>()
let searchTimer: ReturnType<typeof setTimeout> | undefined
let requestSequence = 0

const columns: TableColumn<AccountSummary>[] = [
  { accessorKey: 'username', header: 'Username' },
  { accessorKey: 'id', header: 'Account ID' },
  { accessorKey: 'createdAt', header: 'Created' },
  { accessorKey: 'lastSuccessfulLoginAt', header: 'Last sign-in' },
  { accessorKey: 'hasActiveLoginSession', header: 'Login session' }
]

async function loadAccounts() {
  const sequence = ++requestSequence
  loading.value = true
  error.value = undefined
  try {
    const response = await $fetch<AccountDirectoryPage>(
      accountsUrl(config.public.apiBase, {
        query: search.value,
        page: page.value,
        pageSize
      })
    )
    if (sequence === requestSequence) result.value = response
  } catch (cause) {
    if (sequence !== requestSequence) return
    const statusCode =
      typeof cause === 'object' && cause !== null && 'statusCode' in cause
        ? Number(cause.statusCode)
        : undefined
    error.value =
      statusCode === 404
        ? 'Account directory is unavailable until administrator authentication is configured.'
        : 'Could not load player accounts.'
  } finally {
    if (sequence === requestSequence) loading.value = false
  }
}

function syncRoute() {
  void router.replace({
    path: '/accounts',
    query: {
      ...(search.value.trim() ? { query: search.value.trim() } : {}),
      ...(page.value > 1 ? { page: String(page.value) } : {})
    }
  })
}

watch(search, () => {
  clearTimeout(searchTimer)
  searchTimer = setTimeout(() => {
    if (page.value === 1) {
      syncRoute()
      void loadAccounts()
    } else {
      page.value = 1
    }
  }, 300)
})

watch(page, () => {
  syncRoute()
  void loadAccounts()
})

onMounted(loadAccounts)
onBeforeUnmount(() => clearTimeout(searchTimer))
</script>

<template>
  <section class="page-content">
    <div class="flex flex-wrap items-end justify-between gap-6">
      <div>
        <p class="eyebrow">Player management</p>
        <h1>Accounts</h1>
        <p class="mt-3 text-sm text-muted">
          {{ result?.total ?? 0 }} player
          {{ result?.total === 1 ? 'account' : 'accounts' }}
        </p>
      </div>
      <UInput
        v-model="search"
        aria-label="Search username"
        placeholder="Search username"
        icon="i-lucide-search"
        maxlength="24"
        class="w-full sm:w-80"
      />
    </div>

    <UAlert
      v-if="error"
      color="error"
      variant="subtle"
      title="Account directory unavailable"
      :description="error"
      class="mt-8"
    />

    <UCard v-else class="mt-8 overflow-hidden" :ui="{ body: 'p-0' }">
      <div class="overflow-x-auto">
        <UTable
          :data="result?.items ?? []"
          :columns="columns"
          :loading="loading"
          empty="No accounts match this search."
          class="min-w-[960px]"
        >
          <template #id-cell="{ row }">
            <code class="text-xs text-muted">{{ row.original.id }}</code>
          </template>
          <template #createdAt-cell="{ row }">
            {{ formatAccountDate(row.original.createdAt) }}
          </template>
          <template #lastSuccessfulLoginAt-cell="{ row }">
            {{ formatAccountDate(row.original.lastSuccessfulLoginAt) }}
          </template>
          <template #hasActiveLoginSession-cell="{ row }">
            <UBadge
              :color="
                row.original.hasActiveLoginSession ? 'success' : 'neutral'
              "
              variant="subtle"
            >
              {{ row.original.hasActiveLoginSession ? 'Active' : 'Inactive' }}
            </UBadge>
          </template>
        </UTable>
      </div>
      <div
        v-if="result && result.total > pageSize"
        class="flex justify-end border-t border-default p-4"
      >
        <UPagination
          v-model:page="page"
          :total="result.total"
          :items-per-page="pageSize"
        />
      </div>
    </UCard>

    <p class="mt-5 text-xs text-muted">
      Development-only directory · production access requires administrator
      authentication and permissions.
    </p>
  </section>
</template>
