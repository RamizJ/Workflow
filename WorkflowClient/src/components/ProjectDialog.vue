<template lang="pug">
  base-dialog(v-if="visible" @close="$emit('close')")
    div(slot="title") Проект
    div(slot="body")
      el-form(:model="form" :rules="rules" ref="form")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="name")
              el-input(v-model="form.name" size="medium" placeholder="Новый проект")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="description")
              el-input(v-model="form.description" size="medium" type="textarea" placeholder="Заметки")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="tags")
              el-select(
                v-model="form.tags"
                size="medium"
                placeholder="Теги"
                multiple filterable allow-create default-first-option)
        el-row(:gutter="20")
          el-col(:span="12")
            el-form-item(prop="ownerId")
              el-autocomplete(
                v-model="form.ownerId"
                :fetch-suggestions="searchUsers"
                size="medium"
                placeholder="Руководитель")
          el-col(:span="12")
            el-form-item(prop="teamId")
              el-autocomplete(
                v-model="form.teamId"
                :fetch-suggestions="searchTeams"
                size="medium"
                placeholder="Команда")
    div(slot="footer")
      el-button(size="medium" type="primary" @click="submit") Создать

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseDialog from '~/components/BaseDialog';

export default {
  components: { BaseDialog },
  props: {
    id: Number
  },
  data() {
    return {
      visible: false,
      loading: false,
      isEdit: !!this.id,
      form: {
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
      rules: {
        name: [
          {
            required: true,
            message: 'Введите название проекта',
            trigger: 'blur'
          }
        ],
        ownerId: [
          { required: true, message: 'Укажите руководителя', trigger: 'blur' }
        ]
      }
    };
  },
  async mounted() {
    this.visible = true;
    if (this.isEdit) {
      this.loading = true;
      await this.fetchProject(this.id);
      this.form = this.project;
      this.loading = false;
    }
  },
  computed: {
    ...mapGetters({
      project: 'projects/getProject',
      users: 'users/getUsers',
      teams: 'teams/getTeams',
      me: 'auth/me'
    })
  },
  methods: {
    ...mapActions({
      fetchProject: 'projects/fetchProject',
      createProject: 'projects/createProject',
      updateProject: 'projects/updateProject',
      fetchUsers: 'users/fetchUsers',
      fetchTeams: 'teams/fetchTeams'
    }),
    async searchUsers(query, callback) {
      await this.fetchUsers({
        filter: query,
        pageNumber: 1,
        pageSize: 10
      });
      const results = this.users.map(user => {
        return {
          value: `${user.lastName} ${user.firstName}`,
          id: user.id
        };
      });
      callback(results);
    },
    async searchTeams(query, callback) {
      await this.fetchTeams({
        filter: query,
        pageNumber: 1,
        pageSize: 10
      });
      const results = this.teams.map(team => {
        return {
          value: team.name,
          id: team.id
        };
      });
      callback(results);
    },
    submit() {
      const payload = { ...this.form };
      const form = this.$refs.form;
      form.validate(async valid => {
        if (valid) {
          try {
            if (this.isEdit) await this.updateProject(payload);
            else await this.createProject(payload);
            form.resetFields();
            this.$emit('close');
          } catch (e) {
            this.$message.error('Ошибка отправки запроса');
          }
        } else {
          this.$message.error('Укажите корректные данные');
        }
      });
    }
  }
};
</script>
