<template>
  <div class="sidebar">
    <NavMenu :router="true">
      <AppSidebarHeader slot="header" />
      <NavMenuItem index="/goals" icon="edit-alt">Задачи</NavMenuItem>
      <NavMenuItem index="/projects" icon="layer-group">Проекты</NavMenuItem>
      <!--      <NavMenuItem index="/areas" icon="cube" disabled>Области</NavMenuItem>-->
      <NavMenuItem index="/teams" icon="user-arrows">Команды</NavMenuItem>
      <NavMenuItem index="/users" icon="users-alt">Пользователи</NavMenuItem>
      <SidebarFooter slot="footer" />
    </NavMenu>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import NavMenu from '@/core/components/base-nav-menu.vue'
import NavMenuItem from '@/core/components/base-nav-menu-item.vue'
import SidebarFooter from '@/core/components/app-sidebar/app-sidebar-footer.vue'
import AppSidebarHeader from '@/core/components/app-sidebar/app-sidebar-header.vue'

@Component({
  components: { AppSidebarHeader, NavMenu, NavMenuItem, SidebarFooter },
})
export default class AppSidebar extends Vue {}
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
