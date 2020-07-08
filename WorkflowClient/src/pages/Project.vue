<template lang="pug">
  page
    page-header
      template(slot="title")
        input.title(
          placeholder="Заголовок"
          v-model="projectItem.name"
          v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
          @change="onUpdate")
      template(slot="action")
        el-button(type="text" size="mini" @click="onDelete") Удалить
        el-button(type="text" size="mini" @click="openDialog") Добавить задачу
        el-button(type="text" size="mini" @click="teamDialogVisible = true") Добавить команду
      template(slot="search")
        base-search(:query.sync="searchQuery")
      template(slot="subtitle")
        input.subtitle(
          placeholder="Описание"
          v-model="projectItem.description"
          v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
          @change="onUpdate")

    el-tabs(ref="tabs" v-model="activeTab" @tab-click="onTabClick")
      el-tab-pane(v-for="(tab, index) in tabs" :key="index" :label="tab.label" :name="tab.value")
        table-tasks(v-if="tab.value === activeTab" ref="tasks" :search="searchQuery" :status="tab.value")

    team-dialog(v-if="teamDialogVisible" @close="teamDialogVisible = false")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import Page from '~/components/Page';
import PageHeader from '~/components/PageHeader';
import TableContent from '~/components/TableContent';
import TeamDialog from '~/components/DialogTeam';
import TableTasks from '~/components/TableTasks';
import BaseSearch from '~/components/BaseSearch';

export default {
  name: 'Project',
  components: {
    BaseSearch,
    Page,
    PageHeader,
    TableContent,
    TeamDialog,
    TableTasks
  },
  data() {
    return {
      searchQuery: '',
      activeTab: 'All',
      tabs: [
        { value: 'All', label: 'Все' },
        { value: 'New', label: 'Новые' },
        { value: 'Perform', label: 'Выполняются' },
        { value: 'Testing', label: 'Проверяются' },
        { value: 'Delay', label: 'Отложенные' },
        { value: 'Succeed', label: 'Выполненные' },
        { value: 'Rejected', label: 'Отклоненные' },
        { value: 'Deleted', label: 'Удаленные' }
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
        groupName: null,
        creationDate: new Date()
      },
      teamDialogVisible: false
    };
  },
  computed: {
    ...mapGetters({
      project: 'projects/getProject'
    })
  },
  async mounted() {
    const query = { ...this.$route.query };
    query.status = query.status || this.activeTab;
    this.activeTab = query.status;
    if (JSON.stringify(query) !== JSON.stringify(this.$route.query))
      await this.$router.push({ query });

    await this.fetchProject(this.$route.params.projectId);
    this.projectItem = { ...this.project };
  },
  methods: {
    ...mapActions({
      fetchProject: 'projects/fetchProject',
      updateProject: 'projects/updateProject',
      deleteProject: 'projects/deleteProject',
      fetchSidebarProjects: 'projects/fetchSidebarProjects'
    }),
    openDialog() {
      this.$refs.tasks[0].dialogVisible = true;
    },
    onTabClick(tab) {
      const query = { ...this.$route.query };
      query.status = this.activeTab;
      this.$router.push({ query });
    },
    async onUpdate(e) {
      await this.updateProject(this.projectItem);
    },
    async onDelete() {
      await this.deleteProject(this.projectItem.id);
      await this.fetchSidebarProjects({ reload: true });
      await this.$router.push({ name: 'Projects' });
    }
  }
};
</script>

<style lang="scss" scoped></style>
