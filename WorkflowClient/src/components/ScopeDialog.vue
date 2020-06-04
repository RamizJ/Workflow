<template lang="pug">
  base-dialog(v-if="visible" @close="$emit('close')")
    div(slot="title") Область
    div(slot="body")
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
    div(slot="footer")
      el-button(size="medium" type="primary" @click="submit") Создать

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseDialog from '~/components/BaseDialog';

export default {
  components: { BaseDialog },
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