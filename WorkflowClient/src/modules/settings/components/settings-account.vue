<template>
  <el-form class="settings-account" :model="form" :rules="rules" ref="form">
    <el-row :gutter="20">
      <el-col :span="8">
        <div class="section">
          <h2>Логин</h2>
          <el-form-item prop="userName">
            <el-input v-model="form.userName"></el-input>
          </el-form-item>
        </div>
        <div class="section">
          <h2>Фамилия</h2>
          <el-form-item prop="lastName">
            <el-input v-model="form.lastName"></el-input>
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
      </el-col>
      <el-col :span="8">
        <div class="section">
          <h2>Почта</h2>
          <el-form-item prop="email">
            <el-input v-model="form.email"></el-input>
          </el-form-item>
        </div>
        <div class="section">
          <h2>Имя</h2>
          <el-form-item prop="firstName">
            <el-input v-model="form.firstName"></el-input>
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
      </el-col>
      <el-col :span="8">
        <div class="section">
          <h2>Телефон</h2>
          <el-form-item prop="phone">
            <el-input v-model="form.phone"></el-input>
          </el-form-item>
        </div>
        <div class="section">
          <h2>Отчество</h2>
          <el-form-item prop="middleName">
            <el-input v-model="form.middleName"></el-input>
          </el-form-item>
        </div>
      </el-col>
    </el-row>

    <div class="section buttons">
      <el-button type="primary" @click="updateAccount" :loading="loading">Сохранить</el-button>
      <el-button @click="exit">Выйти</el-button>
    </div>
  </el-form>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'vue-property-decorator'
import User from '@/modules/users/models/user.type'
import authModule from '@/modules/users/store/auth.store'
import usersModule from '@/modules/users/store/users.store'
import settingsModule from '@/modules/settings/store/settings.store'

@Component
export default class SettingsAccount extends Vue {
  private loading = false
  private form: User = {
    lastName: '',
    firstName: '',
    middleName: '',
    userName: '',
    email: '',
    phone: '',
    roles: [],
  }
  private credentials = {
    currentPassword: '',
    newPassword: '',
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
        trigger: 'blur',
      },
    ],
  }

  protected async mounted(): Promise<void> {
    if (authModule.me) this.form = authModule.me
  }

  private async updateAccount(): Promise<void> {
    const currentPassword = this.credentials.currentPassword
    const newPassword = this.credentials.newPassword

    if (JSON.stringify(this.form) === JSON.stringify(authModule.me) && !newPassword) {
      this.$message.warning('Внесите правки для сохранения изменений')
      return
    }

    try {
      this.loading = true
      // Updating password
      if (newPassword && currentPassword) {
        const success = await authModule.changePassword({ currentPassword, newPassword })
        if (!success) this.$message.error('Не удалось обновить пароль')
        this.credentials.currentPassword = ''
        this.credentials.newPassword = ''
      } else if (newPassword && !currentPassword)
        this.$message.warning('Введите старый и новый пароль для смены пароля')

      // Updating account data
      if (JSON.stringify(this.form) !== JSON.stringify(authModule.me)) {
        await usersModule.updateOne(this.form)
        await authModule.updateMe()
        this.$message.success('Данные профиля успешно обновлены')
      }
      this.loading = false
    } catch (e) {
      this.$message.error('Не удалось обновить данные профиля')
      console.error(e)
      this.loading = false
    }
  }

  private async exit(): Promise<void> {
    try {
      await authModule.logout()
      await this.$router.push({ name: 'Login' })
      await settingsModule.closeSettings()
    } catch (e) {
      console.error(e)
      this.$message.error('Ошибка выхода из учётной записи')
    }
  }

  private validatePassword(rule: never, value: string, callback: CallableFunction): void {
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
.buttons {
  text-align: right;
}
.section {
  color: var(--text);
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
</style>
