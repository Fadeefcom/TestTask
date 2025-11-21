<template>
  <div class="card">
    <h2>Create submission</h2>

    <div class="row">
      <label>Name</label>
      <input v-model="name" type="text" />
      <div v-if="touched && !validators.name" class="error">Required</div>
    </div>

    <div class="row">
      <label>Email</label>
      <input v-model="email" type="email" />
      <div v-if="touched && !validators.email" class="error">Invalid email</div>
    </div>

    <div class="row">
      <label>Contact date</label>
      <input v-model="contactDate" type="date" />
      <div v-if="touched && !validators.contactDate" class="error">Required</div>
    </div>

    <div class="row">
      <label>Topic</label>
      <select v-model="topic">
        <option value="">Select</option>
        <option v-for="o in topics" :key="o" :value="o">{{ o }}</option>
      </select>
      <div v-if="touched && !validators.topic" class="error">Required</div>
    </div>

    <div class="row">
      <label>Preferred channel</label>
      <div>
        <label><input type="radio" value="email" v-model="channel" /> Email</label>
        <label><input type="radio" value="phone" v-model="channel" /> Phone</label>
      </div>
      <div v-if="touched && !validators.channel" class="error">Required</div>
    </div>

    <div class="row">
      <label><input type="checkbox" v-model="agree" /> I agree</label>
      <div v-if="touched && !validators.agree" class="error">Required</div>
    </div>

    <button :disabled="submitting" @click="submit">Submit</button>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue"
import { createForm, createSubmission } from "../api/submissionsApi"
import { required, isEmail } from "../validation/validators"

const emit = defineEmits<{ (e: "submitted"): void }>()

const formId = ref<string>("")
const topics = ["Support", "Sales", "Partnership"]
const name = ref("")
const email = ref("")
const contactDate = ref("")
const topic = ref("")
const channel = ref("")
const agree = ref(false)

const touched = ref(false)
const submitting = ref(false)

const validators = computed(() => ({
  name: required(name.value),
  email: required(email.value) && isEmail(email.value),
  contactDate: required(contactDate.value),
  topic: required(topic.value),
  channel: required(channel.value),
  agree: agree.value === true
}))

onMounted(async () => {
  const def = {
    name: "Contact Form",
    fields: [
      { key: "name", label: "Name", type: "Text", required: true },
      { key: "email", label: "Email", type: "Text", required: true, pattern: "^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$" },
      { key: "contactDate", label: "Contact date", type: "Date", required: true },
      { key: "topic", label: "Topic", type: "Dropdown", required: true, options: topics },
      { key: "channel", label: "Preferred channel", type: "Radio", required: true, options: ["email", "phone"] },
      { key: "agree", label: "Agree", type: "Checkbox", required: true }
    ]
  }
  const created = await createForm(def)
  formId.value = created.id
})

async function submit() {
  touched.value = true
  if (!Object.values(validators.value).every(Boolean)) return

  submitting.value = true
  try {
    await createSubmission({
      formId: formId.value,
      payload: {
        name: name.value,
        email: email.value,
        contactDate: contactDate.value,
        topic: topic.value,
        channel: channel.value,
        agree: agree.value
      }
    })
    name.value = ""
    email.value = ""
    contactDate.value = ""
    topic.value = ""
    channel.value = ""
    agree.value = false
    touched.value = false
    emit("submitted")
  } finally {
    submitting.value = false
  }
}
</script>
