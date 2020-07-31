<template lang="pug">
  page
    page-header
      template(slot="title") Настройки

    el-tabs(ref="tabs" v-model="activeTab")
      el-tab-pane(label="Профиль" name="profile")
        div.section
          h2 Оформление
          el-tooltip(content="Светлое" placement="top")
            el-button.theme-preview.light(type="text" @click="switchTheme('light')") А
          el-tooltip(content="Тёмное" placement="top")
            el-button.theme-preview.dark(type="text" @click="switchTheme('dark')") А
        div.section
          h2 Диалоговые окна
          el-checkbox(
            v-model="confirmDialogClose"
            label="Подтверждать закрытие диалоговых окон"
            @change="switchConfirmDialogClose")
        div.section
          h2 Отладка
          el-checkbox(
            v-model="debugMode"
            label="Отображать детали ошибок"
            @change="switchDebugMode")

      el-tab-pane(label="Аккаунт" name="account")
        el-form(:model="form" :rules="rules" ref="form")
          div.section
            h2 Логин
            el-form-item(prop="userName")
              el-input(v-model="form.userName")
          div.section
            h2 Почта
            el-form-item(prop="email")
              el-input(v-model="form.email")
          div.section
            h2 Телефон
            el-form-item(prop="phone")
              el-input(v-model="form.phone")
          div.section
            h2 Фамилия
            el-form-item(prop="lastName")
              el-input(v-model="form.lastName")
          div.section
            h2 Имя
            el-form-item(prop="firstName")
              el-input(v-model="form.firstName")
          div.section
            h2 Отчество
            el-form-item(prop="middleName")
              el-input(v-model="form.middleName")
          div.section
            h2 Смена пароля
            el-form-item(prop="newPassword")
              el-input(
                v-model="form.newPassword"
                type="password"
                placeholder="Новый пароль" readonly
                onfocus="this.removeAttribute('readonly')")
            el-form-item(prop="confirmNewPassword")
              el-input(
                v-model="form.confirmNewPassword"
                type="password"
                placeholder="Новый пароль ещё раз" readonly
                onfocus="this.removeAttribute('readonly')")
          div.section
            h2 Подтвердите текущий пароль
            el-form-item(prop="currentPassword")
              el-input(
                v-model="form.currentPassword"
                type="password"
                placeholder="Текущий пароль" readonly
                onfocus="this.removeAttribute('readonly')")
          div.section
            el-button(type="primary" @click="updateAccount") Сохранить
            el-button(@click="exit") Выйти

      el-tab-pane(label="Обновления" name="updates")
        changelog

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import Page from '@/components/Page';
import PageHeader from '@/components/BaseHeader';
import Changelog from '@/components/Changelog';

export default {
  name: 'Settings',
  components: {
    Page,
    PageHeader,
    Changelog
  },
  data() {
    return {
      search: '',
      form: {
        id: null,
        lastName: '',
        firstName: '',
        middleName: '',
        userName: '',
        newPassword: '',
        confirmNewPassword: '',
        currentPassword: '',
        email: '',
        phone: '',
        positionId: null,
        position: '',
        scopeIds: [],
        roles: []
      },
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
        ]
      },
      activeTab: 'profile',
      appearance: localStorage.getItem('theme') || 'light',
      confirmDialogClose: localStorage.confirmDialogClose === 'true',
      debugMode: localStorage.debugMode === 'true'
    };
  },
  mounted() {
    this.form = { ...this.me };
  },
  computed: {
    ...mapGetters({ me: 'auth/me' })
  },
  methods: {
    ...mapActions({
      logout: 'auth/logout',
      updateUser: 'users/updateUser',
      updatePassword: 'auth/updatePassword',
      fetchMe: 'auth/fetchMe'
    }),
    async updateAccount() {
      if (JSON.stringify(this.form) === JSON.stringify(this.me)) {
        this.$message.warning('Внесите правки для сохранения изменений');
        return;
      }
      try {
        if (this.form.newPassword) {
          if (!this.form.currentPassword)
            this.$message.warning(
              'Введите текущий пароль для сохранения настроек'
            );
          await this.updatePassword({
            currentPassword: this.form.currentPassword,
            newPassword: this.form.newPassword
          });
        }
        await this.updateUser(this.form);
        await this.fetchMe();
        this.$message.success('Данные профиля успешно обновлены');
      } catch (e) {
        this.$message.error('Не удалось обновить данные профиля');
        console.error(e);
      }
    },
    async exit() {
      try {
        await this.logout();
        await this.$router.push({ name: 'Login' });
      } catch (e) {
        this.$message.error('Ошибка выхода из учётной записи');
      }
    },
    switchTheme(appearance) {
      localStorage.setItem('theme', appearance);
      document.documentElement.setAttribute('theme', appearance);
      this.appearance = appearance;
    },
    switchConfirmDialogClose(value) {
      localStorage.confirmDialogClose = value;
    },
    switchDebugMode(value) {
      localStorage.debugMode = value;
    }
  }
};
</script>

<style lang="scss" scoped>
.section {
  margin-bottom: 30px;
  h2 {
    font-size: 14px;
    font-weight: 500;
    margin-top: 10px;
    margin-bottom: 15px;
  }
  h1 {
    font-size: 28px;
    font-weight: 600;
    margin-top: 15px;
  }
}
.theme-preview {
  border-radius: 3px;
  border: solid 3px var(--color-primary);
  cursor: pointer;
  display: inline-block;
  font-weight: 500;
  height: 40px;
  margin-right: 10px;
  padding-left: 4px;
  padding-top: 14px;
  width: 40px;
  font-size: 18px;
  text-align: left;
  &.dark {
    background: #1b1b1b;
    color: #eeeeee;
  }
  &.light {
    background: #f6f6f6;
    color: #303030;
  }
}
</style>
