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
              el-select(v-model="form.teamMembers" size="medium" placeholder="Участники" multiple)
                el-option(v-for="item in users" :key="item.value" :label="item.label" :value="item.value")
          el-col(:span="8")
            el-form-item(prop="projectId")
              el-autocomplete(
                v-model="form.projectId"
                :fetch-suggestions="searchProjects"
                size="medium"
                placeholder="Проект")
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
        title: '',
        priority: null,
        description: '',
        teamMembers: [],
        responsible: null,
        team: null,
        scopeId: null,
        dateStart: null,
        dateEnd: null
      },
      scopes: [
        { value: 0, label: 'Виталий' },
        { value: 1, label: 'Алексей' },
        { value: 2, label: 'Андрей' }
      ],
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
  },
  computed: {
    ...mapGetters({
      team: 'teams/getTeam',
      users: 'users/getUsers',
      projects: 'projects/getProjects'
    })
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
    async searchProjects(query, callback) {
      await this.fetchProjects({
        filter: query,
        pageNumber: 1,
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
            if (this.isEdit) await this.updateTeam(payload);
            else await this.createTeam(payload);
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
