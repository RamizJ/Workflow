import store from '~/store';

export const authGuard = async (to, from, next) => {
  const me = store.getters['auth/me'];
  if (!me) {
    try {
      await store.dispatch('auth/fetchMe');
    } catch (e) {
      console.log(e);
    }
  }
  const loggedIn = store.getters['auth/loggedIn'];
  if (!loggedIn && to.name !== 'Login') next({ name: 'Login' });
  else if (loggedIn && to.name === 'Login') next({ name: 'Tasks' });
  else next();
};
