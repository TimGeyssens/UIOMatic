<template>
  <div class="home">
    <h1>Welcome to UIOMatic</h1>
    <p>Select a content type to manage:</p>
    <div class="content-types-grid">
      <div
        v-for="type in contentTypesOverview"
        :key="type.name"
        class="content-type-card"
      >
        <div class="content-type-card__icon">
          <i :class="['fas', getIconClass(type.icon || 'icon-folder')]"></i>
        </div>
        <div class="content-type-card__content">
          <h3>{{ type.label }}</h3>
          <p>{{ type.description || `Manage your ${type.label.toLowerCase()}` }}</p>
        </div>
        <div class="content-type-card__actions">
          <router-link 
            :to="`/${type.name}/list`" 
            class="btn btn-primary"
          >
            <i class="fas fa-list"></i>
            <span>View All</span>
          </router-link>
          <router-link 
            :to="`/${type.name}/create`" 
            class="btn btn-secondary"
          >
            <i class="fas fa-plus"></i>
            <span>Create New</span>
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
    const contentTypesOverview = computed(() => store.state.contentTypes);

    return {
      contentTypesOverview,
      getIconClass
    };
  }
};
</script>

<style scoped>
.home {
  padding: 2rem;
}

h1 {
  margin-bottom: 0.5rem;
  color: var(--text-color);
  font-size: 2rem;
}

p {
  margin-bottom: 2rem;
  color: var(--text-light);
  font-size: 1.125rem;
}

.content-types-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
  margin-top: 2rem;
}

.content-type-card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
  border: 1px solid var(--border-color);
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 1rem;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.content-type-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
}

.content-type-card__icon {
  width: 48px;
  height: 48px;
  background: var(--primary-color);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 0.5rem;
}

.content-type-card__icon i {
  font-size: 1.5rem;
  color: white;
}

.content-type-card__content {
  flex: 1;
}

.content-type-card__content h3 {
  margin: 0 0 0.5rem 0;
  color: var(--text-color);
  font-size: 1.25rem;
}

.content-type-card__content p {
  margin: 0;
  color: var(--text-light);
  font-size: 0.875rem;
  line-height: 1.5;
}

.content-type-card__actions {
  display: flex;
  gap: 0.75rem;
  margin-top: 1rem;
}

.content-type-card__actions .btn {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: 0.75rem;
  font-size: 0.875rem;
  text-decoration: none;
}

.content-type-card__actions .btn i {
  text-decoration: none;
}

.content-type-card__actions .btn span {
  text-decoration: none;
}

@media (max-width: 640px) {
  .content-types-grid {
    grid-template-columns: 1fr;
  }
  
  .content-type-card__actions {
    flex-direction: column;
  }
}
</style> 