import store from '~/store';

export const authGuard = async (to, from, next) => {
  const token = store.getters['auth/token'];
  const me = store.getters['auth/me'];

  if (!token) await store.dispatch('auth/logout');
  if (!me) await store.dispatch('auth/fetchMe');

  const loggedIn = store.getters['auth/loggedIn'];
  if (!loggedIn && to.name !== 'Login') next({ name: 'Login' });
  else if (loggedIn && to.name === 'Login') next({ name: 'Tasks' });
  else next();
};
