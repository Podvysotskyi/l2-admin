<script setup lang="ts">
import type { TableColumn } from '@nuxt/ui'
import type { CharacterSummary } from '../../../types/models/character-summary'
import {
  characterStatusColor,
  characterStatusLabel,
  resolvedName
} from '../../../utils/character-presentation'
import { formatTimestamp } from '../../../utils/format-timestamp'

defineProps<{
  items: CharacterSummary[]
  total: number
  loading: boolean
  error?: string
}>()
const emit = defineEmits<{ refresh: [] }>()
const query = defineModel<string>('query', { required: true })
const page = defineModel<number>('page', { required: true })
const pageSize = defineModel<number>('pageSize', { required: true })

const columns: TableColumn<CharacterSummary>[] = [
  { accessorKey: 'name', header: 'Character' },
  { accessorKey: 'username', header: 'Owner' },
  { accessorKey: 'raceId', header: 'Race / sex' },
  { accessorKey: 'baseClassId', header: 'Base class' },
  { accessorKey: 'activeClassId', header: 'Active class' },
  { accessorKey: 'level', header: 'Level' },
  { accessorKey: 'experience', header: 'XP' },
  { accessorKey: 'createdAt', header: 'Created' },
  { accessorKey: 'status', header: 'Deletion status' }
]
</script>

<template>
  <AppAdminPageHeader
    eyebrow="Player management"
    title="Player characters"
    description="Search every persisted character and review ownership, progression, class, and deletion metadata."
    icon="i-lucide-contact-round"
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
    title="Character directory unavailable"
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
        <p class="text-sm font-medium text-highlighted">Character directory</p>
        <p class="text-xs text-muted">
          {{ total.toLocaleString() }} persisted
          {{ total === 1 ? 'character' : 'characters' }}
        </p>
      </div>
      <UInput
        v-model="query"
        icon="i-lucide-search"
        placeholder="Search character or owner"
        aria-label="Search character or owner"
        maxlength="254"
        class="w-full sm:w-80"
      />
    </div>

    <div class="overflow-x-auto">
      <UTable
        :data="items"
        :columns="columns"
        :loading="loading"
        empty="No characters match this search."
        class="min-w-[96rem]"
      >
        <template #name-cell="{ row }">
          <div class="flex items-center gap-3">
            <UAvatar
              :alt="row.original.name"
              :text="row.original.name.slice(0, 2).toUpperCase()"
              size="sm"
            />
            <div class="min-w-0">
              <p class="font-medium text-highlighted">{{ row.original.name }}</p>
              <code class="block text-xs text-dimmed" :title="row.original.id">
                {{ row.original.id }}
              </code>
            </div>
          </div>
        </template>
        <template #username-cell="{ row }">
          <div>
            <p class="text-sm text-highlighted">
              {{ resolvedName(row.original.username, row.original.accountId) }}
            </p>
            <code
              class="block text-xs text-dimmed"
              :title="row.original.accountId"
            >
              {{ row.original.accountId }}
            </code>
          </div>
        </template>
        <template #raceId-cell="{ row }">
          <div class="text-sm">
            <p>{{ resolvedName(row.original.raceName, row.original.raceId) }}</p>
            <p class="text-xs text-muted">
              {{ resolvedName(row.original.sexName, row.original.sexId) }}
            </p>
          </div>
        </template>
        <template #baseClassId-cell="{ row }">
          <span class="text-sm">
            {{
              resolvedName(row.original.baseClassName, row.original.baseClassId)
            }}
          </span>
        </template>
        <template #activeClassId-cell="{ row }">
          <span class="text-sm font-medium text-highlighted">
            {{
              resolvedName(
                row.original.activeClassName,
                row.original.activeClassId
              )
            }}
          </span>
        </template>
        <template #level-cell="{ row }">
          <span class="font-medium tabular-nums">{{ row.original.level }}</span>
        </template>
        <template #experience-cell="{ row }">
          <span class="text-sm tabular-nums">
            {{ row.original.experience.toLocaleString() }}
          </span>
        </template>
        <template #createdAt-cell="{ row }">
          <span class="text-sm">{{ formatTimestamp(row.original.createdAt) }}</span>
        </template>
        <template #status-cell="{ row }">
          <div class="space-y-1">
            <UBadge
              :color="characterStatusColor(row.original.status)"
              variant="subtle"
            >
              {{ characterStatusLabel(row.original.status) }}
            </UBadge>
            <p v-if="row.original.deleteAfter" class="text-xs text-muted">
              {{ formatTimestamp(row.original.deleteAfter) }}
            </p>
          </div>
        </template>
      </UTable>
    </div>

    <AppAdminTableFooter
      v-model:page="page"
      v-model:page-size="pageSize"
      :total="total"
      empty-label="No characters"
    />
  </UCard>
</template>
