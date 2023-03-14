<template>
    <table>
        <thead>
            <tr>
                <th>ID</th>
                <th v-for="field in contentType.fields" :key="`${field.name}-th`">{{ field.label }}</th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="item in items"
                :key="item.Id"
                @click="$router.push(`/${contentType.name}/${item.Id}/edit`)">
                <td>{{ item.Id }}</td>
                <td v-for="field in contentType.fields"
                    :key="`${field.name}-${item.Id}`">
                    {{ item[field.name] }}
                </td>
            </tr>
        </tbody>
    </table>
</template>

<script>
export default {
  name: "ListingContainer",
  props: {
    contentType: {
      required: true,
      type: Object
    }
  },
  computed: {
    items() {
      return this.$store.getters[`${this.contentType.name}/list`];
    }
  },
  async created() {
    await this.$store.dispatch(`${this.contentType.name}/all`);
  }
};
</script>

<style>
table {
  width: 100%;
  border-collapse: collapse;
}

thead th {
  text-align: left;
}

tbody tr {
  cursor: pointer;
  transition: background-color 0.2s;
}

tbody tr:nth-child(odd) {
  background-color: #f9f9f9;
}

tbody tr:hover {
  background-color: #efefef;
}

td,
th {
  padding: 0.25em;
}
</style>
