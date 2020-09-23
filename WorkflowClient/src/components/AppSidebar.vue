<template>
  <div class="sidebar">
    <el-menu :router="true" :default-active="$route.path">
      <!--div.profile(v-if="me")
    router-link(to="/profile")
      div.avatar
        el-avatar(:size="40" icon="el-icon-user-solid")
        svg(viewBox="0 0 100 100" xmlns="http://www.w3.org/2000/svg" style="enable-background:new -580 439 577.9 194;" xml:space="preserve")
          circle(cx="50" cy="50" r="40")
      div
        div.profile__title {{ me.firstName }}
        div.profile__subtitle {{ me.email }}
    div.actions
      el-button(type="text" @click="exit")
        feather(type="log-out")
    -->
      <!--el-menu-item(index="/" disabled)
    feather(type="activity")
    span Обзор
    -->
      <el-menu-item index="/tasks">
        <feather type="list"></feather><span>Задачи</span>
      </el-menu-item>
      <el-menu-item index="/projects">
        <feather type="copy"></feather><span>Проекты</span>
      </el-menu-item>
      <el-menu-item index="/teams">
        <feather type="users"></feather><span>Команды</span>
      </el-menu-item>
      <el-menu-item index="/users">
        <feather type="user"></feather><span>Пользователи</span>
      </el-menu-item>

      <div class="divider"></div>

      <el-menu-item index="/settings">
        <feather type="settings"></feather><span>Настройки</span>
      </el-menu-item>
    </el-menu>
    <div class="logo"><img src="@/assets/logo.svg" /></div>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'

@Component
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
  padding: 15px 10px;
  //transition: border-color 0.25s;
}
.el-menu:not(.el-menu--collapse) {
  //width: var(--sidebar-width);
}
.el-menu-item,
.el-submenu,
.el-submenu__title {
  color: var(--sidebar-text);
  height: auto;
  line-height: 38px;
  font-size: 13.5px;
  font-weight: 400;
  letter-spacing: 0.1px;
  padding: 0 8px !important;
  margin: 4px 0;
  border-radius: 6px;
  display: flex;
  align-items: center;
  z-index: 1;
  transition: background-color 0.25s, color 0.25s;
  i {
    width: 13px;
    margin-right: 10px;
    margin-left: 6px;
    overflow: unset;
  }
  &:focus {
    //animation: push 0.5s;
  }
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
  i {
    color: white;
  }
}
.el-menu-item:hover:not(.is-active) {
  transition: background-color 0.05s;
  outline: none;
  background-color: var(--sidebar-item-hover-background);
}
.el-submenu i,
.el-menu-item i {
  color: var(--sidebar-text);
  opacity: 0.85;
  //margin-bottom: 0.5px;
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
.profile {
  display: flex;
  align-items: center;
  justify-content: space-between;
  color: var(--sidebar-text);
  background: var(--sidebar-background);
  text-align: left;
  padding: 0 5px 8px 0;
  z-index: 2;
  transition: background 0.25s;
  a {
    display: flex;
    color: var(--text);
    align-items: center;
    &:focus {
      animation: push 0.6s;
    }
    .avatar {
      position: relative;
      width: 56px;
      height: 56px;
      margin-right: 5px;
      &:hover > svg {
        //opacity: 1;
      }
      svg {
        opacity: 0;
        transition: opacity 0.6s;
        fill: none;
        stroke: var(--color-primary);
        stroke-linecap: round;
        stroke-width: 3;
        stroke-dasharray: 1;
        stroke-dashoffset: 0;
        animation: stroke-draw 2s ease-out infinite alternate;
      }
      img,
      .el-avatar {
        position: absolute;
        left: 50%;
        top: 50%;
        transform: translate(-50%, -50%);
        width: 36px;
        border-radius: 50%;
      }
    }
    .profile__title {
      letter-spacing: 0.2px;
      font-size: 14px;
      font-weight: 500;
      margin-bottom: 6px;
    }
    .profile__subtitle {
      letter-spacing: 0.2px;
      font-size: 11px;
      font-weight: 400;
      width: fit-content;
      color: var(--text-placeholder);
    }
  }
  .actions {
    opacity: 0;
    transform: scale(0.95);
    margin-right: 5px;
    transition: 0.3s;
    .el-button:not(:last-child) {
      margin-right: 6px;
    }
    i {
      height: 16px;
      color: var(--text);
      opacity: 0.6;
    }
  }
  &:hover .actions {
    opacity: 1;
    transform: scale(1);
    margin-right: 0;
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
}

@keyframes push {
  15% {
    transform: scale(0.97);
    opacity: 0.85;
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
