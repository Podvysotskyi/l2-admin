<template>
  <UCard>
    <template #header>
      <div class="flex items-center gap-3">
        <UIcon name="i-lucide-users-round" class="size-5 text-primary" />
        <div>
          <h2 class="text-sm font-semibold text-highlighted">
            Player account directory
          </h2>
          <p class="text-xs text-muted">Search identity and session metadata</p>
        </div>
      </div>
    </template>
    <div class="flex min-h-44 flex-col justify-between gap-6">
      <p class="max-w-2xl text-sm leading-6 text-muted">
        Inspect account IDs, registration dates, successful sign-in activity,
        and current login-session state. Credentials, token hashes, and network
        addresses are never exposed.
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
      <div class="flex items-center gap-3">
        <UIcon name="i-lucide-contact-round" class="size-5 text-info" />
        <div>
          <h2 class="text-sm font-semibold text-highlighted">
            Player character directory
          </h2>
          <p class="text-xs text-muted">
            Search ownership and progression metadata
          </p>
        </div>
      </div>
    </template>
    <div class="flex min-h-44 flex-col justify-between gap-6">
      <p class="text-sm leading-6 text-muted">
        Inspect all persisted characters, including pending and expired
        deletions, with resolved race, sex, class, level, and experience details.
      </p>
      <UButton
        to="/characters"
        label="Open character directory"
        trailing-icon="i-lucide-arrow-right"
        class="self-start"
      />
    </div>
  </UCard>
</template>
