<template lang="pug">
  base-dialog(v-if="visible" @close="exit")
    div(slot="title") Команда
    div(slot="body")
      el-form(:model="form" :rules="rules" ref="form" v-loading="loading")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="name")
              el-input(v-model="form.name" size="medium" placeholder="Новая команда")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="description")
              el-input(v-model="form.description" size="medium" type="textarea" placeholder="Заметки")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item
              el-select(
                v-model="form.userIds"
                size="medium"
                placeholder="Участники"
                :remote-method="searchUsers"
                multiple filterable remote clearable default-first-option)
                el-option(v-for="item in userList" :key="item.id" :label="item.value" :value="item.id")
          el-col(v-if="!$route.params.projectId" :span="24")
            el-form-item
              el-select(
                v-model="form.projectIds"
                size="medium"
                placeholder="Проекты"
                :remote-method="searchProjects"
                multiple filterable remote clearable default-first-option)
                el-option(v-for="item in projectList" :key="item.id" :label="item.value" :value="item.id")

    div(slot="footer")
      el-button(size="medium" type="default" @click="submit") {{ isEdit ? 'Сохранить' : 'Создать' }}

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseDialog from '~/components/BaseDialog';
import dialogMixin from '~/mixins/dialog.mixin';

export default {
  components: { BaseDialog },
  props: {
    id: Number
  },
  mixins: [dialogMixin],
  data() {
    return {
      form: {
        team: {
          name: '',
          description: ''
        },
        userIds: [],
        projectIds: []
      },
      rules: {
        name: [
          {
            required: true,
            message: 'Введите название команды',
            trigger: 'blur'
          }
        ]
      }
    };
  },
  computed: {
    ...mapGetters({
      item: 'teams/getTeam',
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
        teamId: this.id,
        pageNumber: 0,
        pageSize: 10
      });
      await this.fetchTeamProjects({
        teamId: this.id,
        pageNumber: 0,
        pageSize: 10
      });
      this.form.userIds = this.teamUsers.map(user => user.id);
      this.form.projectIds = this.teamProjects.map(project => project.id);
      this.loading = false;
    }
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
