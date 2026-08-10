<script setup lang="ts">
import type { NavigationMenuItem } from '@nuxt/ui'
import { storeToRefs } from 'pinia'
import { useSystemStore } from '../../stores/system'

const systemStore = useSystemStore()
const { serviceDescription, serviceState: connectionState } =
  storeToRefs(systemStore)

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
</script>

<template>
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
        :text="`Admin API: ${connectionState}`"
        :disabled="!collapsed"
        :content="{ side: 'right' }"
      >
        <div
          class="flex min-w-0 items-center gap-3 rounded-lg px-2 py-1.5"
          :class="collapsed ? 'justify-center' : ''"
        >
          <span class="relative flex size-2.5 shrink-0">
            <span
              v-if="connectionState === 'connecting'"
              class="absolute inline-flex size-full animate-ping rounded-full bg-warning opacity-50"
            />
            <span
              class="relative inline-flex size-2.5 rounded-full"
              :class="{
                'bg-success': connectionState === 'connected',
                'bg-error': connectionState === 'error',
                'bg-warning': connectionState === 'connecting'
              }"
            />
          </span>
          <span v-if="!collapsed" class="min-w-0">
            <span class="block text-xs font-medium text-highlighted">
              Admin API
            </span>
            <span class="block truncate text-xs text-muted">
              {{ serviceDescription }}
            </span>
          </span>
        </div>
      </UTooltip>
    </template>
  </UDashboardSidebar>
</template>
