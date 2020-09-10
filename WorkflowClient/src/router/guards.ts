import { Route } from 'vue-router'
import authModule from '@/store/modules/auth.module'

export const authGuard = async (to: Route, from: Route, next: CallableFunction): Promise<void> => {
  if (!authModule.token) await authModule.logout()
  if (!authModule.me) await authModule.getMe()

  if (!authModule.loggedIn && to.name !== 'Login') next({ name: 'Login' })
  else if (authModule.loggedIn && to.name === 'Login') next({ name: 'Tasks' })
  else next()
}
