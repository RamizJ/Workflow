<template>
  <el-card shadow="never">
    <el-form :model="form" :rules="rules" ref="form" @keyup.enter.native="submit">
      <img class="logo" src="../images/logo.svg" />
      <h1 class="title">Вход</h1>
      <el-form-item prop="login">
        <el-input v-model="form.userName" placeholder="Почта" autofocus="autofocus"></el-input>
      </el-form-item>
      <el-form-item prop="password">
        <el-input
          v-model="form.password"
          type="password"
          autocomplete="off"
          placeholder="Пароль"
        ></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" :loading="loading" @click="submit">{{
          loginButtonText
        }}</el-button>
      </el-form-item>
      <el-form-item prop="rememberMe">
        <el-checkbox v-model="form.rememberMe">Запомнить меня</el-checkbox>
      </el-form-item>
      <el-form-item>
        <div class="support">
          По всем вопросам обращайтесь по указанному адресу электронной почты. Специалист службы
          технической поддержки свяжется с Вами.
        </div>
        <el-link class="support" href="mailto:support@girngm.ru">support@girngm.ru</el-link>
      </el-form-item>
    </el-form>
  </el-card>
</template>

<script lang="ts">
import { Vue, Component, Ref } from 'vue-property-decorator'
import { ElForm } from 'element-ui/types/form'

import authModule from '@/modules/users/store/auth.store'
import Credentials from '@/core/types/credentials.type'

@Component
export default class LoginPage extends Vue {
  @Ref('form') readonly formComponent!: ElForm

  private loading = false
  private form: Credentials = new Credentials('', '')
  private loginButtonText = 'Войти'
  private rules = {
    userName: [{ required: true, message: '!', trigger: 'blur' }],
    password: [{ required: true, message: '!', trigger: 'blur' }],
  }

  private async submit(): Promise<void> {
    const formCredentials = { ...this.form }
    this.formComponent.validate(async (valid) => {
      if (valid) {
        try {
          this.loading = true
          this.loginButtonText = ''
          await authModule.login(formCredentials)
          await this.$router.push('/')
          this.loginButtonText = 'Войти'
          this.loading = false
        } catch (error) {
          this.loading = false
          console.error(error)
          this.$message({
            showClose: true,
            message: 'Неверный логин или пароль',
            type: 'error',
          })
        }
      } else {
        this.$message({
          showClose: true,
          message: 'Форма заполнена некорректно',
          type: 'error',
        })
      }
    })
  }
}
</script>

<style lang="scss" scoped>
.el-card {
  text-align: center;
  width: 375px;
  padding: 20px 28px;
  margin-top: 5vh;
  border-radius: 6px;
  color: var(--text);
  border-color: transparent;
  background-color: transparent;
  .logo {
    height: 100px;
    margin-bottom: 40px;
  }
  .title {
    text-align: center;
    font-size: 28px;
    font-weight: 700;
    margin-bottom: 24px;
  }
  .el-button {
    width: 100%;
  }
  .support {
    font-size: 13px;
    line-height: normal;
  }
}
</style>
