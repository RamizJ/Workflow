<template>
  <div class="sidebar">
    <NavMenu :router="true">
      <NavMenuItem index="/areas" icon="cube" disabled>Области</NavMenuItem>
      <NavMenuItem index="/projects" icon="layer-group">Проекты</NavMenuItem>
      <NavMenuItem index="/goals" icon="edit-alt">Задачи</NavMenuItem>
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
import SidebarFooter from '@/core/components/app-sidebar-footer.vue'

@Component({
  components: { NavMenu, NavMenuItem, SidebarFooter },
})
export default class AppSidebar extends Vue {}
</script>

<style lang="scss" scoped>
.sidebar {
  height: 100%;
  background-color: var(--sidebar-background);
  border-right: 1px solid var(--sidebar-border);
  transition: background-color 0.25s, border-color 0.25s;
  position: relative;
  width: var(--sidebar-width);
}
</style>
