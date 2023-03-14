<template>
  <div id="app">
    <nav class="nav-wrap">
      <div class="nav container">
        <RouterLink to="/" class="logo">{{ name }}</RouterLink>
        <ul>
          <li v-for="contentType in contentTypes" :key="contentType.name">
            <RouterLink :to="`/${contentType.name}/list`">{{ contentType.label }}</RouterLink>
          </li>
        </ul>
      </div>
    </nav>
    <main class="main container">
      <RouterView :key="this.$route.path"/>
    </main>
  </div>
</template>

<script>
import config from "./config";



export default {
  name: "App",
  created() {
    this.name = config.name;
    this.contentTypes = config.contentTypes;
  }
};
</script>

<style>
body {
  margin: 0;
  padding: 0;
}

*,
*::before,
*::after {
  box-sizing: border-box;
}

#app {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2c3e50;
}

.nav-wrap {
  padding-top: 1em;
  padding-bottom: 1em;
  border-bottom: 1px solid #ccc;
}

.nav {
  display: flex;
  justify-content: space-between;
}

.nav ul {
  display: flex;
  list-style-type: none;
  margin: 0;
  padding: 0;
}

.nav li:not(:first-child) {
  margin-left: 0.5em;
}

.nav li:not(:first-child)::before {
  content: "|";
  margin-right: 0.5em;
}

.main {
  margin-top: 2em;
}

.container {
  max-width: 42em;
  margin-right: auto;
  margin-left: auto;
  padding-right: 1em;
  padding-left: 1em;
}
</style>
