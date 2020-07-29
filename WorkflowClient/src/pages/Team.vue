<template lang="pug">
  page(v-loading="loading")
    base-header
      template(slot="title")
        input.title(
          v-model="teamItem.name"
          v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
          @change="onTeamUpdate")
      template(slot="action")
        el-dropdown(placement="bottom" :show-timeout="0")
          el-button(type="text" size="mini")
            feather(type="chevron-down" size="22")
          el-dropdown-menu(slot="dropdown")
            el-dropdown-item
              el-button(type="text" size="mini" @click="onUserCreate") Добавить участника
            el-divider
            el-dropdown-item
              el-button(type="text" size="mini" @click="onTeamEdit") Редактировать
            el-dropdown-item
              el-button(type="text" size="mini" @click="onTeamDelete") Переместить в корзину

    el-tabs(v-if="teamItem.id" ref="tabs" v-model="activeTab" @tab-click="setTab")
      el-tab-pane(name="members" label="Участники")
        team-users(v-if="activeTab === 'members'")
      el-tab-pane(name="projects" label="Проекты")

    team-dialog(v-if="dialogTeamVisible" :data="teamItem" @close="dialogTeamVisible = false")
    user-dialog(v-if="dialogUserVisible" @close="dialogUserVisible = false")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import Page from '@/components/Page';
import BaseHeader from '@/components/BaseHeader';
import UserDialog from '@/components/Users/UserDialog';
import TeamUsers from '@/components/Teams/TeamUsers';

export default {
  name: 'Team',
  components: {
    TeamUsers,
    UserDialog,
    Page,
    BaseHeader
  },
  data() {
    return {
      loading: true,
      searchQuery: '',
      teamItem: {
        name: '',
        description: '',
        userIds: [],
        projectIds: []
      },
      activeTab: 'members',
      tabs: [
        { value: 'members', label: 'Участники' },
        { value: 'projects', label: 'Проекты' }
      ],
      dialogTeamVisible: false,
      dialogUserVisible: false
    };
  },
  computed: {
    ...mapGetters({
      team: 'teams/getTeam'
    })
  },
  async mounted() {
    this.loading = true;
    this.loadTab();
    await this.fetchTeam(this.$route.params.teamId);
    this.teamItem = { ...this.team };
    this.loading = false;
  },
  methods: {
    ...mapActions({
      fetchTeam: 'teams/fetchTeam',
      updateTeam: 'teams/updateTeam',
      deleteTeam: 'teams/deleteTeam'
    }),
    loadTab() {
      const query = { ...this.$route.query };
      query.tab = query.tab || this.activeTab;
      this.activeTab = query.tab;
      if (JSON.stringify(query) !== JSON.stringify(this.$route.query))
        this.$router.push({ query });
    },
    setTab() {
      const query = {};
      query.tab = this.activeTab;
      if (JSON.stringify(query) !== JSON.stringify(this.$route.query))
        this.$router.push({ query });
    },
    async onUserCreate(e) {
      if (this.$refs.projectTasks)
        this.$refs.projectTasks.$refs.items.onItemCreate();
      else this.dialogTaskVisible = true;
    },
    async onTeamEdit(e) {
      this.dialogTeamVisible = true;
    },
    async onTeamUpdate(e) {
      await this.updateTeam(this.teamItem);
    },
    async onTeamDelete() {
      await this.deleteTeam(this.teamItem.id);
      await this.$router.push({ name: 'Teams' });
    }
  }
};
</script>

<style lang="scss" scoped></style>
