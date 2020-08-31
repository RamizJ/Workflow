<template>
  <div class="page">
    <div class="header">
      <div class="header__title">Настройки</div>
    </div>
    <el-tabs ref="tabs" v-model="activeTab">
      <el-tab-pane label="Профиль" name="profile">
        <div class="section">
          <h2>Оформление</h2>
          <el-tooltip content="Светлое" placement="top">
            <el-button class="theme-preview light" type="text" @click="switchTheme('light')"
              >А</el-button
            >
          </el-tooltip>
          <el-tooltip content="Тёмное" placement="top">
            <el-button class="theme-preview dark" type="text" @click="switchTheme('dark')"
              >А</el-button
            >
          </el-tooltip>
        </div>
        <div class="section">
          <h2>Диалоговые окна</h2>
          <el-checkbox
            v-model="confirmDialogClose"
            label="Подтверждать закрытие диалоговых окон"
            @change="switchConfirmDialogClose"
          ></el-checkbox>
        </div>
        <div class="section">
          <h2>Отладка</h2>
          <el-checkbox
            v-model="debugMode"
            label="Отображать детали ошибок"
            @change="switchDebugMode"
          ></el-checkbox>
        </div>
      </el-tab-pane>
      <el-tab-pane label="Аккаунт" name="account">
        <el-form :model="form" :rules="rules" ref="form">
          <div class="section">
            <h2>Логин</h2>
            <el-form-item prop="userName">
              <el-input v-model="form.userName"></el-input>
            </el-form-item>
          </div>
          <div class="section">
            <h2>Почта</h2>
            <el-form-item prop="email">
              <el-input v-model="form.email"></el-input>
            </el-form-item>
          </div>
          <div class="section">
            <h2>Телефон</h2>
            <el-form-item prop="phone">
              <el-input v-model="form.phone"></el-input>
            </el-form-item>
          </div>
          <div class="section">
            <h2>Фамилия</h2>
            <el-form-item prop="lastName">
              <el-input v-model="form.lastName"></el-input>
            </el-form-item>
          </div>
          <div class="section">
            <h2>Имя</h2>
            <el-form-item prop="firstName">
              <el-input v-model="form.firstName"></el-input>
            </el-form-item>
          </div>
          <div class="section">
            <h2>Отчество</h2>
            <el-form-item prop="middleName">
              <el-input v-model="form.middleName"></el-input>
            </el-form-item>
          </div>
          <div class="section">
            <h2>Смена пароля</h2>
            <el-form-item prop="newPassword">
              <el-input
                v-model="credentials.newPassword"
                type="password"
                placeholder="Новый пароль"
                readonly="readonly"
                onfocus="this.removeAttribute('readonly')"
              ></el-input>
            </el-form-item>
          </div>
          <div class="section">
            <h2>Подтвердите текущий пароль</h2>
            <el-form-item prop="currentPassword">
              <el-input
                v-model="credentials.currentPassword"
                type="password"
                placeholder="Текущий пароль"
                readonly="readonly"
                onfocus="this.removeAttribute('readonly')"
              ></el-input>
            </el-form-item>
          </div>
          <div class="section">
            <el-button type="primary" @click="updateAccount">Сохранить</el-button>
            <el-button @click="exit">Выйти</el-button>
          </div>
        </el-form>
      </el-tab-pane>
      <el-tab-pane label="Обновления" name="updates">
        <changelog></changelog>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'

import authModule from '@/store/modules/auth.module'
import usersModule from '@/store/modules/users.module'
import Changelog from '@/components/Changelog/Changelog.vue'
import User from '@/types/user.type'

@Component({
  components: {
    Changelog
  }
})
export default class SettingsPage extends Vue {
  private search = ''
  private form: User = {
    lastName: '',
    firstName: '',
    middleName: '',
    userName: '',
    email: '',
    phone: '',
    roles: []
  }
  private credentials = {
    currentPassword: '',
    newPassword: ''
  }
  private rules = {
    lastName: [{ required: true, message: 'Введите фамилию', trigger: 'blur' }],
    firstName: [{ required: true, message: 'Введите имя', trigger: 'blur' }],
    userName: [{ required: true, message: 'Введите логин', trigger: 'blur' }],
    password: [{ validator: this.validatePassword, trigger: 'blur' }],
    email: [
      { required: true, message: 'Введите эл. почту', trigger: 'blur' },
      {
        type: 'email',
        message: 'Некорректный адрес эл. почты',
        trigger: 'blur'
      }
    ]
  }
  private activeTab = 'profile'
  private appearance = localStorage.getItem('theme') || 'light'
  private confirmDialogClose = localStorage.confirmDialogClose === 'true'
  private debugMode = localStorage.debugMode === 'true'

  private mounted() {
    if (authModule.me) this.form = { ...authModule.me } as User
  }

  async updateAccount() {
    if (JSON.stringify(this.form) === JSON.stringify(authModule.me)) {
      this.$message.warning('Внесите правки для сохранения изменений')
      return
    }
    try {
      const currentPassword = this.credentials.currentPassword
      const newPassword = this.credentials.newPassword
      if (newPassword) {
        if (!currentPassword)
          this.$message.warning('Введите текущий пароль для сохранения настроек')
        else await authModule.changePassword({ currentPassword, newPassword })
      }
      await usersModule.updateOne(this.form)
      await authModule.fetchMe()
      this.$message.success('Данные профиля успешно обновлены')
    } catch (e) {
      this.$message.error('Не удалось обновить данные профиля')
      console.error(e)
    }
  }

  async exit() {
    try {
      await authModule.logout()
      await this.$router.push({ name: 'Login' })
    } catch (e) {
      this.$message.error('Ошибка выхода из учётной записи')
    }
  }

  switchTheme(appearance: string) {
    localStorage.setItem('theme', appearance)
    document.documentElement.setAttribute('theme', appearance)
    this.appearance = appearance
  }

  switchConfirmDialogClose(value: string) {
    localStorage.confirmDialogClose = value
  }

  switchDebugMode(value: string) {
    localStorage.debugMode = value
  }

  private validatePassword(rule: any, value: string, callback: any) {
    const length = value?.trim().length
    const symbolsLeft = 6 - length
    if (!value) callback(new Error('!'))
    else if (length < 6)
      callback(
        new Error(`ещё ${symbolsLeft}
        ${symbolsLeft > 1 ? (symbolsLeft > 4 ? 'символов' : 'символа') : 'символ'}`)
      )
    else if (!/[a-z]/.test(value)) callback(new Error('нужна буква'))
    else callback()
  }
}
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
