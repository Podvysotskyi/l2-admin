import { describe, expect, it } from 'vitest'
import {
  accountDirectoryUrl,
  characterDirectoryUrl,
  characterStatusColor,
  characterStatusLabel,
  directoryRouteState,
  formatTimestamp,
  paginationRange,
  positiveInteger,
  resolvedName,
  systemInfoUrl
} from '../lib/admin-api'

describe('Admin API client', () => {
  it('builds a normalized account directory URL', () => {
    expect(
      accountDirectoryUrl('https://admin.example.com/operations/', {
        query: ' Player One ',
        page: 2,
        pageSize: 50
      })
    ).toBe(
      'https://admin.example.com/operations/api/accounts?query=Player+One&page=2&pageSize=50'
    )
  })

  it('builds the system information URL', () => {
    expect(systemInfoUrl('http://localhost:5201/')).toBe(
      'http://localhost:5201/api/system/info'
    )
  })

  it('builds a normalized character directory URL', () => {
    expect(
      characterDirectoryUrl('https://admin.example.com/operations/', {
        query: ' Hero One ',
        page: 3,
        pageSize: 10
      })
    ).toBe(
      'https://admin.example.com/operations/api/characters?query=Hero+One&page=3&pageSize=10'
    )
  })

  it('reads bounded character directory route state', () => {
    expect(
      directoryRouteState({ query: 'Hero', page: '2', pageSize: '50' })
    ).toEqual({ query: 'Hero', page: 2, pageSize: 50 })
    expect(directoryRouteState({ query: ['Hero'], page: '0' })).toEqual({
      query: '',
      page: 1,
      pageSize: 25
    })
  })

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

  it('accepts only positive integer query values', () => {
    expect(positiveInteger('4', 1)).toBe(4)
    expect(positiveInteger('0', 25)).toBe(25)
    expect(positiveInteger(['2'], 25)).toBe(25)
  })

  it('formats optional and invalid timestamps safely', () => {
    expect(formatTimestamp(null)).toBe('Never')
    expect(formatTimestamp('not-a-date')).toBe('Invalid date')
    expect(formatTimestamp('2026-08-04T12:00:00Z')).not.toBe('Never')
  })

  it('reports bounded pagination ranges', () => {
    expect(paginationRange(51, 2, 25)).toEqual({ first: 26, last: 50 })
    expect(paginationRange(2, 10, 25)).toEqual({ first: 2, last: 2 })
    expect(paginationRange(0, 1, 25)).toEqual({ first: 0, last: 0 })
  })
})
