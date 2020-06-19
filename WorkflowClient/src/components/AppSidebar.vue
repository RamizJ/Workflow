<template lang="pug">
  div.sidebar
    el-menu(
      :router="true"
      :default-active="$route.path")
      el-menu-item(index="/tasks")
        i.el-icon-news
        span Задачи
      el-menu-item(index="/projects")
        i.el-icon-copy-document
        span Проекты
      el-menu-item(index="/teams")
        i.el-icon-user
        span Команды
      el-menu-item(index="/scopes" disabled)
        i.el-icon-files
        span Области
      div.divider
      el-menu-item(index="/settings")
        i.el-icon-setting
        span Настройки
      el-menu-item(index="/users")
        i.el-icon-user
        span Пользователи
      div.divider
      el-menu-item(v-for="item in favorites" :index="`/projects/${item.id}`")
        i.el-icon-notebook-2
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
      favorites: []
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
.el-menu {
  border-right: none;
  height: 100%;
  position: relative;
  background-color: var(--sidebar-background);
  padding: 24px 20px;
  transition: background-color 0.25s;
}
.el-menu:not(.el-menu--collapse) {
  width: var(--sidebar-width);
}
.el-menu-item,
.el-submenu,
.el-submenu__title {
  color: var(--sidebar-text);
  height: auto;
  line-height: 38px;
  font-size: 14px;
  font-weight: 500;
  padding: 0 8px !important;
  margin: 5px 0;
  border-radius: 6px;
  display: flex;
  align-items: center;
  transition: background-color 0.25s, color 0.25s;
  i {
    font-size: 15px;
  }
}
.el-menu-item.is-active {
  color: var(--sidebar-item-active-text);
  background-color: var(--sidebar-item-active-background);
}
.el-menu-item:hover:not(.is-active) {
  outline: none;
  background-color: var(--sidebar-item-hover-background);
}
.el-submenu i,
.el-menu-item i {
  color: var(--sidebar-text);
  transition: color 0.25s;
  margin-bottom: 1px;
}
.el-menu--collapse {
  width: var(--header-height);
}
.divider {
  height: 15px;
}
.profile {
  cursor: pointer;
  position: absolute;
  display: flex;
  align-items: center;
  bottom: 0;
  color: var(--sidebar-text);
  text-align: left;
  padding: 25px 8px;
  z-index: 2;
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
