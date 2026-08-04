<script setup lang="ts">
const metrics = [
  { label: 'Connected players', value: '—', detail: 'Awaiting world service' },
  {
    label: 'Server health',
    value: 'Foundation',
    detail: 'Diagnostic APIs online'
  },
  { label: 'Incidents', value: '0', detail: 'No active incidents' }
]
</script>

<template>
  <section class="page-content">
    <div>
      <p class="eyebrow">Overview</p>
      <h1>Dashboard</h1>
    </div>
    <div class="mt-12 grid gap-4 md:grid-cols-3">
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
    </div>
  </section>
</template>
