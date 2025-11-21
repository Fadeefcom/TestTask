<template>
  <div class="row">
    <label>Search</label>
    <input v-model="query" type="text" placeholder="text contains" />

    <label>FormId</label>
    <input v-model="formId" type="text" placeholder="optional guid" />

    <label>From</label>
    <input v-model="from" type="date" />

    <label>To</label>
    <input v-model="to" type="date" />

    <button @click="apply">Apply</button>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue"

const emit = defineEmits<{ (e: "search", p: any): void }>()

const query = ref("")
const formId = ref("")
const from = ref("")
const to = ref("")

function apply() {
  emit("search", {
    query: query.value,
    formId: formId.value,
    from: from.value ? new Date(from.value).toISOString() : null,
    to: to.value ? new Date(to.value).toISOString() : null
  })
}
</script>
