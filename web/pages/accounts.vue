<script setup lang="ts">
import type { TableColumn } from '@nuxt/ui'
import { onBeforeUnmount, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  accountDirectoryUrl,
  formatTimestamp,
  positiveInteger,
  type AccountPage,
  type AccountRecord
} from '../lib/admin-api'

const config = useRuntimeConfig()
const route = useRoute()
const router = useRouter()
const query = ref(
  typeof route.query.query === 'string' ? route.query.query : ''
)
const page = ref(positiveInteger(route.query.page, 1))
const pageSize = ref(positiveInteger(route.query.pageSize, 25))
const result = ref<AccountPage>()
const loading = ref(true)
const error = ref<string>()
let searchTimer: ReturnType<typeof setTimeout> | undefined
let requestVersion = 0

const columns: TableColumn<AccountRecord>[] = [
  { accessorKey: 'username', header: 'Player' },
  { accessorKey: 'email', header: 'Login email' },
  { accessorKey: 'id', header: 'Account ID' },
  { accessorKey: 'createdAt', header: 'Registered' },
  { accessorKey: 'lastSuccessfulLoginAt', header: 'Last sign-in' },
  { accessorKey: 'hasActiveLoginSession', header: 'Session' }
]

function syncRoute() {
  void router.replace({
    path: '/accounts',
    query: {
      ...(query.value.trim() ? { query: query.value.trim() } : {}),
      ...(page.value > 1 ? { page: String(page.value) } : {}),
      ...(pageSize.value !== 25 ? { pageSize: String(pageSize.value) } : {})
    }
  })
}

async function loadAccounts() {
  const version = ++requestVersion
  loading.value = true
  error.value = undefined
  try {
    const response = await $fetch<AccountPage>(
      accountDirectoryUrl(config.public.apiBase, {
        query: query.value,
        page: page.value,
        pageSize: pageSize.value
      })
    )
    if (version === requestVersion) result.value = response
  } catch (cause) {
    if (version !== requestVersion) return
    const statusCode =
      typeof cause === 'object' && cause !== null && 'statusCode' in cause
        ? Number(cause.statusCode)
        : undefined
    error.value =
      statusCode === 404
        ? 'The directory is disabled outside Development until administrator authentication is available.'
        : 'Player accounts could not be loaded from the Admin API.'
  } finally {
    if (version === requestVersion) loading.value = false
  }
}

watch(query, () => {
  clearTimeout(searchTimer)
  searchTimer = setTimeout(() => {
    if (page.value !== 1) page.value = 1
    else {
      syncRoute()
      void loadAccounts()
    }
  }, 300)
})

watch(page, () => {
  syncRoute()
  void loadAccounts()
})

watch(pageSize, () => {
  if (page.value !== 1) page.value = 1
  else {
    syncRoute()
    void loadAccounts()
  }
})

onMounted(loadAccounts)
onBeforeUnmount(() => clearTimeout(searchTimer))
</script>

<template>
  <div class="space-y-6">
    <AdminPageHeader
      eyebrow="Player management"
      title="Player accounts"
      description="Search registered player identities and review operational sign-in and session metadata."
      icon="i-lucide-users-round"
    >
      <template #actions>
        <UButton
          label="Refresh"
          icon="i-lucide-refresh-cw"
          color="neutral"
          variant="outline"
          :loading="loading"
          @click="loadAccounts"
        />
      </template>
    </AdminPageHeader>

    <UAlert
      v-if="error"
      color="error"
      variant="subtle"
      icon="i-lucide-circle-alert"
      title="Account directory unavailable"
      :description="error"
    >
      <template #actions>
        <UButton color="error" variant="soft" size="sm" @click="loadAccounts">
          Try again
        </UButton>
      </template>
    </UAlert>

    <UCard v-else :ui="{ body: 'p-0 sm:p-0' }">
      <div
        class="flex flex-wrap items-center justify-between gap-4 border-b border-default px-4 py-3"
      >
        <div>
          <p class="text-sm font-medium text-highlighted">Account directory</p>
          <p class="text-xs text-muted">
            {{ result?.total.toLocaleString() ?? 0 }} registered
            {{ result?.total === 1 ? 'player' : 'players' }}
          </p>
        </div>
        <UInput
          v-model="query"
          icon="i-lucide-search"
          placeholder="Search username or email"
          aria-label="Search username or email"
          maxlength="254"
          class="w-full sm:w-80"
        />
      </div>

      <div class="overflow-x-auto">
        <UTable
          :data="result?.items ?? []"
          :columns="columns"
          :loading="loading"
          empty="No player accounts match this search."
          class="min-w-[64rem]"
        >
          <template #username-cell="{ row }">
            <div class="flex items-center gap-3">
              <UAvatar
                :alt="row.original.username"
                :text="row.original.username.slice(0, 2).toUpperCase()"
                size="sm"
              />
              <span class="font-medium text-highlighted">
                {{ row.original.username }}
              </span>
            </div>
          </template>
          <template #id-cell="{ row }">
            <code class="text-xs text-muted" :title="row.original.id">
              {{ row.original.id }}
            </code>
          </template>
          <template #email-cell="{ row }">
            <span class="text-sm text-muted">{{ row.original.email }}</span>
          </template>
          <template #createdAt-cell="{ row }">
            <span class="text-sm">{{
              formatTimestamp(row.original.createdAt)
            }}</span>
          </template>
          <template #lastSuccessfulLoginAt-cell="{ row }">
            <span class="text-sm">
              {{ formatTimestamp(row.original.lastSuccessfulLoginAt) }}
            </span>
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

      <AdminTableFooter
        v-model:page="page"
        v-model:page-size="pageSize"
        :total="result?.total ?? 0"
      />
    </UCard>

    <div class="flex items-start gap-3 rounded-lg border border-default p-4">
      <UIcon
        name="i-lucide-lock-keyhole"
        class="mt-0.5 size-4 shrink-0 text-muted"
      />
      <p class="text-xs leading-5 text-muted">
        This read-only response excludes password hashes, session-token hashes,
        IP addresses, and authoritative player state. Production access remains
        disabled until administrator authentication and permissions are
        complete.
      </p>
    </div>
  </div>
</template>
