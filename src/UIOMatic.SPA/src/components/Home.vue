<template>
  <div class="home">
    <div class="home__header">
      <h1 class="home__title">Welcome to UIOMatic</h1>
      <p class="home__subtitle">Manage your content with ease</p>
    </div>

    <div class="content-types-grid">
      <div
        v-for="type in contentTypes"
        :key="type.alias"
        class="content-type-card"
      >
        <div class="content-type-card__header">
          <div class="content-type-card__icon">
            <i :class="['fas', getIconClass(type.folderIcon)]"></i>
          </div>
          <h2 class="content-type-card__title">{{ type.displayNamePlural }}</h2>
        </div>
        <p class="content-type-card__description">
          Manage your {{ type.displayNamePlural.toLowerCase() }} with a user-friendly interface
        </p>
        <div class="content-type-card__actions">
          <router-link
            :to="`/${type.alias}/list`"
            class="btn btn-secondary"
          >
            <i class="fas fa-list"></i>
            View All
          </router-link>
          <router-link
            :to="`/${type.alias}/create`"
            class="btn btn-primary"
          >
            <i class="fas fa-plus"></i>
            Create New
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { computed } from 'vue';
import { useStore } from 'vuex';
import { getIconClass } from '../utils/icons';

export default {
  name: 'Home',
  setup() {
    const store = useStore();
    const contentTypes = computed(() => {
      const types = store.state.contentTypes;
      console.log('Content Types:', types);
      return types;
    });

    return {
      contentTypes,
      getIconClass
    };
  }
};
</script>

<style scoped>
.home {
  max-width: 1200px;
  margin: 0 auto;
}

.home__header {
  text-align: center;
  margin-bottom: 3rem;
}

.home__title {
  font-size: 2.5rem;
  color: var(--primary-color);
  margin-bottom: 1rem;
}

.home__subtitle {
  font-size: 1.25rem;
  color: var(--text-light);
}

.content-types-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
  padding: 1rem;
}

.content-type-card {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
  transition: all 0.3s ease;
  border: 1px solid var(--border-color);
  display: flex;
  flex-direction: column;
}

.content-type-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 12px rgba(0, 0, 0, 0.1);
  border-color: var(--primary-color);
}

.content-type-card__header {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1rem;
}

.content-type-card__icon {
  width: 48px;
  height: 48px;
  background-color: var(--primary-color);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 1.5rem;
}

.content-type-card__title {
  font-size: 1.25rem;
  font-weight: 600;
  margin: 0;
}

.content-type-card__description {
  color: var(--text-light);
  font-size: 0.875rem;
  margin-bottom: 1.5rem;
  flex-grow: 1;
}

.content-type-card__actions {
  display: flex;
  gap: 0.75rem;
  margin-top: auto;
}

.content-type-card__actions .btn {
  flex: 1;
  justify-content: center;
}

.content-type-card__actions .btn i {
  margin-right: 0.5rem;
}
</style> 