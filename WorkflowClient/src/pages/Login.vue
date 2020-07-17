<template lang="pug">
  el-card(shadow="never")
    el-form(:model="form" :rules="rules" ref="form" @keyup.enter.native="submit")
      h1.title Вход
      el-form-item(prop="login")
        el-input(v-model="form.login" placeholder="Логин" autofocus)
      el-form-item(prop="password")
        el-input(v-model="form.password" type="password" autocomplete="off" placeholder="Пароль")
      el-form-item
        el-button(type="primary" :loading="loading" @click="submit") Войти
      //el-checkbox(v-model="form.rememberMe") Запомнить меня
</template>

<script>
import { mapActions, mapGetters } from 'vuex';

export default {
  name: 'Login',
  data() {
    return {
      loading: false,
      form: {
        login: '',
        password: '',
        rememberMe: false
      },
      rules: {
        login: [{ required: true, message: '!', trigger: 'blur' }],
        password: [{ required: true, message: '!', trigger: 'blur' }]
      }
    };
  },
  computed: {
    ...mapGetters({ loggedIn: 'auth/loggedIn' })
  },
  methods: {
    ...mapActions({ login: 'auth/login' }),
    async submit() {
      const credentials = { ...this.form };
      const form = this.$refs.form;
      form.validate(async valid => {
        if (valid) {
          try {
            this.loading = true;
            await this.login(credentials);
            await this.$router.push('/tasks');
            form.resetFields();
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
      });
    }
  }
};
</script>

<style lang="scss" scoped>
.el-card {
  text-align: center;
  width: 350px;
  height: 300px;
  padding: 20px 28px;
  margin-bottom: 15vh;
  border-radius: 6px;
  color: var(--text);
  border-color: transparent;
  background-color: transparent;
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
