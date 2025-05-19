<template>
  <nav class="breadcrumb" aria-label="breadcrumb">
    <ol class="breadcrumb__list">
      <li class="breadcrumb__item">
        <router-link to="/" class="breadcrumb__link">
          <i class="fas fa-home"></i>
          <span>Home</span>
        </router-link>
      </li>
      <template v-if="breadcrumbs.length > 0">
        <li v-for="(crumb, index) in breadcrumbs" :key="index" class="breadcrumb__item">
          <i class="fas fa-chevron-right breadcrumb__separator"></i>
          <router-link 
            v-if="crumb.path && index !== breadcrumbs.length - 1" 
            :to="crumb.path" 
            class="breadcrumb__link"
          >
            {{ crumb.title }}
          </router-link>
          <span v-else class="breadcrumb__text">{{ crumb.title }}</span>
        </li>
      </template>
    </ol>
  </nav>
</template>

<script>
import { computed } from 'vue';
import { useRoute } from 'vue-router';

export default {
  name: 'Breadcrumb',
  setup() {
    const route = useRoute();

    const breadcrumbs = computed(() => {
      const paths = route.path.split('/').filter(Boolean);
      const result = [];
      let currentPath = '';

      paths.forEach((path, index) => {
        currentPath += `/${path}`;
        
        // Skip the 'list' path as it's redundant
        if (path === 'list') return;

        let title = path;
        
        // Format the title
        if (path === 'create') {
          title = 'Create New';
        } else if (path === 'edit') {
          title = 'Edit';
        } else {
          // Capitalize and replace dashes with spaces
          title = path
            .split('-')
            .map(word => word.charAt(0).toUpperCase() + word.slice(1))
            .join(' ');
        }

        result.push({
          title,
          path: index === paths.length - 1 ? null : currentPath
        });
      });

      return result;
    });

    return {
      breadcrumbs
    };
  }
};
</script>

<style scoped>
.breadcrumb {
  margin-bottom: 1.5rem;
}

.breadcrumb__list {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.breadcrumb__item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.breadcrumb__link {
  color: var(--text-color);
  text-decoration: none;
  font-size: 0.875rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: color 0.2s ease;
}

.breadcrumb__link:hover {
  color: var(--primary-color);
}

.breadcrumb__text {
  color: var(--text-light);
  font-size: 0.875rem;
}

.breadcrumb__separator {
  color: var(--text-light);
  font-size: 0.75rem;
}

.breadcrumb__link i {
  font-size: 1rem;
}
</style> 