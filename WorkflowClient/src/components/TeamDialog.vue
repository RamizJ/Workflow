<template lang="pug">
  el-dialog(:visible.sync="visible" :before-close="close" v-loading="loading")
    div.header(slot="title")
      div.title Команда
    div.body
      el-form(:model="form" :rules="rules" ref="form")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item
              el-input(v-model="form.title" size="medium" placeholder="Новая команда")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item
              el-input(v-model="form.description" size="medium" type="textarea" placeholder="Заметки")
        el-row(:gutter="20")
          el-col(:span="16")
            el-form-item
              el-select(v-model="form.teamMembers" size="medium" placeholder="Участники" multiple)
                el-option(v-for="item in users" :key="item.value" :label="item.label" :value="item.value")
          el-col(:span="8")
            el-form-item
              el-select(v-model="form.scope" size="medium" placeholder="Проект")
                el-option(v-for="item in scopes" :key="item.value" :label="item.label" :value="item.value")
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
          title: "",
          priority: null,
          description: "",
          teamMembers: [],
          responsible: null,
          team: null,
          scope: null,
          dateStart: null,
          dateEnd: null
        },
        users: [
          { value: 0, label: "Виталий" },
          { value: 1, label: "Алексей" },
          { value: 2, label: "Иван" },
          { value: 3, label: "Константин" },
          { value: 4, label: "Олег" },
          { value: 5, label: "Николай" },
          { value: 6, label: "Андрей" }
        ],
        scopes: [
          { value: 0, label: "Виталий" },
          { value: 1, label: "Алексей" },
          { value: 2, label: "Андрей" }
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
