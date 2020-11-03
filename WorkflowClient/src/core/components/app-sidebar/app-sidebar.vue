<template>
  <div class="sidebar">
    <Nav :router="true">
      <AppSidebarHeader slot="header" />
      <NavItem index="/goals" icon="edit-alt">Задачи</NavItem>
      <NavItem index="/projects" icon="layer-group">Проекты</NavItem>
      <!--<NavItem index="/areas" icon="cube" disabled>Области</NavItem>-->
      <NavItem index="/teams" icon="user-arrows">Команды</NavItem>
      <NavItem index="/users" icon="users-alt">Пользователи</NavItem>
      <NavItem index="/settings" icon="sliders-v-alt">Настройки</NavItem>
    </Nav>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import Nav from '@/core/components/base-nav/base-nav.vue'
import NavItem from '@/core/components/base-nav/base-nav-item.vue'
import SidebarFooter from '@/core/components/app-sidebar/app-sidebar-footer.vue'
import AppSidebarHeader from '@/core/components/app-sidebar/app-sidebar-header.vue'
import SettingsWindow from '@/modules/settings/components/settings-window.vue'
import settingsStore from '@/modules/settings/store/settings.store'

@Component({
  components: { SettingsWindow, AppSidebarHeader, Nav, NavItem, SidebarFooter },
})
export default class AppSidebar extends Vue {
  private openSettings(): void {
    settingsStore.openSettings()
  }
}
</script>

<style lang="scss" scoped>
.sidebar {
  height: 100%;
  background-color: var(--sidebar-background);
  transition: background-color 0.25s, border-color 0.25s;
  position: relative;
  width: var(--sidebar-width);
}
</style>
