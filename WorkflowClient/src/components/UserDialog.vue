<template lang="pug">
  base-dialog(v-if="visible" @close="$emit('close')")
    div(slot="title") Область
    div(slot="body")
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
    div(slot="footer")
      el-button(size="medium" type="primary" @click="submit") Создать

</template>

<script>
import BaseDialog from '~/components/BaseDialog';
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
      await this.fetchUser(this.id);
      this.form = this.user;
      this.loading = false;
    }
  },
  computed: {
    ...mapGetters({ user: 'users/getUser' })
  },
  methods: {
    ...mapActions({
      fetchUser: 'users/fetchUser',
      createUser: 'users/createUser',
      updateUser: 'users/updateUser'
    }),
    submit() {
      const payload = { ...this.form };
      const form = this.$refs.form;
      form.validate(async (valid) => {
        if (valid) {
          try {
            if (this.isEdit)
              await this.updateUser(payload);
            else
              await this.createUser(payload);
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
