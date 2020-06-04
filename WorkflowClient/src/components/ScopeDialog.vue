<template lang="pug">
  el-dialog(:visible.sync="visible" :before-close="close" v-loading="loading")
    div.header(slot="title")
      div.title Область
    div.body
      el-form(:model="form" ref="form")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item
              el-input(v-model="form.title" size="medium" placeholder="Новая область")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item
              el-input(v-model="form.description" size="medium" type="textarea" placeholder="Заметки")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item
              el-select(v-model="form.projects" size="medium" placeholder="Проекты" multiple)
                el-option(v-for="item in projects" :key="item.value" :label="item.label" :value="item.value")

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
          priority: null,
          description: "",
          projects: [],
          responsible: null,
          team: null,
          scope: null,
          dateStart: null,
          dateEnd: null
        },
        projects: [
          { value: 0, label: "Проект1" },
          { value: 1, label: "Проект2" },
          { value: 2, label: "Проект3" },
          { value: 3, label: "Проект4" }
        ]
      };
    },
    async mounted() {
      this.visible = true;
      if (this.isEdit) {
        this.loading = true;
        await this.fetchScope(this.id);
        this.form = this.scope;
        this.loading = false;
      }
    },
    computed: {
      ...mapGetters({ scope: 'scopes/getScope' })
    },
    methods: {
      ...mapActions({
        fetchScope: 'scopes/fetchScope',
        createScope: 'scopes/createScope',
        updateScope: 'scopes/updateScope'
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
                await this.updateScope(payload);
              else
                await this.createScope(payload);
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
