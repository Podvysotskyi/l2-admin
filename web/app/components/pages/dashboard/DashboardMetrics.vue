<script setup lang="ts">
import { computed } from 'vue'
import type { AdminServiceInfo } from '../../../types/responses/admin-service-info'

const props = defineProps<{
  accountCount: number
  characterCount: number
  loading: boolean
  systemInfo?: AdminServiceInfo
}>()

const metrics = computed(() => [
  {
    label: 'Player accounts',
    value: props.accountCount.toLocaleString(),
    detail: 'Registered player identities',
    icon: 'i-lucide-users-round',
    color: 'text-primary'
  },
  {
    label: 'Player characters',
    value: props.characterCount.toLocaleString(),
    detail: 'All persisted character rows',
    icon: 'i-lucide-contact-round',
    color: 'text-info'
  },
  {
    label: 'API environment',
    value: props.systemInfo?.environment ?? '—',
    detail: props.systemInfo?.service ?? 'Awaiting Admin API',
    icon: 'i-lucide-server-cog',
    color: 'text-warning'
  },
  {
    label: 'Build version',
    value: props.systemInfo?.buildVersion ?? '—',
    detail: 'Running Admin API release',
    icon: 'i-lucide-git-commit-horizontal',
    color: 'text-success'
  }
])
</script>

<template>
  <section aria-labelledby="operations-summary">
    <div class="mb-3 flex items-center justify-between">
      <h2
        id="operations-summary"
        class="text-sm font-semibold text-highlighted"
      >
        Environment summary
      </h2>
      <UBadge color="neutral" variant="subtle">Read only</UBadge>
    </div>
    <div class="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
      <UCard
        v-for="metric in metrics"
        :key="metric.label"
        :ui="{ body: 'flex min-h-40 flex-col gap-5' }"
      >
        <div class="flex items-start justify-between gap-4">
          <div class="grid size-10 place-items-center rounded-lg bg-elevated">
            <UIcon :name="metric.icon" class="size-5" :class="metric.color" />
          </div>
          <USkeleton v-if="loading" class="h-8 w-24" />
          <strong
            v-else
            class="max-w-48 truncate text-right text-2xl font-semibold tabular-nums"
            :title="metric.value"
          >
            {{ metric.value }}
          </strong>
        </div>
        <div class="mt-auto">
          <p class="text-sm font-medium text-highlighted">{{ metric.label }}</p>
          <p class="mt-1 text-xs text-muted">{{ metric.detail }}</p>
        </div>
      </UCard>
    </div>
  </section>
</template>
