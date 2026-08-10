import { describe, expect, it } from 'vitest'
import {
  characterStatusColor,
  characterStatusLabel,
  resolvedName
} from '../../app/utils/character-presentation'
import { formatTimestamp } from '../../app/utils/format-timestamp'

describe('Presentation utilities', () => {
  it('formats character lookup fallbacks and deletion statuses', () => {
    expect(resolvedName('Human', 0)).toBe('Human')
    expect(resolvedName(null, 777)).toBe('Unknown (777)')
    expect(characterStatusLabel('active')).toBe('Active')
    expect(characterStatusLabel('pending_deletion')).toBe('Pending deletion')
    expect(characterStatusLabel('deletion_expired')).toBe('Deletion expired')
    expect(characterStatusColor('active')).toBe('success')
    expect(characterStatusColor('pending_deletion')).toBe('warning')
    expect(characterStatusColor('deletion_expired')).toBe('error')
  })

  it('formats optional and invalid timestamps safely', () => {
    expect(formatTimestamp(null)).toBe('Never')
    expect(formatTimestamp('not-a-date')).toBe('Invalid date')
    expect(formatTimestamp('2026-08-04T12:00:00Z')).not.toBe('Never')
  })
})
