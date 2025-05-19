import { createStore } from 'vuex';

export default createStore({
  state: {
    contentTypes: [],
    apiRoot: import.meta.env.VITE_API_ROOT || 'https://localhost:7170'
  },
  mutations: {
    setContentTypes(state, types) {
      state.contentTypes = types;
    }
  },
  actions: {
    async fetchContentTypes({ commit, state }) {
      try {
        const response = await fetch(`${state.apiRoot}/UIOMatic/GetAll`);
        const data = await response.json();
        const types = data.map(type => ({
          name: type.alias,
          label: type.name,
          description: `Manage ${type.name} records.`
        }));
        commit('setContentTypes', types);
        
        // Create modules for each content type
        const modules = Object.fromEntries(
          data.map(type => [
            type.alias,
            {
              namespaced: true,
              state: {
                list: []
              },
              mutations: {
                setList(state, items) {
                  state.list = items;
                }
              },
              actions: {
                async all({ commit, rootState }) {
                  try {
                    const response = await fetch(`${rootState.apiRoot}/Object/${type.alias}`);
                    const data = await response.json();
                    commit('setList', data);
                  } catch (error) {
                    console.error(`Error fetching ${type.alias} items:`, error);
                  }
                }
              },
              getters: {
                list: state => state.list
              }
            }
          ])
        );
        
        // Register the modules
        Object.entries(modules).forEach(([name, module]) => {
          if (!this.hasModule([name])) {
            this.registerModule(name, module);
          }
        });
      } catch (error) {
        console.error('Error fetching content types:', error);
      }
    }
  },
  modules: {}
});
