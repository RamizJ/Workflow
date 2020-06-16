<template lang="pug">
  base-dialog(v-if="visible" @close="$emit('close')")
    div(slot="title") Команда
    div(slot="body")
      el-form(:model="form" :rules="rules" ref="form")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="name")
              el-input(v-model="form.name" size="medium" placeholder="Новая команда")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="description")
              el-input(v-model="form.description" size="medium" type="textarea" placeholder="Заметки")
        el-row(:gutter="20")
          el-col(:span="16")
            el-form-item(prop="teamMembers")
              el-select(v-model="form.teamMembers" size="medium" placeholder="Участники" multiple filterable)
                el-option(v-for="item in userList" :key="item.id" :label="item.value" :value="item.id")
          el-col(:span="8")
            el-form-item(prop="projectId")
              el-select(v-model="form.projectId" size="medium" placeholder="Проект" filterable)
                el-option(v-for="item in projectList" :key="item.id" :label="item.value" :value="item.id")

    div(slot="footer")
      el-button(size="medium" type="primary" @click="submit") Создать

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseDialog from '~/components/BaseDialog';

export default {
  components: { BaseDialog },
  props: {
    id: String
  },
  data() {
    return {
      visible: false,
      loading: true,
      isEdit: !!this.id,
      form: {
        name: '',
        description: '',
        teamMembers: [],
        projectId: null,
        priority: null
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
  async mounted() {
    this.visible = true;
    if (this.isEdit) {
      this.loading = true;
      await this.fetchTeam(this.id);
      this.form = this.team;
      this.loading = false;
    }
    await this.fetchUsers({
      pageNumber: 0,
      pageSize: 10
    });
    await this.fetchProjects({
      pageNumber: 0,
      pageSize: 10
    });
  },
  computed: {
    ...mapGetters({
      team: 'teams/getTeam',
      users: 'users/getUsers',
      projects: 'projects/getProjects'
    }),
    userList() {
      return this.users.map(user => {
        return {
          value: `${user.lastName} ${user.firstName}`,
          id: user.id
        };
      });
    },
    projectList() {
      return this.projects.map(project => {
        return {
          value: project.name,
          id: project.id
        };
      });
    }
  },
  methods: {
    ...mapActions({
      fetchTeam: 'teams/fetchTeam',
      createTeam: 'teams/createTeam',
      updateTeam: 'teams/updateTeam',
      fetchUsers: 'users/fetchUsers',
      fetchProjects: 'projects/fetchProjects'
    }),
    async searchUsers(query, callback) {
      await this.fetchUsers({
        filter: query,
        pageNumber: 0,
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
    async searchProjects(query, callback) {
      await this.fetchProjects({
        filter: query,
        pageNumber: 0,
        pageSize: 10
      });
      const results = this.projects.map(project => {
        return {
          value: project.name,
          id: project.id
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
            const projectId = payload.projectId;
            if (this.isEdit) await this.updateTeam(payload);
            else await this.createTeam({ payload, projectId });
            form.resetFields();
            this.$emit('close');
          } catch (e) {
            this.$message.error('Ошибка отправки запроса');
            console.error(e);
          }
        } else {
          this.$message.error('Укажите корректные данные');
        }
      });
    }
  }
};
</script>
