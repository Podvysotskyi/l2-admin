<script setup lang="ts">
import type { TableColumn } from '@nuxt/ui'
import { onBeforeUnmount, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  characterDirectoryUrl,
  characterStatusColor,
  characterStatusLabel,
  directoryRouteState,
  formatTimestamp,
  resolvedName,
  type CharacterPage,
  type CharacterRecord
} from '../lib/admin-api'

const config = useRuntimeConfig()
const route = useRoute()
const router = useRouter()
const initialRoute = directoryRouteState(route.query)
const query = ref(initialRoute.query)
const page = ref(initialRoute.page)
const pageSize = ref(initialRoute.pageSize)
const result = ref<CharacterPage>()
const loading = ref(true)
const error = ref<string>()
let searchTimer: ReturnType<typeof setTimeout> | undefined
let requestVersion = 0

const columns: TableColumn<CharacterRecord>[] = [
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

function syncRoute() {
  void router.replace({
    path: '/characters',
    query: {
      ...(query.value.trim() ? { query: query.value.trim() } : {}),
      ...(page.value > 1 ? { page: String(page.value) } : {}),
      ...(pageSize.value !== 25 ? { pageSize: String(pageSize.value) } : {})
    }
  })
}

async function loadCharacters() {
  const version = ++requestVersion
  loading.value = true
  error.value = undefined
  try {
    const response = await $fetch<CharacterPage>(
      characterDirectoryUrl(config.public.apiBase, {
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
        : 'Player characters could not be loaded from the Admin API.'
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
      void loadCharacters()
    }
  }, 300)
})

watch(page, () => {
  syncRoute()
  void loadCharacters()
})

watch(pageSize, () => {
  if (page.value !== 1) page.value = 1
  else {
    syncRoute()
    void loadCharacters()
  }
})

onMounted(loadCharacters)
onBeforeUnmount(() => clearTimeout(searchTimer))
</script>

<template>
  <div class="space-y-6">
    <AdminPageHeader
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
          @click="loadCharacters"
        />
      </template>
    </AdminPageHeader>

    <UAlert
      v-if="error"
      color="error"
      variant="subtle"
      icon="i-lucide-circle-alert"
      title="Character directory unavailable"
      :description="error"
    >
      <template #actions>
        <UButton color="error" variant="soft" size="sm" @click="loadCharacters">
          Try again
        </UButton>
      </template>
    </UAlert>

    <UCard v-else :ui="{ body: 'p-0 sm:p-0' }">
      <div
        class="flex flex-wrap items-center justify-between gap-4 border-b border-default px-4 py-3"
      >
        <div>
          <p class="text-sm font-medium text-highlighted">
            Character directory
          </p>
          <p class="text-xs text-muted">
            {{ result?.total.toLocaleString() ?? 0 }} persisted
            {{ result?.total === 1 ? 'character' : 'characters' }}
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
          :data="result?.items ?? []"
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
                <p class="font-medium text-highlighted">
                  {{ row.original.name }}
                </p>
                <code
                  class="block text-xs text-dimmed"
                  :title="row.original.id"
                >
                  {{ row.original.id }}
                </code>
              </div>
            </div>
          </template>
          <template #username-cell="{ row }">
            <div>
              <p class="text-sm text-highlighted">
                {{
                  resolvedName(row.original.username, row.original.accountId)
                }}
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
              <p>
                {{ resolvedName(row.original.raceName, row.original.raceId) }}
              </p>
              <p class="text-xs text-muted">
                {{ resolvedName(row.original.sexName, row.original.sexId) }}
              </p>
            </div>
          </template>
          <template #baseClassId-cell="{ row }">
            <span class="text-sm">
              {{
                resolvedName(
                  row.original.baseClassName,
                  row.original.baseClassId
                )
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
            <span class="font-medium tabular-nums">{{
              row.original.level
            }}</span>
          </template>
          <template #experience-cell="{ row }">
            <span class="text-sm tabular-nums">
              {{ row.original.experience.toLocaleString() }}
            </span>
          </template>
          <template #createdAt-cell="{ row }">
            <span class="text-sm">{{
              formatTimestamp(row.original.createdAt)
            }}</span>
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

      <AdminTableFooter
        v-model:page="page"
        v-model:page-size="pageSize"
        :total="result?.total ?? 0"
        empty-label="No characters"
      />
    </UCard>

    <div class="flex items-start gap-3 rounded-lg border border-default p-4">
      <UIcon
        name="i-lucide-lock-keyhole"
        class="mt-0.5 size-4 shrink-0 text-muted"
      />
      <p class="text-xs leading-5 text-muted">
        This directory is strictly read-only. Character selection, restoration,
        deletion, editing, and other authoritative gameplay operations remain
        owned by the Game Server.
      </p>
    </div>
  </div>
</template>
