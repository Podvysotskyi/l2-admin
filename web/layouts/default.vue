<script setup lang="ts">
import type { NavigationMenuItem } from '@nuxt/ui'
import type { ServiceState } from '@podvysotskyi/l2-ui'
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { systemInfoUrl, type SystemInfo } from '../lib/admin-api'

const route = useRoute()
const config = useRuntimeConfig()
const serviceState = ref<ServiceState>('connecting')
const systemInfo = ref<SystemInfo>()

const navigation: NavigationMenuItem[] = [
  { label: 'Operations', type: 'label' },
  {
    label: 'Dashboard',
    icon: 'i-lucide-gauge',
    to: '/',
    exact: true
  },
  { label: 'Players', type: 'label' },
  {
    label: 'Accounts',
    icon: 'i-lucide-users-round',
    to: '/accounts'
  },
  {
    label: 'Characters',
    icon: 'i-lucide-contact-round',
    to: '/characters'
  }
]

const routeTitle = computed(() =>
  route.path === '/accounts'
    ? 'Player accounts'
    : route.path === '/characters'
      ? 'Player characters'
      : 'Operations dashboard'
)
const statusColor = computed<'success' | 'error' | 'neutral'>(() =>
  serviceState.value === 'connected'
    ? 'success'
    : serviceState.value === 'error'
      ? 'error'
      : 'neutral'
)

onMounted(async () => {
  try {
    systemInfo.value = await $fetch<SystemInfo>(
      systemInfoUrl(config.public.apiBase)
    )
    serviceState.value = 'connected'
  } catch {
    serviceState.value = 'error'
  }
})
</script>

<template>
  <UDashboardGroup unit="rem" class="min-h-screen">
    <UDashboardSidebar
      id="admin-sidebar"
      collapsible
      resizable
      :default-size="17"
      :min-size="15"
      :max-size="22"
      :collapsed-size="4"
      :ui="{ footer: 'border-t border-default' }"
    >
      <template #header="{ collapsed }">
        <NuxtLink to="/" class="flex min-w-0 items-center gap-3">
          <span
            class="grid size-9 shrink-0 place-items-center rounded-lg bg-primary text-sm font-black text-inverted shadow-sm shadow-primary/30"
          >
            L2
          </span>
          <span v-if="!collapsed" class="min-w-0">
            <strong class="block truncate text-sm text-highlighted">
              Admin
            </strong>
            <small class="block truncate text-xs text-muted">
              Live operations
            </small>
          </span>
        </NuxtLink>
      </template>

      <template #default="{ collapsed }">
        <UNavigationMenu
          :items="navigation"
          orientation="vertical"
          :collapsed="collapsed"
          :tooltip="collapsed"
          :popover="collapsed"
          highlight
          class="w-full"
        />
      </template>

      <template #footer="{ collapsed }">
        <UTooltip
          :text="`Admin API: ${serviceState}`"
          :disabled="!collapsed"
          :content="{ side: 'right' }"
        >
          <div
            class="flex min-w-0 items-center gap-3 rounded-lg px-2 py-1.5"
            :class="collapsed ? 'justify-center' : ''"
          >
            <span class="relative flex size-2.5 shrink-0">
              <span
                v-if="serviceState === 'connecting'"
                class="absolute inline-flex size-full animate-ping rounded-full bg-warning opacity-50"
              />
              <span
                class="relative inline-flex size-2.5 rounded-full"
                :class="{
                  'bg-success': serviceState === 'connected',
                  'bg-error': serviceState === 'error',
                  'bg-warning': serviceState === 'connecting'
                }"
              />
            </span>
            <span v-if="!collapsed" class="min-w-0">
              <span class="block text-xs font-medium text-highlighted">
                Admin API
              </span>
              <span class="block truncate text-xs text-muted">
                {{
                  systemInfo
                    ? `${systemInfo.environment} · ${systemInfo.buildVersion}`
                    : config.public.apiBase
                }}
              </span>
            </span>
          </div>
        </UTooltip>
      </template>
    </UDashboardSidebar>

    <UDashboardPanel id="admin-panel">
      <template #header>
        <UDashboardNavbar :title="routeTitle" icon="i-lucide-shield-check">
          <template #right>
            <UBadge
              :color="statusColor"
              variant="subtle"
              class="hidden sm:flex"
            >
              API {{ serviceState }}
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
