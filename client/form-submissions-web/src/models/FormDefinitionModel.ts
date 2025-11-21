export type FormFieldDefinitionModel = {
  key: string
  label: string
  type: string
  required: boolean
  options?: string[]
  pattern?: string
  min?: number
  max?: number
}

export type FormDefinitionModel = {
  id: string
  name: string
  version: number
  fields: FormFieldDefinitionModel[]
}
