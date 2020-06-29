<template lang="pug">
  div.container
    base-header
      template(slot="title") Настройки

    div.settings
      div.item
        div.item__title Оформление
        div.item__content
          img.theme-icon(@click="switchTheme('light')" :class="appearance === 'light' ? 'active' : ''" src="../assets/icons/theme-light.png")
          img.theme-icon(@click="switchTheme('dark')" :class="appearance === 'dark' ? 'active' : ''" src="../assets/icons/theme-dark.png")
</template>

<script>
import BaseHeader from '~/components/BaseHeader';

export default {
  name: 'Settings',
  components: {
    BaseHeader
  },
  data() {
    return {
      search: '',
      appearance: localStorage.getItem('theme') || 'light'
    };
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
.item__title {
  font-size: 15px;
  font-weight: 500;
  margin-bottom: 15px;
}
.item__content {
}
.theme-icon {
  cursor: pointer;
  height: 65px;
  border-radius: 10px;
  border: 3px solid var(--input-background);
  margin-right: 15px;
  transition: border-color 0.25s;
  &.active {
    border-color: var(--color-primary);
  }
}
</style>
