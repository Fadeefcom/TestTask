<template>
  <div class="card">
    <h2>Submissions</h2>

    <SearchBar @search="onSearch" />

    <div class="list">
      <div v-for="s in items" :key="s.id" class="item">
        <div class="meta">{{ s.id }} | {{ s.formId }} | {{ new Date(s.createdAt).toLocaleString() }}</div>
        <div class="kv">
          <template v-for="(v,k) in s.payload" :key="k">
            <div>{{ k }}</div>
            <div>{{ format(v) }}</div>
          </template>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue"
import SearchBar from "./SearchBar.vue"
import { searchSubmissions } from "../api/submissionsApi"
import type { SubmissionModel } from "../models/SubmissionModel"

const items = ref<SubmissionModel[]>([])
const current = ref<{ formId?: string; query?: string; from?: string; to?: string }>({})

async function load() {
  items.value = await searchSubmissions(current.value)
}

function onSearch(p: any) {
  current.value = p
  load()
}

function format(v: any) {
  if (typeof v === "boolean") return v ? "true" : "false"
  if (v === null || v === undefined) return ""
  return String(v)
}

defineExpose({ load })

load()
</script>
