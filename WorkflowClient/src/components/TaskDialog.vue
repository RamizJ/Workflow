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
              el-select(
                v-model="form.performerId"
                size="medium"
                placeholder="Ответственный"
                :remote-method="searchUsers"
                filterable remote clearable default-first-option)
                el-option(v-for="item in userList" :key="item.id" :label="item.value" :value="item.id")
          el-col(:span="8")
            el-form-item(prop="projectId")
              el-select(
                v-model="form.projectId"
                size="medium"
                placeholder="Проект"
                :remote-method="searchProjects"
                filterable remote clearable default-first-option)
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
import dialogMixin from '~/mixins/dialog.mixin';

export default {
  components: { BaseDialog },
  props: {
    id: String
  },
  mixins: [dialogMixin],
  data() {
    return {
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
      },
      priorities: [
        { value: 'High', label: 'Высокий' },
        { value: 'Normal', label: 'Обычный' },
        { value: 'Low', label: 'Низкий' }
      ]
    };
  },
  computed: {
    ...mapGetters({
      item: 'tasks/getTask'
    })
  },
  methods: {
    ...mapActions({
      fetchItem: 'tasks/fetchTask',
      createItem: 'tasks/createTask',
      updateItem: 'tasks/updateTask'
    })
  }
};
</script>
