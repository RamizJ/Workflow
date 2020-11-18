<template>
  <BasePage>
    <BasePageHeader>
      <input
        v-if="!loading"
        v-model="entity.name"
        v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
        @change="updateEntity"
      />
    </BasePageHeader>
    <BasePageSubheader>
      <input
        placeholder="Описание..."
        v-model="entity.description"
        v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
        @change="updateEntity"
      />
    </BasePageSubheader>
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
import Group from '@/modules/groups/models/group.model'
import groupsStore from '../store/groups.store'
import BaseTabs from '@/core/components/base-tabs/base-tabs.vue'

@Component({
  components: { BaseTabs, BasePageSubheader, BasePageHeader, BasePage },
})
export default class GroupPage extends Vue {
  private loading = true
  private entity: Group = new Group()
  private currentTab = 'overview'
  private tabs: Array<{ label: string; name: string; component?: any }> = [
    { label: 'Проекты', name: 'projects' },
  ]

  protected async mounted(): Promise<void> {
    this.loading = true
    ;(this as any).$insProgress.start()
    this.loadTab()
    const groupId = parseInt(this.$route.params.groupId)
    const group = groupsStore.group || (await groupsStore.get(groupId))
    this.entity = { ...group }
    groupsStore.setGroup(group)
    ;(this as any).$insProgress.finish()
    this.loading = false
  }

  private loadTab() {
    const tab = this.$route.path.substring(this.$route.path.lastIndexOf('/') + 1)
    switch (tab) {
      case 'projects':
        this.currentTab = tab
        break
      default:
        this.currentTab = 'projects'
        if (this.$route.name !== 'group-projects') this.$router.replace({ name: `group-projects` })
        break
    }
  }

  private setTab() {
    const tab = this.$route.path.substring(this.$route.path.lastIndexOf('/') + 1)
    let path = this.$route.path

    switch (tab) {
      case 'projects':
        path = path.replace(tab, this.currentTab)
        break
      default:
        path = this.currentTab === 'projects' ? path : `${path}/${this.currentTab}`
        break
    }

    const targetRoute = `group-${this.currentTab}`
    if (this.$route.name !== targetRoute) this.$router.replace({ name: targetRoute })
  }

  private async updateEntity(): Promise<void> {
    await groupsStore.update(this.entity)
  }
}
</script>

<style lang="scss" scoped></style>
