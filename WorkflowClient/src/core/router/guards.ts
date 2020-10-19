import { Route } from 'vue-router'
import authModule from '@/modules/users/store/auth.store'

export const authGuard = async (to: Route, from: Route, next: CallableFunction): Promise<void> => {
  if (!authModule.me) await authModule.updateMe()
  if (!authModule.isLogged && to.name !== 'Login') next({ name: 'Login' })
  else if (authModule.isLogged && to.name === 'Login') next({ name: 'Tasks' })
  else next()
}
