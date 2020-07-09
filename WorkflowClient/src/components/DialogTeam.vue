<template lang="pug">
  base-dialog(v-if="visible" @close="exit" ref="dialog")
    h1(slot="title") Команда
    el-form(slot="body" :model="form" :rules="rules" ref="form" v-loading="loading" @submit.native.prevent="submit")
      el-row(:gutter="20")
        el-col(:span="24")
          el-form-item(prop="name")
            el-input(ref="title" v-model="form.name" placeholder="Новая команда")
      el-row(:gutter="20")
        el-col(v-if="descriptionVisible || form.description" :span="24")
          el-form-item(prop="description")
            el-input(v-model="form.description" :autosize="{ minRows: 2 }" type="textarea" placeholder="Описание")
      el-row(:gutter="20")
        transition(name="fade")
          el-col(v-if="teamMembersVisible || (form.userIds && form.userIds.length)" :span="24")
            el-form-item
              el-select(
                v-model="form.userIds"
                placeholder="Участники"
                :remote-method="searchUsers"
                multiple filterable remote clearable default-first-option)
                el-option(v-for="item in userList" :key="item.id" :label="item.value" :value="item.id")
        transition(name="fade")
          el-col(v-if="!$route.params.projectId && (projectsVisible || (form.projectIds && form.projectIds.length))" :span="24")
            el-form-item
              el-select(
                v-model="form.projectIds"
                placeholder="Проекты"
                :remote-method="searchProjects"
                multiple filterable remote clearable default-first-option)
                el-option(v-for="item in projectList" :key="item.id" :label="item.value" :value="item.id")
    template(slot="footer")
      div.extra
        el-tooltip(content="Описание" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(v-if="!form.description" type="text" title="Теги" @click="descriptionVisible = !descriptionVisible" circle)
            feather(type="align-left")
        el-tooltip(content="Участники" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(size="small" v-if="!(form.userIds && form.userIds.length)" type="text" title="Теги" @click="teamMembersVisible = !teamMembersVisible" circle)
            feather(type="users")
        el-tooltip(content="Проекты" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(size="small" v-if="!(form.projectIds && form.projectIds.length)" type="text" title="Теги" @click="projectsVisible = !projectsVisible" circle)
            feather(type="layers")
      div.send
        el-tooltip(content="Сохранить" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(type="text" @click="submit" circle)
            feather(type="arrow-right")
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseDialog from '~/components/Dialog';
import dialogMixin from '~/mixins/dialog.mixin';

export default {
  components: { BaseDialog },
  mixins: [dialogMixin],
  data() {
    return {
      form: {
        name: '',
        description: '',
        userIds: [],
        projectIds: []
      },
      rules: {
        name: [{ required: true, message: '!', trigger: 'blur' }]
      },
      descriptionVisible: null,
      teamMembersVisible: null,
      projectsVisible: null
    };
  },
  computed: {
    ...mapGetters({
      teamUsers: 'teams/getTeamUsers',
      teamProjects: 'teams/getTeamProjects'
    })
  },
  async mounted() {
    if (this.$route.params.projectId)
      this.form.projectIds.push(parseInt(this.$route.params.projectId));

    await this.searchUsers();
    await this.searchProjects();

    if (this.isEdit) {
      this.loading = true;
      await this.fetchTeamUsers({
        teamId: this.form.id,
        pageNumber: 0,
        pageSize: 10
      });
      await this.fetchTeamProjects({
        teamId: this.form.id,
        pageNumber: 0,
        pageSize: 10
      });
      for (let user of this.teamUsers) {
        this.form.userIds.push(user.id);
      }
      for (let project of this.teamProjects) {
        this.form.projectIds.push(project.id);
      }
      this.loading = false;
    }
    this.$refs.title.focus();
  },
  methods: {
    ...mapActions({
      fetchItem: 'teams/fetchTeam',
      createItem: 'teams/createTeam',
      updateItem: 'teams/updateTeam',
      fetchTeamUsers: 'teams/fetchTeamUsers',
      fetchTeamProjects: 'teams/fetchTeamProjects'
    })
  }
};
</script>
