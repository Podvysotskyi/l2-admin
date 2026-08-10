<script setup lang="ts">
import type { TableColumn } from '@nuxt/ui'
import type { AccountSummary } from '../../../types/models/account-summary'
import { formatTimestamp } from '../../../utils/format-timestamp'

defineProps<{
  items: AccountSummary[]
  total: number
  loading: boolean
  error?: string
}>()
const emit = defineEmits<{ refresh: [] }>()
const query = defineModel<string>('query', { required: true })
const page = defineModel<number>('page', { required: true })
const pageSize = defineModel<number>('pageSize', { required: true })

const columns: TableColumn<AccountSummary>[] = [
  { accessorKey: 'username', header: 'Player' },
  { accessorKey: 'email', header: 'Login email' },
  { accessorKey: 'id', header: 'Account ID' },
  { accessorKey: 'createdAt', header: 'Registered' },
  { accessorKey: 'lastSuccessfulLoginAt', header: 'Last sign-in' },
  { accessorKey: 'hasActiveLoginSession', header: 'Session' }
]
</script>

<template>
  <AppAdminPageHeader
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
        @click="emit('refresh')"
      />
    </template>
  </AppAdminPageHeader>

  <UAlert
    v-if="error"
    color="error"
    variant="subtle"
    icon="i-lucide-circle-alert"
    title="Account directory unavailable"
    :description="error"
  >
    <template #actions>
      <UButton color="error" variant="soft" size="sm" @click="emit('refresh')">
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
          {{ total.toLocaleString() }} registered
          {{ total === 1 ? 'player' : 'players' }}
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
        :data="items"
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
          <span class="text-sm">{{ formatTimestamp(row.original.createdAt) }}</span>
        </template>
        <template #lastSuccessfulLoginAt-cell="{ row }">
          <span class="text-sm">
            {{ formatTimestamp(row.original.lastSuccessfulLoginAt) }}
          </span>
        </template>
        <template #hasActiveLoginSession-cell="{ row }">
          <UBadge
            :color="row.original.hasActiveLoginSession ? 'success' : 'neutral'"
            variant="subtle"
          >
            {{ row.original.hasActiveLoginSession ? 'Active' : 'Inactive' }}
          </UBadge>
        </template>
      </UTable>
    </div>

    <AppAdminTableFooter
      v-model:page="page"
      v-model:page-size="pageSize"
      :total="total"
    />
  </UCard>
</template>
