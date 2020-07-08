<template lang="pug">
  base-dialog(v-if="visible" @close="exit")
    div(slot="title") Область
    div(slot="body")
      el-form(:model="form" :rules="rules" ref="form")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="name")
              el-input(v-model="form.name" size="medium" placeholder="Новая область")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="description")
              el-input(v-model="form.description" size="medium" type="textarea" placeholder="Заметки")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="projects")
              el-select(v-model="form.projects" size="medium" placeholder="Проекты" multiple)
                el-option(v-for="item in projects" :key="item.value" :label="item.label" :value="item.value")
    div(slot="footer")
      el-button(size="medium" type="primary" @click="submit") {{ isEdit ? 'Сохранить' : 'Создать' }}

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseDialog from '~/components/Dialog';

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
        projects: [],
        responsible: null,
        team: null,
        scope: null,
        dateStart: null,
        dateEnd: null
      },
      projects: [
        { value: 0, label: 'Проект1' },
        { value: 1, label: 'Проект2' },
        { value: 2, label: 'Проект3' },
        { value: 3, label: 'Проект4' }
      ],
      rules: {
        name: [
          {
            required: true,
            message: 'Введите название области',
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
      form.validate(async valid => {
        if (valid) {
          try {
            if (this.isEdit) await this.updateScope(payload);
            else await this.createScope(payload);
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
