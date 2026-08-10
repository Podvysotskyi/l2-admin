<template>
  <div class="flex items-start gap-3 rounded-lg border border-default p-4">
    <UIcon
      name="i-lucide-lock-keyhole"
      class="mt-0.5 size-4 shrink-0 text-muted"
    />
    <p class="text-xs leading-5 text-muted">
      This read-only response excludes password hashes, session-token hashes,
      IP addresses, and authoritative player state. Production access remains
      disabled until administrator authentication and permissions are complete.
    </p>
  </div>
</template>
