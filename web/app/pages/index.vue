<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { useDashboardStore } from '../stores/dashboard'
import { useSystemStore } from '../stores/system'

const dashboardStore = useDashboardStore()
const systemStore = useSystemStore()
const { accountCount, characterCount, loading, error } =
  storeToRefs(dashboardStore)
const { info: systemInfo } = storeToRefs(systemStore)

onMounted(() => void dashboardStore.load())
</script>

<template>
  <div class="space-y-8">
    <AppAdminPageHeader
      eyebrow="Live operations"
      title="Operations dashboard"
      description="Monitor the administrative surface and inspect player identity and character data exposed by the development Admin API."
      icon="i-lucide-gauge"
    >
      <template #actions>
        <UButton
          label="Refresh data"
          icon="i-lucide-refresh-cw"
          color="neutral"
          variant="outline"
          :loading="loading"
          @click="dashboardStore.load"
        />
      </template>
    </AppAdminPageHeader>

    <UAlert
      color="warning"
      variant="subtle"
      icon="i-lucide-shield-alert"
      title="Development operations surface"
      description="Production access remains disabled until administrator identity, MFA, permissions, and audit controls are implemented."
    />

    <UAlert
      v-if="error"
      color="error"
      variant="subtle"
      icon="i-lucide-server-off"
      title="Admin data unavailable"
      :description="error"
    >
      <template #actions>
        <UButton
          color="error"
          variant="soft"
          size="sm"
          @click="dashboardStore.load"
        >
          Try again
        </UButton>
      </template>
    </UAlert>

    <PagesDashboardDashboardMetrics
      :account-count="accountCount"
      :character-count="characterCount"
      :loading="loading"
      :system-info="systemInfo"
    />

    <div class="grid gap-4 lg:grid-cols-3">
      <PagesDashboardDirectoryNavigation />
      <PagesDashboardControlReadiness />
    </div>
  </div>
</template>
