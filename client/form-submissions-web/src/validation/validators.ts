export function required(v: any) {
  return v !== null && v !== undefined && v !== ""
}

export function isEmail(v: string) {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v)
}
