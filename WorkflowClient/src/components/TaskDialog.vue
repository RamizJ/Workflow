<template lang="pug">
  base-dialog(v-if="visible" @close="$emit('close')")
    div(slot="title") Задача
    div(slot="body")
      el-form(:model="form" :rules="rules" ref="form" v-loading="loading")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="title")
              el-input(v-model="form.title" size="medium" placeholder="Новая задача")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="description")
              el-input(v-model="form.description" size="medium" type="textarea" placeholder="Заметки")
        el-row(:gutter="20")
          el-col(:span="16")
            el-form-item(prop="tags")
              el-select(
                v-model="form.tags"
                size="medium"
                placeholder="Теги"
                multiple filterable allow-create default-first-option)
          el-col(:span="8")
            el-form-item(prop="priority")
              el-select(
                v-model="form.priority"
                size="medium"
                placeholder="Приоритет")
                el-option(v-for="item in priorities" :key="item.value" :label="item.label" :value="item.value")
        el-row(:gutter="20")
          el-col(:span="8")
            el-form-item(prop="performerId")
              el-select(v-model="form.performerId" size="medium" placeholder="Ответственный" filterable)
                el-option(v-for="item in userList" :key="item.id" :label="item.value" :value="item.id")

          el-col(:span="8")
            el-form-item(prop="projectId")
              el-select(v-model="form.projectId" size="medium" placeholder="Проект" filterable)
                el-option(v-for="item in projectList" :key="item.id" :label="item.value" :value="item.id")

          el-col(:span="8")
            el-form-item(prop="dateEnd")
              el-date-picker(
                v-model="form.dateEnd"
                size="medium"
                prefix-icon="el-icon-arrow-down"
                suffix-icon="el-icon-arrow-down"
                placeholder="Крайний срок")
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
      loading: false,
      isEdit: !!this.id,
      form: {
        title: '',
        description: '',
        tags: [],
        observers: [],
        priority: null,
        ownerId: null,
        performerId: null,
        projectId: null,
        creationDate: new Date(),
        dateEnd: null
      },
      priorities: [
        { value: 'High', label: 'Высокий' },
        { value: 'Normal', label: 'Обычный' },
        { value: 'Low', label: 'Низкий' }
      ],
      rules: {
        title: [
          {
            required: true,
            message: 'Введите название задачи',
            trigger: 'blur'
          }
        ],
        performerId: [
          { required: true, message: 'Укажите ответственного', trigger: 'blur' }
        ],
        projectId: [
          { required: true, message: 'Укажите проект', trigger: 'blur' }
        ]
      }
    };
  },
  async mounted() {
    this.visible = true;
    if (this.isEdit) {
      this.loading = true;
      await this.fetchTask(this.id);
      this.form = this.task;
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
      task: 'tasks/getTask',
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
      fetchTask: 'tasks/fetchTask',
      createTask: 'tasks/createTask',
      updateTask: 'tasks/updateTask',
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
            if (this.isEdit) await this.updateTask(payload);
            else await this.createTask(payload);
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
