<template lang="pug">
  base-dialog(v-if="visible" @close="exit" ref="dialog")
    h1(slot="title") Добавить команду
    el-form(slot="body" :model="form" ref="form" v-loading="loading" @submit.native.prevent="submit")
      el-row(:gutter="20")
        el-col(:span="24")
          el-form-item
            el-select(
              ref="input"
              v-model="form.teamId"
              placeholder="Найти команду..."
              :remote-method="searchTeams"
              filterable remote clearable default-first-option)
              el-option(v-for="item in teamsToAdd" :key="item.id" :label="item.value" :value="item.id")
    template(slot="footer")
      div.extra
      div.send
        el-tooltip(content="Сохранить" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(type="text" @click="submit" circle)
            feather(type="arrow-right")
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseDialog from '@/components/BaseDialog';
import dialogMixin from '@/mixins/dialog.mixin';

export default {
  components: { BaseDialog },
  mixins: [dialogMixin],
  data() {
    return {
      form: {
        teamId: ''
      }
    };
  },
  computed: {
    ...mapGetters({
      projectTeams: 'projects/getProjectTeams',
      teamProjects: 'teams/getTeamProjects'
    }),
    teamsToAdd() {
      const allTeams = this.teamList;
      const existingTeams = this.existingTeams;
      return allTeams.filter(team => {
        return !existingTeams.find(existingTeam => existingTeam.id === team.id);
      });
    },
    existingTeams() {
      return this.projectTeams.map(team => {
        return {
          value: team.name,
          id: team.id
        };
      });
    }
  },
  async mounted() {
    await this.searchTeams();
    await this.fetchProjectTeams({
      projectId: this.$route.params.projectId,
      pageNumber: 0,
      pageSize: 100
    });
    this.$refs.input.focus();
  },
  methods: {
    ...mapActions({
      addTeam: 'projects/addTeam',
      fetchItem: 'teams/findOneById',
      createItem: 'teams/createOne',
      updateItem: 'teams/updateOne',
      fetchProjectTeams: 'projects/findTeams',
      fetchTeamProjects: 'teams/findProjects'
    }),
    async submit() {
      const projectId = this.$route.params.projectId;
      const teamId = this.form.teamId;
      if (teamId) {
        this.loading = true;
        await this.addTeam({ projectId, teamId });
        this.$emit('submit');
        this.loading = false;
      }
      this.exit();
    }
  }
};
</script>
