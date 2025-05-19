<template>
  <div class="list-view">
    <div class="card">
      <div class="card__header">
        <div class="header-content">
          <i :class="['fas', getIconClass(typeInfo?.folderIcon)]"></i>
          <h1 class="card__title">{{ typeInfo?.displayNamePlural || 'Loading...' }}</h1>
        </div>
        <div class="header-actions">
          <div class="search-wrapper">
            <i class="fas fa-search search-icon"></i>
            <input
              v-model="searchTerm"
              type="text"
              class="form-control search-input"
              placeholder="Search..."
              @input="handleSearch"
            />
          </div>
          <router-link
            :to="`/${type}/create`"
            class="btn btn-primary"
          >
            <i class="fas fa-plus"></i>
            Create New
          </router-link>
        </div>
      </div>

      <div v-if="isLoading" class="card__body">
        <div class="list-view__loading">
          <div class="loading-spinner">
            <i class="fas fa-spinner fa-spin"></i>
          </div>
          <p>Loading {{ typeInfo?.displayNamePlural || 'items' }}...</p>
        </div>
      </div>

      <div v-else-if="items.length === 0" class="card__body">
        <div class="list-view__empty">
          <i class="fas fa-inbox"></i>
          <p>No {{ typeInfo?.displayNamePlural || 'items' }} found</p>
          <router-link
            :to="`/${type}/create`"
            class="btn btn-primary"
          >
            <i class="fas fa-plus"></i>
            Create New
          </router-link>
        </div>
      </div>

      <div v-else class="card__body">
        <table class="table">
          <thead>
            <tr>
              <th
                v-for="column in columns"
                :key="column.name"
                @click="handleSort(column.name)"
                class="sortable"
              >
                {{ column.label }}
                <i
                  v-if="sortColumn === column.name"
                  :class="[
                    'fas',
                    sortOrder === 'asc' ? 'fa-sort-up' : 'fa-sort-down'
                  ]"
                ></i>
                <i v-else class="fas fa-sort"></i>
              </th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in items" :key="item.id">
              <td v-for="column in columns" :key="column.name">
                {{ item[column.name] }}
              </td>
              <td class="actions">
                <router-link
                  :to="`/${type}/edit/${item.id}`"
                  class="btn btn-secondary btn-sm"
                >
                  <i class="fas fa-edit"></i>
                  Edit
                </router-link>
                <button
                  @click="handleDelete(item.id)"
                  class="btn btn-danger btn-sm"
                >
                  <i class="fas fa-trash"></i>
                  Delete
                </button>
              </td>
            </tr>
          </tbody>
        </table>

        <div class="pagination">
          <button
            class="btn btn-secondary"
            :disabled="currentPage === 1"
            @click="handlePageChange(currentPage - 1)"
          >
            <i class="fas fa-chevron-left"></i>
            Previous
          </button>
          <span class="pagination-info">
            Page {{ currentPage }} of {{ totalPages }}
          </span>
          <button
            class="btn btn-secondary"
            :disabled="currentPage === totalPages"
            @click="handlePageChange(currentPage + 1)"
          >
            Next
            <i class="fas fa-chevron-right"></i>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted, watch, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { UIOMaticService } from '../services/uiomatic.service';
import { config } from '../config';
import { getIconClass } from '../utils/icons';
import { toast } from './Toast.vue';

export default {
  name: 'ListView',
  props: {
    type: {
      type: String,
      required: true
    }
  },
  setup(props) {
    const route = useRoute();
    const router = useRouter();
    const typeInfo = ref(null);
    const items = ref([]);
    const currentPage = ref(1);
    const totalPages = ref(1);
    const sortColumn = ref(config.listView.defaultSortColumn);
    const sortOrder = ref(config.listView.defaultSortOrder);
    const filters = ref({});
    const searchTerm = ref('');
    const isLoading = ref(true);

    const columns = computed(() => {
      if (!typeInfo.value?.listViewProperties) return [];
      return typeInfo.value.listViewProperties.map(prop => ({
        name: prop.columnName.toLowerCase(),
        label: prop.label
      }));
    });

    const loadTypeInfo = async () => {
      try {
        console.log('Loading type info for:', props.type);
        const types = await UIOMaticService.getAllTypes();
        console.log('Available types:', types);
        typeInfo.value = types.find(t => t.alias === props.type);
        if (!typeInfo.value) {
          console.error('Type not found:', props.type);
          typeInfo.value = {
            alias: props.type,
            displayNamePlural: props.type,
            listViewProperties: []
          };
          return;
        }
        console.log('Type info loaded:', typeInfo.value);
        console.log('ListView properties:', typeInfo.value.listViewProperties);
        console.log('Type info structure:', {
          alias: typeInfo.value.alias,
          displayNamePlural: typeInfo.value.displayNamePlural,
          listViewProperties: typeInfo.value.listViewProperties,
          editableProperties: typeInfo.value.editableProperties
        });
      } catch (error) {
        console.error('Error loading type info:', error);
        typeInfo.value = {
          alias: props.type,
          displayNamePlural: props.type,
          listViewProperties: []
        };
      }
    };

    const loadData = async () => {
      try {
        isLoading.value = true;
        console.log('Loading data for type:', props.type);
        console.log('Props:', props);
        console.log('Route params:', route.params);
        
        if (!props.type) {
          console.error('type prop is missing');
          return;
        }

        const response = await UIOMaticService.getItems(
          props.type,
          currentPage.value,
          config.listView.itemsPerPage,
          sortColumn.value,
          sortOrder.value,
          filters.value,
          searchTerm.value
        );
        console.log('API Response:', response);
        items.value = response.items || [];
        totalPages.value = response.totalPages || 1;
        console.log('Items loaded:', items.value);
        if (items.value.length > 0) {
          console.log('First item structure:', items.value[0]);
          console.log('Available properties:', Object.keys(items.value[0]));
        }
      } catch (error) {
        console.error('Error loading items:', error);
        items.value = [];
        totalPages.value = 1;
      } finally {
        isLoading.value = false;
      }
    };

    const handleSort = async (column) => {
      if (sortColumn.value === column) {
        sortOrder.value = sortOrder.value === 'ASC' ? 'DESC' : 'ASC';
      } else {
        sortColumn.value = column;
        sortOrder.value = 'ASC';
      }
      // Reset current page when sorting changes
      currentPage.value = 1;
      // Clear existing items before loading new ones
      items.value = [];
      await loadData();
    };

    const handleFilter = () => {
      currentPage.value = 1;
      loadData();
    };

    const handleSearch = () => {
      currentPage.value = 1;
      loadData();
    };

    const handlePageChange = (page) => {
      currentPage.value = page;
      loadData();
    };

    const handleDelete = async (id) => {
      if (confirm('Are you sure you want to delete this item?')) {
        try {
          console.log('Deleting item:', id);
          await UIOMaticService.deleteItems(props.type, [id]);
          toast.success('Item deleted successfully');
          loadData();
        } catch (error) {
          console.error('Error deleting item:', error);
          toast.error('Failed to delete item');
        }
      }
    };

    const getInputType = (type) => {
      switch (type.toLowerCase()) {
        case 'datetime':
          return 'datetime-local';
        case 'number':
          return 'number';
        default:
          return 'text';
      }
    };

    onMounted(async () => {
      console.log('ListView mounted with type:', props.type);
      await loadTypeInfo();
      await loadData();
    });

    // Watch for route changes
    watch(() => props.type, async (newType) => {
      console.log('Type changed to:', newType);
      if (newType) {
        await loadTypeInfo();
        await loadData();
      }
    }, { immediate: true });

    // Watch for page changes
    watch([currentPage, sortColumn, sortOrder, searchTerm], () => {
      loadData();
    });

    return {
      typeInfo,
      items,
      currentPage,
      totalPages,
      sortColumn,
      sortOrder,
      filters,
      searchTerm,
      columns,
      isLoading,
      handleSort,
      handleFilter,
      handleSearch,
      handlePageChange,
      handleDelete,
      getInputType,
      getIconClass
    };
  }
};
</script>

<style scoped>
.list-view {
  max-width: 1200px;
  margin: 0 auto;
}

.header-content {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.header-content i {
  font-size: 1.5rem;
  color: var(--primary-color);
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.search-wrapper {
  position: relative;
}

.search-icon {
  position: absolute;
  left: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  color: var(--text-light);
}

.search-input {
  padding-left: 2.5rem;
  width: 300px;
}

.sortable {
  cursor: pointer;
  user-select: none;
  white-space: nowrap;
}

.sortable i {
  margin-left: 0.5rem;
  color: var(--text-light);
}

.actions {
  display: flex;
  gap: 0.5rem;
  justify-content: flex-end;
}

.btn-sm {
  padding: 0.375rem 0.75rem;
  font-size: 0.75rem;
}

.pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  margin-top: 2rem;
}

.pagination-info {
  color: var(--text-light);
  font-size: 0.875rem;
}

/* Table styles */
.table th {
  background-color: var(--background-color);
  font-weight: 600;
  text-align: left;
  padding: 1rem;
  border-bottom: 2px solid var(--border-color);
}

.table td {
  padding: 1rem;
  border-bottom: 1px solid var(--border-color);
}

.table tr:hover {
  background-color: var(--background-color);
}

/* Button hover effects */
.btn:hover {
  transform: translateY(-1px);
}

.btn:active {
  transform: translateY(0);
}

/* Search input focus effect */
.search-input:focus {
  border-color: var(--primary-color);
  box-shadow: 0 0 0 3px rgba(79, 70, 229, 0.1);
}

.list-view__loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 300px;
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
  border: 1px solid var(--border-color);
}

.loading-spinner {
  font-size: 2rem;
  color: var(--primary-color);
  margin-bottom: 1rem;
}

.list-view__loading p {
  color: var(--text-light);
  font-size: 1rem;
}

.list-view__empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 300px;
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
  border: 1px solid var(--border-color);
  gap: 1rem;
}

.list-view__empty i {
  font-size: 3rem;
  color: var(--text-light);
}

.list-view__empty p {
  color: var(--text-light);
  font-size: 1.25rem;
}
</style> 