import { Route } from 'vue-router'
import authStore from '@/modules/users/store/auth.store'
import globalHubStore from '@/core/store/global-hub.store'

export const authGuard = async (to: Route, from: Route, next: CallableFunction): Promise<void> => {
  if (!authStore.me) await authStore.updateMe()
  if (!authStore.isLogged && to.name !== 'login') next({ name: 'login' })
  else if (authStore.isLogged && to.name === 'login') next({ name: 'goals' })
  else next()
}

export const hubGuard = async (to: Route, from: Route, next: CallableFunction): Promise<void> => {
  if (!globalHubStore.isActive) await globalHubStore.start()
  next()
}
