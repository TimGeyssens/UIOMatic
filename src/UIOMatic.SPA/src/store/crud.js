import Vue from "vue";

export default function makeCrudModule({
  normalizeRelations = x => x,
  resolveRelations = x => x,
  service
} = {}) {
  return {
    // Actions for `create`, `update` and `delete` omitted for brevity.
    actions: {
        update: async ({ commit }, object) => {
        if (!service) throw new Error("No service specified!");
        await service.update(object);
        },
      all: async ({ commit }) => {
        // It is not strictly necessary to pass a service,
        // but if none was passed, no data can be loaded.
        if (!service) throw new Error("No service specified!");

      
        const items = await service.list();
      
        items.forEach(item => {
          // Noramlize nested data and swap the related objects
          // in the API response with an ID reference.
          commit("add", normalizeRelations(item));
        });
      },
      load: async ({ commit }, id) => {
        if (!service) throw new Error("No service specified!");

        const item = await service.find(id);
        commit("add", normalizeRelations(item));
      }
    },
    getters: {
      // Return a single item with the given id.
      find: state => id => {
        // Swap ID referenes with the resolved objects.
        return resolveRelations(state.byId[id]);
      },
      // Return a list of items in the order of `allIds`.
      list: (state, getters) => {
        return state.allIds.map(id => getters.find(id));
      }
    },
    mutations: {
      add: (state, item) => {
        Vue.set(state.byId, item.Id, item);
        if (state.allIds.includes(item.Id)) return;
        state.allIds.push(item.Id);
      }
    },
    namespaced: true,
    state: {
      byId: {},
      allIds: []
    }
  };
}
