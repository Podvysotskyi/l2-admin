import { describe, expect, it } from 'vitest'
import { systemInfoUrl } from '../lib/system-info'

describe('systemInfoUrl', () => {
  it('builds the stable system endpoint', () => {
    expect(systemInfoUrl('https://admin.example.com')).toBe(
      'https://admin.example.com/api/system/info'
    )
  })
})
