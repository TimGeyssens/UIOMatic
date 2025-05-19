<template>
  <form @submit.prevent="save">
    <div v-for="field in contentType.fields" :key="field.name" class="control">
      <label :for="field.name">{{ field.label }}</label>
      <!-- This is a very simple implementation. -->
      <!-- In a real app you would dynamically load -->
      <!-- different components for certain field types -->
      <input v-if="field.type === 'text'" :id="field.name"  v-model="item[field.name]">
      <textarea v-else-if="field.type === 'textarea'" :id="field.name" :value="item[field.name]"/>
    </div>
    <button>Save</button>
  </form>
</template>

<script>
export default {
  name: "ListingContainer",
  props: {
    contentType: {
      required: true,
      type: Object
    },
    id: {
      required: true,
      type: String
    }
  },
  computed: {
    item() {
      return this.$store.getters[`${this.contentType.name}/find`](this.id);
    }
  },
  async created() {
    await this.$store.dispatch(`${this.contentType.name}/load`, this.id);
  },
  methods: {
    save() {
          this.$store.dispatch(`${this.contentType.name}/update`, this.item);
    }
  }
};
</script>

<style>
label {
  display: block;
}

.control {
  margin-bottom: 1em;
}
</style>
