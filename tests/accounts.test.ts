import { describe, expect, it } from 'vitest'
import { accountsUrl, formatAccountDate, pageFromQuery } from '../lib/accounts'

describe('account directory helpers', () => {
  it('builds encoded paginated search URLs', () => {
    expect(
      accountsUrl('https://admin.example.com/operations/', {
        query: ' Player One ',
        page: 2,
        pageSize: 25
      })
    ).toBe(
      'https://admin.example.com/operations/api/accounts?query=Player+One&page=2&pageSize=25'
    )
  })

  it('uses safe page defaults', () => {
    expect(pageFromQuery('3')).toBe(3)
    expect(pageFromQuery('0')).toBe(1)
    expect(pageFromQuery(['2'])).toBe(1)
  })

  it('formats optional account dates', () => {
    expect(formatAccountDate(null)).toBe('—')
    expect(formatAccountDate('2026-08-04T12:00:00Z')).not.toBe('—')
  })
})
