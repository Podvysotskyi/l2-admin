export function httpStatusCode(cause: unknown): number | undefined {
  if (typeof cause !== 'object' || cause === null || !('statusCode' in cause)) {
    return undefined
  }

  const statusCode = Number(cause.statusCode)
  return Number.isFinite(statusCode) ? statusCode : undefined
}
