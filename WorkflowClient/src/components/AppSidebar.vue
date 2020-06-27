<template lang="pug">
  div.sidebar
    el-menu(
      :router="true"
      :default-active="$route.path")
      //el-menu-item(index="/" disabled)
        feather(type="activity")
        span Обзор
      el-menu-item(index="/tasks")
        feather(type="list")
        span Задачи
      el-menu-item(index="/projects")
        feather(type="layers")
        span Проекты
      el-menu-item(index="/teams")
        feather(type="user")
        span Команды
      //el-menu-item(index="/scopes" disabled)
        feather(type="hexagon")
        span Области

      div.divider

      el-menu-item(index="/settings")
        feather(type="settings")
        span Настройки
      el-menu-item(index="/users")
        feather(type="users")
        span Пользователи
      //el-menu-item(index="/journal" disabled)
        feather(type="check-circle")
        span Журнал
      //el-menu-item(index="/trash" disabled)
        feather(type="trash")
        span Корзина

      div.divider

      el-collapse(v-model="collapseState")
        el-collapse-item(title="Проекты" name="projects")
          el-menu-item(v-for="item in favorites" :index="`/projects/${item.id}`")
            feather(type="box")
            span {{ item.name }}

    div.profile(v-if="!!me" @click="$router.push({ name: 'Profile' })")
      el-avatar(:size="36" icon="el-icon-user-solid")
      div
        div.profile__title {{ `${me.lastName} ${me.firstName}` }}
        div.profile__subtitle {{ me.position || 'Разработчик' }}
</template>

<script>
import { mapActions, mapGetters } from 'vuex';

export default {
  name: 'AppSidebar',
  data() {
    return {
      favorites: [],
      collapseState: ['projects']
    };
  },
  computed: {
    ...mapGetters({
      me: 'auth/me',
      projects: 'projects/getProjects'
    })
  },
  async mounted() {
    await this.fetchProjects({
      pageNumber: 0,
      pageSize: 5
    });
    this.favorites = { ...this.projects };
  },
  methods: {
    ...mapActions({ fetchProjects: 'projects/fetchProjects' })
  }
};
</script>

<style lang="scss" scoped>
.sidebar {
  height: 100%;
  border-right: 1px solid var(--sidebar-item-hover-background);
  background-color: var(--sidebar-background);
  transition: background-color 0.5s, border-color 0.3s;
}
.el-menu {
  overflow: auto;
  border-right: none;
  height: 100%;
  position: relative;
  background-color: transparent;
  padding: 24px 15px;
}
.el-menu:not(.el-menu--collapse) {
  width: var(--sidebar-width);
}
.el-menu-item,
.el-submenu,
.el-submenu__title {
  color: var(--sidebar-text);
  height: auto;
  line-height: 40px;
  font-size: 14px;
  font-weight: 400;
  padding: 0 8px !important;
  margin: 4px 0;
  border-radius: 10px;
  display: flex;
  align-items: center;
  transition: background-color 0.25s, color 0.25s;
  i {
    height: 14px;
    margin-right: 12px;
    margin-left: 8px;
  }
  &:focus {
    animation: push 0.6s;
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
  margin-bottom: 1px;
}
.el-menu--collapse {
  width: var(--header-height);
}
.divider {
  height: 15px;
}

.profile {
  width: var(--sidebar-width);
  cursor: pointer;
  position: absolute;
  display: flex;
  align-items: center;
  bottom: 0;
  color: var(--sidebar-text);
  background: var(--sidebar-background);
  text-align: left;
  padding: 20px 25px;
  z-index: 2;
  transition: background 0.5s;
  .el-avatar {
    margin-right: 12px;
  }
  .profile__title {
    font-size: 14px;
    font-weight: 600;
    margin-bottom: 5px;
  }
  .profile__subtitle {
    font-size: 11px;
    font-weight: 500;
    opacity: 0.9;
    width: fit-content;
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
</style>
