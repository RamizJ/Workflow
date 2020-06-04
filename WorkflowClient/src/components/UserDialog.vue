<template lang="pug">
  el-dialog(:visible.sync="visible" :before-close="close" v-loading="loading")
    div.header(slot="title")
      div.title Пользователь
    div.body
      el-form(:model="form" :rules="rules" ref="form")
        el-row(:gutter="20")
          el-col(:span="8")
            el-form-item
              el-input(v-model="form.lastName" size="medium" placeholder="Фамилия")
          el-col(:span="8")
            el-form-item
              el-input(v-model="form.firstName" size="medium" placeholder="Имя")
          el-col(:span="8")
            el-form-item
              el-input(v-model="form.middleName" size="medium" placeholder="Отчество")
        el-row(:gutter="20")
          el-col(:span="8")
            el-form-item
              el-input(v-model="form.userName" size="medium" placeholder="Логин")
          el-col(:span="8")
            el-form-item
              el-input(v-model="form.password" size="medium" placeholder="Пароль")
          el-col(:span="8")
            el-form-item
              el-input(v-model="form.email" size="medium" placeholder="Почта")

        el-row(:gutter="20")
          el-col(:span="16")
            el-form-item
              el-select(v-model="form.scopeIds" size="medium" placeholder="Проекты" multiple)
                el-option(v-for="item in projects" :key="item.value" :label="item.label" :value="item.value")
          el-col(:span="8")
            el-form-item
              el-input(v-model="form.phone" size="medium" placeholder="Телефон")
        el-row(:gutter="20")
          el-col(:span="16")
            el-form-item
              el-select(v-model="form.roles" size="medium" placeholder="Права" multiple)
                el-option(v-for="item in roles" :key="item.value" :label="item.label" :value="item.value")
          el-col(:span="8")
            el-form-item
              el-select(v-model="form.positionId" size="medium" placeholder="Должность")
                el-option(v-for="item in positions" :key="item.value" :label="item.label" :value="item.value")
      div.footer
        el-button(size="medium" @click="close") Закрыть
        el-button(size="medium" type="primary" @click="submit") Создать
</template>

<script>
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
        lastName: '',
        firstName: '',
        middleName: '',
        userName: '',
        password: '',
        email: '',
        phone: '',
        positionId: null,
        position: '',
        scopeIds: [],
        roles: []
      },
      projects: [
        { value: 0, label: "Проект1" },
        { value: 1, label: "Проект2" },
        { value: 2, label: "Проект3" },
        { value: 3, label: "Проект4" }
      ],
      roles: [
        { value: 0, label: "Управление проектами" },
        { value: 1, label: "Управление пользователями" },
        { value: 2, label: "Управление областями" }
      ],
      positions: [
        { value: 0, label: "Начальник" },
        { value: 1, label: "Уборщик" },
        { value: 2, label: "Оператор" }
      ],
      rules: {
        title: [
          {
            required: true,
            message: 'Пожалуйста, укажите название',
            trigger: 'blur',
          },
        ],
        responsible: [
          {
            required: true,
            message: 'Пожалуйста, укажите ответственного',
            trigger: 'blur',
          },
        ],
      },
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
  methods: {
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
              await this.$store.dispatch('updateTask', payload);
            else
              await this.$store.dispatch('createTask', payload);
            this.$message.success('Задача успешно создана');
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
.el-form-item {
  margin-bottom: 15px;
}
.el-range-editor.el-input__inner {
  width: 100%;
}
</style>
