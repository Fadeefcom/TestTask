<template>
  <div class="container">
    <h1 class="title">Form Submissions</h1>
    <ContactForm @submitted="reload" />
    <SubmissionList ref="list" />
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue"
import ContactForm from "./components/ContactForm.vue"
import SubmissionList from "./components/SubmissionList.vue"

const list = ref<InstanceType<typeof SubmissionList> | null>(null)

function reload() {
  list.value?.load()
}
</script>
