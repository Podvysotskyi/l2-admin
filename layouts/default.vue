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
    <header class="admin-header">
      <div>
        <p class="eyebrow">Live Operations</p>
        <nav class="admin-nav" aria-label="Admin navigation">
          <NuxtLink to="/">Dashboard</NuxtLink>
          <NuxtLink to="/accounts">Accounts</NuxtLink>
        </nav>
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
    <slot />
  </main>
</template>
