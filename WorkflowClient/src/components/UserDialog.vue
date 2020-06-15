<template lang="pug">
  base-dialog(v-if="visible" @close="$emit('close')")
    div(slot="title") Пользователь
    div(slot="body")
      el-form(:model="form" :rules="rules" ref="form" v-loading="loading")
        el-row(:gutter="20")
          el-col(:span="8")
            el-form-item(prop="lastName")
              el-input(v-model="form.lastName" size="medium" placeholder="Фамилия")
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
            el-form-item(prop="teams")
              el-select(v-model="form.teamIds" size="medium" placeholder="Команды" multiple)
                el-option(v-for="item in teams" :key="item.value" :label="item.label" :value="item.value")
          el-col(:span="8")
            el-form-item(prop="phone")
              el-input(v-model="form.phone" size="medium" placeholder="Телефон")
        el-row(:gutter="20")
          el-col(:span="16")
            el-form-item(prop="roles")
              el-select(v-model="form.roles" size="medium" placeholder="Права" multiple)
                el-option(v-for="item in roles" :key="item.value" :label="item.label" :value="item.value")
          el-col(:span="8")
            el-form-item(prop="positionId")
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
    id: Number
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
        teamIds: [],
        roles: []
      },
      teams: [
        { value: 0, label: 'Команда1' },
        { value: 1, label: 'Команда2' },
        { value: 2, label: 'Команда3' },
        { value: 3, label: 'Команда4' }
      ],
      roles: [
        { value: 0, label: 'Управление проектами' },
        { value: 1, label: 'Управление пользователями' },
        { value: 2, label: 'Управление областями' }
      ],
      positions: [
        { value: 0, label: 'Начальник' },
        { value: 1, label: 'Уборщик' },
        { value: 2, label: 'Оператор' }
      ],
      rules: {
        lastName: [
          { required: true, message: 'Введите фамилию', trigger: 'blur' }
        ],
        firstName: [
          { required: true, message: 'Введите имя', trigger: 'blur' }
        ],
        userName: [
          { required: true, message: 'Введите логин', trigger: 'blur' }
        ],
        password: [{ validator: this.validatePassword, trigger: 'blur' }],
        email: [
          { required: true, message: 'Введите эл. почту', trigger: 'blur' },
          {
            type: 'email',
            message: 'Некорректный адрес эл. почты',
            trigger: 'blur'
          }
        ],
        phone: [
          { required: true, message: 'Введите номер телефона', trigger: 'blur' }
        ]
      }
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
    validatePassword(rule, value, callback) {
      const length = value.trim().length;
      const symbolsLeft = 6 - length;
      if (!value) callback(new Error('Введите пароль'));
      else if (length < 6)
        callback(
          new Error(`Введите ещё ${symbolsLeft}
        ${
          symbolsLeft > 1
            ? symbolsLeft > 4
              ? 'символов'
              : 'символа'
            : 'символ'
        }`)
        );
      else if (!/[a-z]/.test(value))
        callback(new Error('Введите хотя бы одну букву'));
      else callback();
    },
    submit() {
      const payload = { ...this.form };
      const form = this.$refs.form;
      form.validate(async valid => {
        if (valid) {
          try {
            if (this.isEdit) await this.updateUser(payload);
            else await this.createUser(payload);
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
