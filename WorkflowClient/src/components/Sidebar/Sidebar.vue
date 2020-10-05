<template>
  <div class="sidebar">
    <el-menu :router="true" :default-active="$route.path">
      <!--<el-menu-item index="/">
        <unicon name="apps" />
        <span>Обзор</span>
      </el-menu-item>-->
      <!--<el-menu-item index="/areas">
        <unicon name="cube" />
        <span>Области</span>
      </el-menu-item>-->
      <el-menu-item index="/projects">
        <unicon name="layer-group" />
        <span>Проекты</span>
      </el-menu-item>
      <el-menu-item index="/tasks">
        <unicon name="edit-alt" />
        <span>Задачи</span>
      </el-menu-item>
      <el-menu-item index="/teams">
        <unicon name="user-arrows" />
        <span>Команды</span>
      </el-menu-item>
      <el-menu-item index="/users">
        <unicon name="users-alt" />
        <span>Пользователи</span>
      </el-menu-item>
      <div class="divider"></div>
      <!--<el-menu-item index="/settings">
        <unicon name="sliders-v-alt" /><span>Настройки</span>
      </el-menu-item>-->

      <div class="bottom-bar">
        <sidebar-add-new />
        <sidebar-open-settings />
      </div>
    </el-menu>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import SidebarAddNew from '@/components/Sidebar/SidebarAddNew.vue'
import SidebarOpenSettings from '@/components/Sidebar/SidebarOpenSettings.vue'

@Component({
  components: {
    SidebarAddNew,
    SidebarOpenSettings,
  },
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
.el-menu {
  overflow: auto;
  border: none;
  height: 100%;
  position: relative;
  background-color: transparent;
  padding: 18px;
}
.el-menu-item,
.el-submenu,
.el-submenu__title {
  color: var(--sidebar-text);
  height: auto;
  line-height: 38px;
  font-size: 13.5px;
  font-weight: 500;
  letter-spacing: 0.2px;
  padding: 0 4px !important;
  margin: 4px 0;
  border-radius: 6px;
  display: flex;
  align-items: center;
  z-index: 1;
  transition: none;
  span {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    word-break: break-all;
  }
}

.el-menu-item.is-active {
  background-color: var(--color-primary);
  color: white;
  .unicon {
    fill: white;
  }
}
.el-menu-item:hover:not(.is-active) {
  outline: none;
  background-color: var(--sidebar-item-hover-background);
}
.el-submenu .unicon,
.el-menu-item .unicon {
  fill: var(--sidebar-text);
  opacity: 0.85;
}
.el-menu--collapse {
  width: var(--header-height);
}
.divider {
  height: 15px;
}
.el-collapse {
  margin-bottom: 110px;
}

.logo {
  width: 100%;
  position: absolute;
  display: flex;
  justify-content: center;
  padding: 30px 10px;
  bottom: 0;
  left: 0;
  z-index: 2;
  img {
    opacity: 0.8;
    height: 80px;
  }
}
</style>

<style lang="scss">
.sidebar {
  .el-collapse-item__header {
    color: var(--text);
    padding-left: 12px;
    font-size: 15.5px;
    font-weight: 600;
    height: 35px;
    line-height: 35px;
  }
  .el-menu-item svg,
  .el-submenu svg,
  .el-submenu__title svg {
    width: 17.5px;
    margin-right: 10px;
    margin-left: 8px;
    margin-bottom: 2px;
    overflow: unset;
  }
  .bottom-bar {
    width: 100%;
    position: absolute;
    bottom: 0;
    left: 0;
    padding: 18px;
    display: flex;
    justify-content: space-between;
  }
}

@keyframes push {
  15% {
    transform: scale(0.98);
    opacity: 0.8;
  }
  100% {
    transform: scale(1);
    opacity: 1;
  }
}

@keyframes spin {
  100% {
    transform: rotate(360deg);
  }
}

@keyframes stroke-draw {
  from {
    stroke: #8a3ab9;
    stroke-dasharray: 1;
  }
  to {
    stroke: #cd486b;
    transform: rotate(180deg);
    stroke-dasharray: 8;
  }
}
</style>
