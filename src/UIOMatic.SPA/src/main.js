import { createApp } from 'vue';
import App from './App.vue';
import router from './router';
import store from './store';

const app = createApp(App);

app.use(router);
app.use(store);

// Remove the initial content types fetch
// store.dispatch('fetchContentTypes');

app.mount('#app');
