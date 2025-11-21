const baseUrl = import.meta.env.VITE_API_BASE_URL || ""

export async function createForm(definition: any) {
  const r = await fetch(`${baseUrl}/api/forms`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(definition)
  })
  if (!r.ok) throw new Error(await r.text())
  return r.json()
}

export async function listForms() {
  const r = await fetch(`${baseUrl}/api/forms`)
  if (!r.ok) throw new Error(await r.text())
  return r.json()
}

export async function createSubmission(payload: any) {
  const r = await fetch(`${baseUrl}/api/submissions`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload)
  })
  if (!r.ok) throw new Error(await r.text())
  return r.json()
}

export async function searchSubmissions(params: Record<string, any>) {
  const qs = new URLSearchParams()
  for (const k of Object.keys(params)) {
    if (params[k] !== null && params[k] !== undefined && params[k] !== "")
      qs.set(k, params[k])
  }
  const r = await fetch(`${baseUrl}/api/submissions?${qs.toString()}`)
  if (!r.ok) throw new Error(await r.text())
  return r.json()
}
