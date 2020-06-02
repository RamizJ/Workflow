<template lang="pug">
  div.container
    base-toolbar
      template(slot="title") Настройки
    div.settings
      div.option
        div.option__title Оформление
        div.option__content
          img.theme-icon(@click="switchTheme('light')" :class="appearance === 'light' ? 'active' : ''" src="../assets/icons/theme-light.png")
          img.theme-icon(@click="switchTheme('dark')" :class="appearance === 'dark' ? 'active' : ''" src="../assets/icons/theme-dark.png")
</template>

<script>
import BaseToolbar from '~/components/BaseToolbar';
import { mapActions } from 'vuex';
export default {
  name: 'Settings',
  components: {
    BaseToolbar
  },
  data() {
    return {
      appearance: localStorage.getItem('theme')
    }
  },
  methods: {
    switchTheme(appearance) {
      localStorage.setItem('theme', appearance);
      document.documentElement.setAttribute('theme', appearance);
      this.appearance = appearance;
    }
  }
};
</script>

<style lang="scss" scoped>
.settings {
  padding: 0 35px;
}
.option__title {
  font-size: 20px;
  font-weight: 600;
  margin-bottom: 20px;
}
.option__content {

}
.theme-icon {
  cursor: pointer;
  height: 80px;
  border-radius: 10px;
  border: 3px solid var(--input-background);
  margin-right: 30px;
  transition: border-color .25s;
  &.active {
    border-color: var(--color-primary);
  }
}
</style>
