<template lang="pug">
  div.container
    base-toolbar
      template(slot="title") Профиль
      template(slot="subtitle")
        a(href="#" @click="exit") Выйти
</template>

<script>
import BaseToolbar from '~/components/BaseToolbar';
import { mapActions } from 'vuex';

export default {
  name: "Profile",
  components: { BaseToolbar },
  data() {
    return {

    };
  },
  methods: {
    ...mapActions({ logout: 'auth/logout' }),
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

<style lang="scss" scoped></style>
