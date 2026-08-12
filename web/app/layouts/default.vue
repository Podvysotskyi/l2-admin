<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { useSystemStore } from '../stores/system'
import { useGameVersionStore } from '../stores/game-version'

const route = useRoute()
const systemStore = useSystemStore()
const gameVersionStore = useGameVersionStore()
const { serviceState: connectionState } = storeToRefs(systemStore)

const routeTitle = computed(() =>
  route.path === '/accounts'
    ? 'Player accounts'
    : route.path === '/characters'
      ? 'Player characters'
      : 'Operations dashboard'
)
const statusColor = computed<'success' | 'error' | 'neutral'>(() =>
  connectionState.value === 'connected'
    ? 'success'
    : connectionState.value === 'error'
      ? 'error'
      : 'neutral'
)

onMounted(() => {
  void systemStore.load().catch(() => undefined)
  void gameVersionStore.load().catch(() => undefined)
})
</script>

<template>
  <UDashboardGroup unit="rem" class="min-h-screen">
    <AppAdminSidebar />

    <UDashboardPanel id="admin-panel">
      <template #header>
        <UDashboardNavbar :title="routeTitle" icon="i-lucide-shield-check">
          <template #right>
            <USelect
              :model-value="gameVersionStore.selected"
              :items="gameVersionStore.options"
              :loading="gameVersionStore.loading"
              aria-label="Game version"
              class="w-40"
              @update:model-value="value => gameVersionStore.select(value as string)"
            />
            <UBadge
              :color="statusColor"
              variant="subtle"
              class="hidden sm:flex"
            >
              API {{ connectionState }}
            </UBadge>
            <UColorModeButton color="neutral" variant="ghost" />
          </template>
        </UDashboardNavbar>
      </template>

      <template #body>
        <div class="admin-page">
          <slot />
        </div>
      </template>
    </UDashboardPanel>
  </UDashboardGroup>
</template>
