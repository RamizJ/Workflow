<template lang="pug">
  base-dialog(v-if="visible" @close="$emit('close')")
    div(slot="title") Пользователь
    div(slot="body")
      el-form(:model="form" :rules="rules" ref="form")
        el-row(:gutter="20")
          el-col(:span="8")
            el-form-item(prop="lastName" required)
              el-input(v-model="form.lastName" size="medium" placeholder="Фамилия" required)
          el-col(:span="8")
            el-form-item(prop="firstName")
              el-input(v-model="form.firstName" size="medium" placeholder="Имя")
          el-col(:span="8")
            el-form-item(prop="middleName")
              el-input(v-model="form.middleName" size="medium" placeholder="Отчество")
        el-row(:gutter="20")
          el-col(:span="8")
            el-form-item(prop="userName")
              el-input(v-model="form.userName" size="medium" placeholder="Логин")
          el-col(:span="8")
            el-form-item(prop="password")
              el-input(v-model="form.password" type="password" size="medium" placeholder="Пароль")
          el-col(:span="8")
            el-form-item(prop="email")
              el-input(v-model="form.email" size="medium" placeholder="Почта")
        el-row(:gutter="20")
          el-col(:span="16")
            el-form-item(prop="scopes")
              el-select(v-model="form.scopeIds" size="medium" placeholder="Проекты" multiple)
                el-option(v-for="item in projects" :key="item.value" :label="item.label" :value="item.value")
          el-col(:span="8")
            el-form-item(prop="phone")
              el-input(v-model="form.phone" size="medium" placeholder="Телефон")
        el-row(:gutter="20")
          el-col(:span="16")
            el-form-item(prop="roles")
              el-select(v-model="form.roles" size="medium" placeholder="Права" multiple)
                el-option(v-for="item in roles" :key="item.value" :label="item.label" :value="item.value")
          el-col(:span="8")
            el-form-item(prop="position")
              el-select(v-model="form.positionId" size="medium" placeholder="Должность")
                el-option(v-for="item in positions" :key="item.value" :label="item.label" :value="item.value")
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
        lastName: [ { required: true, message: 'Укажите фамилию', trigger: 'blur', } ],
        firstName: [ { required: true, message: 'Укажите имя', trigger: 'blur', } ],
        userName: [ { required: true, message: 'Укажите логин', trigger: 'blur', } ],
        password: [
          { required: true, message: 'Укажите пароль', trigger: 'blur' },
          { min: 6, message: 'Минимальная длина - 6 символов', trigger: 'blur' }
        ],
        email: [
          { required: true, message: 'Укажите эл. почту', trigger: 'blur', },
          { type: 'email', message: 'Некорректный адрес эл. почты', trigger: 'blur' }
        ],
        phone: [ { required: true, message: 'Укажите номер телефона', trigger: 'blur', } ],
        position: [ { required: true, message: 'Пожалуйста, укажите должность', trigger: 'blur', } ],
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
