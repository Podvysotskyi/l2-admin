import { describe, expect, it } from 'vitest'
import {
  directoryRouteQuery,
  directoryRouteState,
  paginationRange,
  positiveInteger
} from '../../app/utils/directory'

describe('Directory utilities', () => {
  it('reads bounded route state', () => {
    expect(
      directoryRouteState({ query: 'Hero', page: '2', pageSize: '50' })
    ).toEqual({ query: 'Hero', page: 2, pageSize: 50 })
    expect(directoryRouteState({ query: ['Hero'], page: '0' })).toEqual({
      query: '',
      page: 1,
      pageSize: 25
    })
  })

  it('writes only non-default route values', () => {
    expect(directoryRouteQuery(' Hero ', 2, 50)).toEqual({
      query: 'Hero',
      page: '2',
      pageSize: '50'
    })
    expect(directoryRouteQuery('', 1, 25)).toEqual({})
  })

  it('accepts only positive integer query values', () => {
    expect(positiveInteger('4', 1)).toBe(4)
    expect(positiveInteger('0', 25)).toBe(25)
    expect(positiveInteger(['2'], 25)).toBe(25)
  })

  it('reports bounded pagination ranges', () => {
    expect(paginationRange(51, 2, 25)).toEqual({ first: 26, last: 50 })
    expect(paginationRange(2, 10, 25)).toEqual({ first: 2, last: 2 })
    expect(paginationRange(0, 1, 25)).toEqual({ first: 0, last: 0 })
  })
})
