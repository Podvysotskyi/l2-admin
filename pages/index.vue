<script setup lang="ts">
import { computed } from 'vue'
import {
  accountDirectoryUrl,
  systemInfoUrl,
  type AccountPage,
  type SystemInfo
} from '../lib/admin-api'

const config = useRuntimeConfig()
const loading = ref(true)
const error = ref<string>()
const accountCount = ref(0)
const systemInfo = ref<SystemInfo>()

const metrics = computed(() => [
  {
    label: 'Player accounts',
    value: accountCount.value.toLocaleString(),
    detail: 'Registered player identities',
    icon: 'i-lucide-users-round',
    color: 'text-primary'
  },
  {
    label: 'API environment',
    value: systemInfo.value?.environment ?? '—',
    detail: systemInfo.value?.service ?? 'Awaiting Admin API',
    icon: 'i-lucide-server-cog',
    color: 'text-info'
  },
  {
    label: 'Build version',
    value: systemInfo.value?.buildVersion ?? '—',
    detail: 'Running Admin API release',
    icon: 'i-lucide-git-commit-horizontal',
    color: 'text-success'
  }
])

async function loadDashboard() {
  loading.value = true
  error.value = undefined
  try {
    const [info, accounts] = await Promise.all([
      $fetch<SystemInfo>(systemInfoUrl(config.public.apiBase)),
      $fetch<AccountPage>(
        accountDirectoryUrl(config.public.apiBase, { page: 1, pageSize: 1 })
      )
    ])
    systemInfo.value = info
    accountCount.value = accounts.total
  } catch {
    error.value =
      'Operational data could not be loaded. The account directory is available only in Development until administrator authentication is implemented.'
  } finally {
    loading.value = false
  }
}

onMounted(loadDashboard)
</script>

<template>
  <div class="space-y-8">
    <AdminPageHeader
      eyebrow="Live operations"
      title="Operations dashboard"
      description="Monitor the administrative surface and inspect player identity data exposed by the development Admin API."
      icon="i-lucide-gauge"
    >
      <template #actions>
        <UButton
          label="Refresh data"
          icon="i-lucide-refresh-cw"
          color="neutral"
          variant="outline"
          :loading="loading"
          @click="loadDashboard"
        />
      </template>
    </AdminPageHeader>

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
        <UButton color="error" variant="soft" size="sm" @click="loadDashboard">
          Try again
        </UButton>
      </template>
    </UAlert>

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
      <div class="grid gap-4 md:grid-cols-3">
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
            <p class="text-sm font-medium text-highlighted">
              {{ metric.label }}
            </p>
            <p class="mt-1 text-xs text-muted">{{ metric.detail }}</p>
          </div>
        </UCard>
      </div>
    </section>

    <div class="grid gap-4 lg:grid-cols-[minmax(0,2fr)_minmax(18rem,1fr)]">
      <UCard>
        <template #header>
          <div class="flex items-center gap-3">
            <UIcon name="i-lucide-users-round" class="size-5 text-primary" />
            <div>
              <h2 class="text-sm font-semibold text-highlighted">
                Player account directory
              </h2>
              <p class="text-xs text-muted">
                Search identity and session metadata
              </p>
            </div>
          </div>
        </template>
        <div class="flex min-h-44 flex-col justify-between gap-6">
          <p class="max-w-2xl text-sm leading-6 text-muted">
            Inspect account IDs, registration dates, successful sign-in
            activity, and current login-session state. Credentials, token
            hashes, and network addresses are never exposed.
          </p>
          <UButton
            to="/accounts"
            label="Open account directory"
            trailing-icon="i-lucide-arrow-right"
            class="self-start"
          />
        </div>
      </UCard>

      <UCard>
        <template #header>
          <h2 class="text-sm font-semibold text-highlighted">
            Control readiness
          </h2>
        </template>
        <dl class="space-y-4 text-sm">
          <div class="flex items-center justify-between gap-4">
            <dt class="text-muted">Administrator identity</dt>
            <dd><UBadge color="warning" variant="subtle">Planned</UBadge></dd>
          </div>
          <div class="flex items-center justify-between gap-4">
            <dt class="text-muted">MFA</dt>
            <dd><UBadge color="warning" variant="subtle">Planned</UBadge></dd>
          </div>
          <div class="flex items-center justify-between gap-4">
            <dt class="text-muted">Permissions</dt>
            <dd><UBadge color="warning" variant="subtle">Planned</UBadge></dd>
          </div>
          <div class="flex items-center justify-between gap-4">
            <dt class="text-muted">Player credentials</dt>
            <dd><UBadge color="success" variant="subtle">Protected</UBadge></dd>
          </div>
        </dl>
      </UCard>
    </div>
  </div>
</template>
