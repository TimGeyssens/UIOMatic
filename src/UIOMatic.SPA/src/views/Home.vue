
<template>
  <div>
    <h1>{{ $config.title }}</h1>
    <p>{{ $config.description }}</p>
  </div>
</template>

<script>
export default {
  name: 'Home',
}
</script>

<style scoped>
h1 {
  color: #2c3e50;
}

p {
  font-size: 14px;
  color: #777;
}
</style>