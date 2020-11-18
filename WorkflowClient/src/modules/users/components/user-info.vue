<template>
  <el-form class="settings-account" :model="entity" :rules="rules" ref="form">
    <el-row :gutter="20">
      <el-col :span="8">
        <div class="section">
          <h2>Логин</h2>
          <el-form-item prop="userName">
            <el-input v-model="entity.userName"></el-input>
          </el-form-item>
        </div>
        <div class="section">
          <h2>Фамилия</h2>
          <el-form-item prop="lastName">
            <el-input v-model="entity.lastName"></el-input>
          </el-form-item>
        </div>
        <div class="section">
          <h2>Сброс пароля</h2>
          <el-form-item prop="newPassword">
            <el-input
              v-model="newPassword"
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
            <el-input v-model="entity.email"></el-input>
          </el-form-item>
        </div>
        <div class="section">
          <h2>Имя</h2>
          <el-form-item prop="firstName">
            <el-input v-model="entity.firstName"></el-input>
          </el-form-item>
        </div>
      </el-col>
      <el-col :span="8">
        <div class="section">
          <h2>Телефон</h2>
          <el-form-item prop="phone">
            <el-input v-model="entity.phone"></el-input>
          </el-form-item>
        </div>
        <div class="section">
          <h2>Отчество</h2>
          <el-form-item prop="middleName">
            <el-input v-model="entity.middleName"></el-input>
          </el-form-item>
        </div>
      </el-col>
    </el-row>

    <div class="section buttons">
      <el-button type="primary" @click="save" :loading="loading">Сохранить</el-button>
    </div>
  </el-form>
</template>

<script lang="ts">
import { Component, Vue, Watch } from 'vue-property-decorator'
import User from '../models/user.type'
import usersStore from '../store/users.store'
import authModule from '@/modules/users/store/auth.store'
import usersModule from '@/modules/users/store/users.store'

@Component
export default class UserInfo extends Vue {
  private loading = false
  private entity: User = new User()
  private newPassword = ''
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

  private get user(): User | null {
    return usersStore.user
  }

  @Watch('user')
  private onUserChange(newUser: User): void {
    this.entity = { ...newUser }
  }

  protected mounted(): void {
    if (this.user) this.entity = { ...this.user }
  }

  private async save(): Promise<void> {
    try {
      this.loading = true
      // Updating password
      if (this.newPassword && this.entity.id) {
        const success = usersStore.resetPassword({
          userId: this.entity.id,
          newPassword: this.newPassword,
        })
        if (!success) this.$message.error('Не удалось сбросить пароль')
        this.newPassword = ''
      }

      // Updating account data
      if (JSON.stringify(this.entity) !== JSON.stringify(usersStore.user)) {
        await usersModule.updateOne(this.entity)
        this.$message.success('Данные профиля успешно обновлены')
      }
      this.loading = false
    } catch (e) {
      this.$message.error('Не удалось обновить данные профиля')
      console.error(e)
      this.loading = false
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
