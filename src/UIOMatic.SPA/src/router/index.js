import { createRouter, createWebHistory } from 'vue-router';
import { AuthService } from '../services/auth.service';
import Login from '../components/Login.vue';
import Home from '../views/Home.vue';

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: Login,
    meta: { requiresAuth: false }
  },
  {
    path: '/',
    name: 'Home',
    component: Home,
    meta: { requiresAuth: true }
  },
  {
    path: '/:type/list',
    name: 'List',
    component: () => import('../components/ListView.vue'),
    props: true,
    meta: { requiresAuth: true }
  },
  {
    path: '/:type/create',
    name: 'Create',
    component: () => import('../components/EditForm.vue'),
    props: true,
    meta: { requiresAuth: true }
  },
  {
    path: '/:type/edit/:id',
    name: 'Edit',
    component: () => import('../components/EditForm.vue'),
    props: true,
    meta: { requiresAuth: true }
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

router.beforeEach((to, from, next) => {
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth);
  const isAuthenticated = AuthService.isAuthenticated();

  if (requiresAuth && !isAuthenticated) {
    next('/login');
  } else if (to.path === '/login' && isAuthenticated) {
    next('/');
  } else {
    next();
  }
});

export default router;
