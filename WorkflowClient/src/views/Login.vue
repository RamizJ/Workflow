<template>
  <el-card shadow="never">
    <el-form :model="form" :rules="rules" ref="form" @keyup.enter.native="submit">
      <img class="logo" src="@/assets/logo.svg" />
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
        <el-button type="primary" :loading="loading" @click="submit">Войти</el-button>
      </el-form-item>
    </el-form>
  </el-card>
</template>

<script lang="ts">
import { Vue, Component, Ref } from 'vue-property-decorator'
import { ElForm } from 'element-ui/types/form'

import authModule from '@/store/modules/auth.module'
import Credentials from '@/types/credentials.type'

@Component
export default class LoginPage extends Vue {
  @Ref('form') readonly formComponent!: ElForm

  private loading = false
  private form: Credentials = {
    userName: '',
    password: '',
    rememberMe: false,
  }
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
          await authModule.login(formCredentials)
          await this.$router.push('/tasks')
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
      this.formComponent.resetFields()
    })
  }
}
</script>

<style lang="scss" scoped>
.el-card {
  text-align: center;
  width: 350px;
  padding: 20px 28px;
  margin-bottom: 20vh;
  border-radius: 6px;
  color: var(--text);
  border-color: transparent;
  background-color: transparent;
  .logo {
    height: 110px;
    margin-bottom: 40px;
  }
  .title {
    text-align: center;
    font-size: 28px;
    font-weight: 600;
    margin-bottom: 24px;
  }
  .el-button {
    width: 100%;
  }
  .support {
    font-size: 13px;
  }
}
</style>
