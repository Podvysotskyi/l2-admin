import { describe, expect, it } from 'vitest'
import {
  accountDirectoryUrl,
  formatTimestamp,
  paginationRange,
  positiveInteger,
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
