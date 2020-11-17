<template>
  <div class="sidebar">
    <BaseNav :router="true">
      <AppSidebarHeader slot="header" />
      <BaseNavItem index="/goals" icon="edit-alt">Задачи</BaseNavItem>
      <BaseNavItem index="/projects" icon="layer-group">Проекты</BaseNavItem>
      <!--<BaseNavItem index="/groups" icon="cube">Области</BaseNavItem>-->
      <BaseNavItem index="/teams" icon="user-arrows">Команды</BaseNavItem>
      <BaseNavItem index="/users" icon="users-alt">Пользователи</BaseNavItem>
      <BaseNavItem index="/settings" icon="sliders-v-alt">Настройки</BaseNavItem>
    </BaseNav>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import BaseNav from '@/core/components/base-nav/base-nav.vue'
import BaseNavItem from '@/core/components/base-nav/base-nav-item.vue'
import SidebarFooter from '@/core/components/app-sidebar/app-sidebar-footer.vue'
import AppSidebarHeader from '@/core/components/app-sidebar/app-sidebar-header.vue'
import SettingsWindow from '@/modules/settings/components/settings-window.vue'
import settingsStore from '@/modules/settings/store/settings.store'

@Component({
  components: { SettingsWindow, AppSidebarHeader, BaseNav, BaseNavItem, SidebarFooter },
})
export default class AppSidebar extends Vue {
  private openSettings(): void {
    settingsStore.openSettings()
  }
}
</script>

<style lang="scss" scoped>
.sidebar {
  width: var(--sidebar-width);
  height: 100%;
  background-color: var(--sidebar-background);
  position: relative;
  transition: 0.2s;
}
</style>
