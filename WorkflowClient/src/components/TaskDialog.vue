<template lang="pug">
  el-dialog(:visible.sync="visible" :before-close="close" v-loading="loading")
    div.header(slot="title")
      div.title Задача
    div.body
      el-form(:model="form" ref="form")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item
              el-input(v-model="form.title" size="medium" placeholder="Новая задача")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item
              el-input(v-model="form.description" size="medium" type="textarea" placeholder="Заметки")
        el-row(:gutter="20")
          el-col(:span="16")
            el-form-item
              el-select(
                v-model="form.tags"
                size="medium"
                placeholder="Теги"
                multiple filterable allow-create default-first-option)
          el-col(:span="8")
            el-form-item
              el-select(
                v-model="form.priority"
                size="medium"
                placeholder="Приоритет")
                el-option(v-for="item in priorities" :key="item.value" :label="item.label" :value="item.value")
        el-row(:gutter="20")
          el-col(:span="8")
            el-form-item
              el-select(
                v-model="form.responsible"
                size="medium"
                placeholder="Ответственный")
                el-option(v-for="item in users" :key="item.value" :label="item.label" :value="item.value")
          el-col(:span="8")
            el-form-item
              el-select(
                v-model="form.list"
                size="medium"
                placeholder="Проект")
                el-option(v-for="item in projects" :key="item.value" :label="item.label" :value="item.value")
          el-col(:span="8")
            el-form-item
              el-date-picker(
                v-model="form.dateEnd"
                size="medium"
                prefix-icon="el-icon-arrow-down"
                suffix-icon="el-icon-arrow-down"
                placeholder="Крайний срок")
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
        title: "",
        description: "",
        tags: [],
        observers: [],
        priority: null,
        ownerId: null,
        performerId: null,
        scopeId: null,
        creationDate: new Date(),
        dateEnd: null
      },
      priorities: [
        { value: 0, label: "Важно" },
        { value: 1, label: "Срочно" }
      ],
      users: [
        { value: 0, label: "Виталий" },
        { value: 1, label: "Алексей" },
        { value: 2, label: "Андрей" }
      ],
      projects: [],
    };
  },
  async mounted() {
    this.visible = true;
    if (this.isEdit) {
      this.loading = true;
      await this.$store.dispatch('getTask', this.id);
      this.form = this.$store.getters.task;
      this.loading = false;
    }
  },
  computed: {
    ...mapGetters({ task: 'tasks/getTask' })
  },
  methods: {
    ...mapActions({
      fetchTask: 'tasks/fetchTask',
      createTask: 'tasks/createTask',
      updateTask: 'tasks/updateTask'
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
              await this.updateTask(payload);
            else
              await this.createTask(payload);
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
.el-form-item {
  margin-bottom: 15px;
}
.el-select {
  width: 100%;
}
.el-range-editor.el-input__inner {
  width: 100%;
}
.el-date-editor.el-input,
.el-date-editor.el-input__inner {
  width: auto;
}
</style>

<style lang="scss">
.el-date-editor .el-input__prefix {
  left: unset;
  right: 5px;
}
.el-date-editor .el-input__inner {
  padding-left: 15px;
}
</style>
