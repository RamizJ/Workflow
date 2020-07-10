<template lang="pug">
  page
    page-header
      template(slot="title") Настройки

    el-tabs(ref="tabs" v-model="activeTab")
      el-tab-pane(label="Аккаунт" name="Account")
        el-form(:model="form" :rules="rules" ref="form")
          el-row
            el-col(:span="24")
              div.item
                div.item__title Логин
                div.item__content
                  el-form-item(prop="userName")
                    el-input(v-model="form.userName")
          el-row
            el-col(:span="24")
              div.item
                div.item__title Почта
                div.item__content
                  el-form-item(prop="email")
                    el-input(v-model="form.email")
          el-row(:gutter="40")
            el-col(:span="6")
              div.item
                div.item__title Телефон
                div.item__content
                  el-form-item(prop="phone")
                    el-input(v-model="form.phone")
            el-col(:span="6")
              div.item
                div.item__title Фамилия
                div.item__content
                  el-form-item(prop="lastName")
                    el-input(v-model="form.lastName")
            el-col(:span="6")
              div.item
                div.item__title Имя
                div.item__content
                  el-form-item(prop="firstName")
                    el-input(v-model="form.firstName")
            el-col(:span="6")
              div.item
                div.item__title Отчество
                div.item__content
                  el-form-item(prop="middleName")
                    el-input(v-model="form.middleName")
          el-row
            el-col(:span="24")
              div.item
                div.item__title Смена пароля
                div.item__content
                  el-form-item(prop="newPassword")
                    el-input(v-model="form.newPassword" type="password" placeholder="Новый пароль" readonly onfocus="this.removeAttribute('readonly')")
          el-row
            el-col(:span="24")
              div.item
                div.item__title Подтвердите текущий пароль
                div.item__content
                  el-form-item(prop="currentPassword")
                    el-input(v-model="form.currentPassword" type="password" placeholder="Текущий пароль" readonly onfocus="this.removeAttribute('readonly')")
          el-row
            el-col(:span="24")
              div.item
                div.item__content
                  el-form-item(prop="password")
                    el-button(type="primary" @click="updateAccount") Сохранить
                    el-button(@click="exit") Выйти

      el-tab-pane(label="Внешний вид" name="Appearance")
        div.item
          div.item__title Оформление
          div.item__content
            img.theme-icon(@click="switchTheme('light')" :class="appearance === 'light' ? 'active' : ''" src="../assets/icons/theme-light.png")
            img.theme-icon(@click="switchTheme('dark')" :class="appearance === 'dark' ? 'active' : ''" src="../assets/icons/theme-dark.png")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import Page from '~/components/Page';
import PageHeader from '~/components/PageHeader';

export default {
  name: 'Settings',
  components: {
    Page,
    PageHeader
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
      activeTab: 'Account',
      tabs: [
        { value: 'General', label: 'Общее' },
        { value: 'Account', label: 'Аккаунт' }
      ],
      appearance: localStorage.getItem('theme') || 'light'
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
      try {
        if (this.form.newPassword)
          await this.updatePassword({
            currentPassword: this.form.currentPassword,
            newPassword: this.form.newPassword
          });
        await this.updateUser(this.form);
        await this.fetchMe();
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
    }
  }
};
</script>

<style lang="scss" scoped>
.item__title {
  margin-top: 15px;
  font-size: 14px;
  font-weight: 500;
  margin-bottom: 15px;
}
.item__content {
}
.theme-icon {
  cursor: pointer;
  height: 65px;
  border-radius: 10px;
  border: 3px solid var(--input-background);
  margin-right: 15px;
  transition: border-color 0.25s;
  &.active {
    border-color: var(--color-primary);
  }
}
</style>
