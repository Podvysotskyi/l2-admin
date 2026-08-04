<script setup lang="ts">
import { serviceLabels, type ServiceState } from '@l2/ui'
import { computed } from 'vue'
import { systemInfoUrl, type SystemInfo } from '../lib/system-info'

const config = useRuntimeConfig()
const state = ref<ServiceState>('connecting')
const info = ref<SystemInfo>()
const error = ref<string>()
const statusColor = computed<'success' | 'error' | 'neutral'>(() =>
  state.value === 'connected'
    ? 'success'
    : state.value === 'error'
      ? 'error'
      : 'neutral'
)

const metrics = [
  { label: 'Connected players', value: '—', detail: 'Awaiting world service' },
  {
    label: 'Server health',
    value: 'Foundation',
    detail: 'Diagnostic APIs online'
  },
  { label: 'Incidents', value: '0', detail: 'No active incidents' }
]

onMounted(async () => {
  try {
    info.value = await $fetch<SystemInfo>(systemInfoUrl(config.public.apiBase))
    state.value = 'connected'
  } catch {
    state.value = 'error'
    error.value = 'Could not connect to the Admin API.'
  }
})
</script>

<template>
  <main class="admin-shell">
    <header>
      <div>
        <p class="eyebrow">Live Operations</p>
        <h1>Admin</h1>
      </div>
      <UCard
        variant="subtle"
        class="min-w-72"
        :ui="{ body: 'flex items-center gap-3 p-4' }"
      >
        <UBadge :color="statusColor" variant="subtle">
          Admin API: {{ serviceLabels[state] }}
        </UBadge>
        <div class="grid gap-1 text-xs text-muted">
          <span v-if="info"
            >{{ info.service }} · {{ info.buildVersion }} ·
            {{ info.environment }}</span
          >
          <span v-else>{{ error ?? config.public.apiBase }}</span>
        </div>
      </UCard>
    </header>
    <section class="mt-20 grid gap-4 md:grid-cols-3">
      <UCard
        v-for="metric in metrics"
        :key="metric.label"
        variant="subtle"
        :ui="{ body: 'flex min-h-56 flex-col p-6' }"
      >
        <span class="text-sm text-muted">{{ metric.label }}</span>
        <strong class="my-auto text-4xl font-light">{{ metric.value }}</strong>
        <span class="text-sm text-muted">{{ metric.detail }}</span>
      </UCard>
    </section>
  </main>
</template>
