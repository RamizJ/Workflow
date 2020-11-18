<template>
  <BasePage>
    <BasePageHeader>{{ userFio }}</BasePageHeader>
    <BasePageSubheader :no-border="true">
      <BaseTabs v-model="currentTab" :tabs="tabs" @tab-click="setTab" :routing="true" />
    </BasePageSubheader>
    <RouterView />
  </BasePage>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import BasePage from '@/core/components/base-page/base-page.vue'
import BasePageHeader from '@/core/components/base-page/base-page-header.vue'
import BasePageSubheader from '@/core/components/base-page/base-page-subheader.vue'
import BaseTabs from '@/core/components/base-tabs/base-tabs.vue'
import User from '@/modules/users/models/user.type'
import usersStore from '../store/users.store'

@Component({
  components: { BaseTabs, BasePageSubheader, BasePageHeader, BasePage },
})
export default class UserPage extends Vue {
  private loading = false
  private entity: User | null = null
  private currentTab = 'info'
  private tabs: Array<{ label: string; name: string }> = [
    { label: 'Информация', name: 'info' },
    { label: 'Статистика', name: 'statistics' },
  ]

  private get id(): string {
    return this.$route.params.userId
  }

  private get userFio(): string {
    if (this.entity)
      return `${this.entity.lastName} ${this.entity.firstName} ${this.entity.middleName}`
    else return ''
  }

  protected async mounted(): Promise<void> {
    this.loading = true
    ;(this as any).$insProgress.start()
    this.loadTab()
    const entity = usersStore.user || (await usersStore.findOneById(this.id))
    this.entity = { ...entity }
    usersStore.setUser(entity)
    ;(this as any).$insProgress.finish()
    this.loading = false
  }

  protected beforeDestroy(): void {
    usersStore.setUser(null)
  }

  private loadTab() {
    const tab = this.$route.path.substring(this.$route.path.lastIndexOf('/') + 1)
    switch (tab) {
      case 'statistics':
        this.currentTab = tab
        break
      default:
        this.currentTab = 'info'
        if (this.$route.name !== 'user-info') this.$router.replace({ name: `user-info` })
        break
    }
  }

  private setTab() {
    const tab = this.$route.path.substring(this.$route.path.lastIndexOf('/') + 1)
    let path = this.$route.path

    switch (tab) {
      case 'statistics':
      case 'info':
        path = path.replace(tab, this.currentTab)
        break
      default:
        path = this.currentTab === 'info' ? path : `${path}/${this.currentTab}`
        break
    }

    const targetRoute = `user-${this.currentTab}`
    if (this.$route.name !== targetRoute) this.$router.replace({ name: targetRoute })
  }
}
</script>

<style lang="scss" scoped></style>
