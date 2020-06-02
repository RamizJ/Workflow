<template lang="pug">
  el-dialog(:visible.sync="visible" :before-close="close" v-loading="loading")
    div.header(slot="title")
      div.title Проект
    div.body
      el-form(:model="form" ref="form")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item
              el-input(v-model="form.name" placeholder="Новый проект")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item
              el-input(v-model="form.description" type="textarea" placeholder="Заметки")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item
              el-select(
                v-model="form.tags"
                placeholder="Теги"
                multiple filterable allow-create default-first-option)
        el-row(:gutter="20")
          el-col(:span="8")
            el-form-item
              el-select(v-model="form.ownerId" placeholder="Руководитель")
                el-option(v-for="item in users" :key="item.id" :label="item.name" :value="item.name")
          el-col(:span="8")
            el-form-item
              el-select(v-model="form.teamId" placeholder="Команда")
                el-option(v-for="item in teams" :key="item.id" :label="item.name" :value="item.name")
          el-col(:span="8")
            el-form-item
              el-select(v-model="form.groupId" placeholder="Область")
                el-option(v-for="item in scopes" :key="item.id" :label="item.name" :value="item.name")
      div.footer
        el-button(size="medium" @click="close") Закрыть
        el-button(size="medium" type="primary" @click="submit") Создать
</template>

<script>
  import { mapActions, mapGetters } from 'vuex';

export default {
  components: {  },
  props: {
    id: Number,
  },
  data() {
    return {
      visible: false,
      loading: false,
      isEdit: !!this.id,
      form: {
        name: "",
        description: "",
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
        { id: 0, name: "Виталий" },
        { id: 1, name: "Алексей" },
        { id: 2, name: "Андрей" }
      ],
      teams: [
        { id: 0, name: "Виталий" },
        { id: 1, name: "Алексей" },
        { id: 2, name: "Андрей" }
      ],
      scopes: [
        { id: 0, name: "Виталий" },
        { id: 1, name: "Алексей" },
        { id: 2, name: "Андрей" }
      ],
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
    close() {
      this.visible = false;
      setTimeout(() => this.$emit('close'), 300);
    },
    submit() {
      const payload = { ...this.form };
      const form = this.$refs.form;
      form.validate(async (valid) => {
        if (valid) {
          try {
            if (this.isEdit)
              await this.updateProject(payload);
            else
              await this.createProject(payload);
            form.resetFields();
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

<style lang="scss" scoped>
.header,
.body {
  padding: 14px 18px;
}
.header {
  padding-bottom: 0;
}
.body {
  padding-top: 0;
}
.title {
  font-size: 24px;
  font-weight: 700;
}
.footer {
  margin-top: 10px;
  display: flex;
  justify-content: flex-end;
}
.el-select {
  width: 100%;
}
.el-range-editor.el-input__inner {
  width: 100%;
}

</style>
