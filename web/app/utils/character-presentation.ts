import type { CharacterStatus } from '../types/models/character-status'

export function resolvedName(
  name: string | null | undefined,
  id: string | number
) {
  return name ?? `Unknown (${id})`
}

export function characterStatusLabel(status: CharacterStatus): string {
  switch (status) {
    case 'active':
      return 'Active'
    case 'pending_deletion':
      return 'Pending deletion'
    case 'deletion_expired':
      return 'Deletion expired'
  }
}

export function characterStatusColor(
  status: CharacterStatus
): 'success' | 'warning' | 'error' {
  return status === 'active'
    ? 'success'
    : status === 'pending_deletion'
      ? 'warning'
      : 'error'
}
