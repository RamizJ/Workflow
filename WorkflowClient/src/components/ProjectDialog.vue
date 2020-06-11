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
          el-col(:span="8")
            el-form-item(prop="ownerId")
              el-select(v-model="form.ownerId" size="medium" placeholder="Руководитель")
                el-option(v-for="item in users" :key="item.id" :label="item.name" :value="item.id")
          el-col(:span="8")
            el-form-item(prop="teamId")
              el-select(v-model="form.teamId" size="medium" placeholder="Команда")
                el-option(v-for="item in teams" :key="item.id" :label="item.name" :value="item.id")
          el-col(:span="8")
            el-form-item(prop="groupId")
              el-select(v-model="form.groupId" size="medium" placeholder="Область")
                el-option(v-for="item in scopes" :key="item.id" :label="item.name" :value="item.id")
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
      users: [
        { id: 0, name: 'Виталий' },
        { id: 1, name: 'Алексей' },
        { id: 2, name: 'Андрей' }
      ],
      teams: [
        { id: 0, name: 'Виталий' },
        { id: 1, name: 'Алексей' },
        { id: 2, name: 'Андрей' }
      ],
      scopes: [
        { id: 0, name: 'Виталий' },
        { id: 1, name: 'Алексей' },
        { id: 2, name: 'Андрей' }
      ],
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
        ],
        groupId: [
          { required: true, message: 'Укажите область', trigger: 'blur' }
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
    ...mapGetters({ project: 'projects/getProject' })
  },
  methods: {
    ...mapActions({
      fetchProject: 'projects/fetchProject',
      createProject: 'projects/createProject',
      updateProject: 'projects/updateProject'
    }),
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
