<template lang="pug">
  page(v-loading="loading")
    base-header
      template(slot="title")
        input.title(
          v-model="projectItem.name"
          v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
          @change="onProjectUpdate")
      template(slot="action")
        el-dropdown(placement="bottom" :show-timeout="0")
          el-button(type="text" size="mini")
            feather(type="chevron-down" size="22")
          el-dropdown-menu(slot="dropdown")
            el-dropdown-item
              el-button(type="text" size="mini" @click="onTaskCreate") Создать задачу
            el-dropdown-item
              el-button(type="text" size="mini" @click="onTeamCreate") Создать команду
            el-divider
            el-dropdown-item
              el-button(type="text" size="mini" @click="onProjectEdit") Редактировать
            el-dropdown-item
              el-button(type="text" size="mini" @click="onProjectDelete") Переместить в корзину
      //template(slot="subtitle")
        input.subtitle(
          v-model="projectItem.description"
          v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
          @change="onProjectUpdate")
      //template(slot="search")
        base-search(:query.sync="searchQuery")

    el-tabs(v-if="projectItem.id" ref="tabs" v-model="activeTab" @tab-click="setTab")
      el-tab-pane(name="overview" label="Обзор")
        project-overview(v-if="activeTab === 'overview'" :data="projectItem")
      el-tab-pane(name="tasks" label="Задачи")
        project-tasks(v-if="activeTab === 'tasks'")
      el-tab-pane(name="team" label="Команда")
        project-team(v-if="activeTab === 'team'")

    dialog-project(v-if="dialogProjectVisible" :data="projectItem" @close="dialogProjectVisible = false")
    dialog-task(v-if="dialogTaskVisible" @close="dialogTaskVisible = false")
    dialog-team(v-if="dialogTeamVisible" @close="dialogTeamVisible = false")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import Page from '~/components/Page';
import BaseHeader from '~/components/BaseHeader';
import BaseSearch from '~/components/BaseSearch';
import DialogProject from '~/components/ProjectDialog';
import DialogTask from '~/components/TaskDialog';
import DialogTeam from '~/components/TeamDialog';
import ProjectOverview from '~/components/ProjectOverview';
import ProjectTasks from '~/components/ProjectTasks';
import ProjectTeam from '~/components/ProjectTeam';

export default {
  name: 'Project',
  components: {
    Page,
    BaseHeader,
    BaseSearch,
    ProjectOverview,
    ProjectTeam,
    ProjectTasks,
    DialogProject,
    DialogTask,
    DialogTeam
  },
  data() {
    return {
      loading: true,
      searchQuery: '',
      activeTab: 'overview',
      tabs: [
        { value: 'overview', label: 'Общее' },
        { value: 'tasks', label: 'Задачи' },
        { value: 'team', label: 'Команда' },
        { value: 'history', label: 'История' }
      ],
      projectItem: {
        name: '',
        description: '',
        tags: [],
        ownerId: null,
        ownerFio: null,
        teamId: null,
        teamName: null,
        groupId: null,
        groupName: null
      },
      dialogProjectVisible: false,
      dialogTaskVisible: false,
      dialogTeamVisible: false
    };
  },
  computed: {
    ...mapGetters({
      project: 'projects/getProject'
    })
  },
  async mounted() {
    this.loading = true;
    this.loadTab();
    await this.fetchProject(this.$route.params.projectId);
    this.projectItem = { ...this.project };
    this.loading = false;
  },
  methods: {
    ...mapActions({
      fetchProject: 'projects/fetchProject',
      updateProject: 'projects/updateProject',
      deleteProject: 'projects/deleteProject',
      fetchSidebarProjects: 'projects/fetchSidebarProjects'
    }),
    loadTab() {
      const query = { ...this.$route.query };
      query.tab = query.tab || this.activeTab;
      this.activeTab = query.tab;
      if (JSON.stringify(query) !== JSON.stringify(this.$route.query))
        this.$router.push({ query });
    },
    setTab() {
      const query = { ...this.$route.query };
      query.tab = this.activeTab;
      if (JSON.stringify(query) !== JSON.stringify(this.$route.query))
        this.$router.push({ query });
    },
    async onTaskCreate(e) {
      this.dialogTaskVisible = true;
    },
    async onTeamCreate(e) {
      this.dialogTeamVisible = true;
    },
    async onProjectEdit(e) {
      this.dialogProjectVisible = true;
    },
    async onProjectUpdate(e) {
      await this.updateProject(this.projectItem);
    },
    async onProjectDelete() {
      await this.deleteProject(this.projectItem.id);
      await this.fetchSidebarProjects({ reload: true });
      await this.$router.push({ name: 'Projects' });
    }
  }
};
</script>

<style lang="scss" scoped></style>
