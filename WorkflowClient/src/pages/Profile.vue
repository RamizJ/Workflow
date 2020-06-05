<template lang="pug">
  div.container
    base-toolbar
      template(slot="title") {{ `${me.lastName} ${me.firstName}` }}
      template(slot="subtitle")
        a(href="#" @click="exit") Выйти
    div.profile
      div.items
        div.item
          div.item__title Логин
          div.item__content
            el-input(v-model="form.userName" @change="updateProfile" :placeholder="emptyText")
        div.item
          div.item__title Пароль
          div.item__content
            el-input(v-model="form.password" @change="updateProfile" type="password" placeholder="**********")
        div.item
          div.item__title Почта
          div.item__content
            el-input(v-model="form.email" @change="updateProfile" :placeholder="emptyText")
        div.item
          div.item__title Телефон
          div.item__content
            el-input(v-model="form.phone" @change="updateProfile" :placeholder="emptyText")
        div.item
          div.item__title Фамилия
          div.item__content
            el-input(v-model="form.lastName" @change="updateProfile" :placeholder="emptyText")
        div.item
          div.item__title Имя
          div.item__content
            el-input(v-model="form.firstName" @change="updateProfile" :placeholder="emptyText")
        div.item
          div.item__title Отчество
          div.item__content
            el-input(v-model="form.middleName" @change="updateProfile" :placeholder="emptyText")
        div.item
          div.item__title Должность
          div.item__content
            el-input(v-model="form.position" @change="updateProfile" :placeholder="emptyText")

</template>

<script>
import BaseToolbar from '~/components/BaseToolbar';
import { mapActions, mapGetters } from 'vuex';

export default {
  name: "Profile",
  components: { BaseToolbar },
  data() {
    return {
      emptyText: '-',
      form: {
        id: null,
        lastName: '',
        firstName: '',
        middleName: '',
        userName: '',
        password: '',
        email: '',
        phone: '',
        positionId: null,
        position: '',
        scopeIds: [],
        roles: []
      }
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
    async updateProfile() {
      try {
        if (this.form.password)
          await this.setPassword();
        await this.updateUser(this.form);
        await this.fetchMe();
      } catch (e) {
        this.$message.error('Не удалось обновить данные профиля');
        console.error(e);
      }
    },
    async setPassword() {
      const newPassword = this.form.password;
      const answer = await this.$prompt('Введите текущий пароль', 'Смена пароля', {
        confirmButtonText: 'OK',
        cancelButtonText: 'Закрыть'
      });
      const currentPassword = answer.value;
      try {
        await this.updatePassword(currentPassword, newPassword);
        this.$message.success('Пароль успешно изменен');
      } catch (e) {
        this.$message.error('Во время смены пароля произошла ошибка')
      }
    },
    async exit() {
      try {
        await this.logout();
        await this.$router.push({ name: 'Login' });
      } catch (e) {
        this.$message.error('Ошибка выхода из учётной записи');
      }
    }
  }
};
</script>

<style lang="scss" scoped>
.profile {
  padding: 0 35px;
}
.items {
  display: grid;
  grid-gap: 40px 20px;
  grid-template-columns: repeat(4, 200px);
}
.item__title {
  cursor: default;
  font-size: 16px;
  font-weight: 600;
  margin-bottom: 15px;
}
.item__content {
  font-size: 14px;
  cursor: pointer;
  color: var(--link);
}

</style>

<style lang="scss">
.profile {
  .el-input__inner {
    cursor: pointer;
    height: auto;
    line-height: unset;
    color: var(--link);
    border: none;
    border-radius: 0;
    padding: 0;
    background-color: transparent;
    &:hover, &:focus, &:hover:not(:focus) {
      color: var(--link-hover);
      background-color: transparent;
    }
  }
}
</style>