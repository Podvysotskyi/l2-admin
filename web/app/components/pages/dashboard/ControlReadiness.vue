<template>
  <UCard>
    <template #header>
      <h2 class="text-sm font-semibold text-highlighted">Control readiness</h2>
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
</template>
