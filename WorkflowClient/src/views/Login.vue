<template lang="pug">
  el-card(shadow="never")
    el-form(:model="form" :rules="rules" ref="form" @keyup.enter.native="submit")
      img.logo(src="@/assets/logo.svg")
      h1.title Вход
      el-form-item(prop="login")
        el-input(v-model="form.userName" placeholder="Почта" autofocus)
      el-form-item(prop="password")
        el-input(v-model="form.password" type="password" autocomplete="off" placeholder="Пароль")
      el-form-item
        el-button(type="primary" :loading="loading" @click="submit") Войти
      //el-checkbox(v-model="form.rememberMe") Запомнить меня
</template>

<script lang="ts">
import { Vue, Component, Ref } from 'vue-property-decorator';
import { ElForm } from 'element-ui/types/form';
import Credentials from '@/types/credentials.type';
import authModule from '@/store/modules/auth.module';

@Component
export default class Login extends Vue {
  @Ref('form') readonly formComponent!: ElForm;

  private loading = false;
  private form: Credentials = {
    userName: '',
    password: '',
    rememberMe: false
  };
  private rules = {
    userName: [{ required: true, message: '!', trigger: 'blur' }],
    password: [{ required: true, message: '!', trigger: 'blur' }]
  };

  async submit(): Promise<void> {
    const formCredentials = { ...this.form };
    this.formComponent.validate(async valid => {
      if (valid) {
        try {
          this.loading = true;
          await authModule.login(formCredentials);
          await this.$router.push('/tasks');
          this.loading = false;
        } catch (error) {
          this.loading = false;
          console.error(error);
          this.$message({
            showClose: true,
            message: 'Неверный логин или пароль',
            type: 'error'
          });
        }
      } else {
        this.$message({
          showClose: true,
          message: 'Форма заполнена некорректно',
          type: 'error'
        });
      }
      this.formComponent.resetFields();
    });
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
