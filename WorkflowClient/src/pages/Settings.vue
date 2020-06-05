<template lang="pug">
  div.container
    base-toolbar
      template(slot="title") Настройки
    div.settings
      div.item
        div.item__title Оформление
        div.item__content
          img.theme-icon(@click="switchTheme('light')" :class="appearance === 'light' ? 'active' : ''" src="../assets/icons/theme-light.png")
          img.theme-icon(@click="switchTheme('dark')" :class="appearance === 'dark' ? 'active' : ''" src="../assets/icons/theme-dark.png")
</template>

<script>
import { mapActions } from 'vuex';
import BaseToolbar from '~/components/BaseToolbar';
import BaseToolbarItem from '~/components/BaseToolbarItem';

export default {
  name: 'Settings',
  components: {
    BaseToolbar,
    BaseToolbarItem
  },
  data() {
    return {
      search: '',
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
.item__title {
  font-size: 16px;
  font-weight: 600;
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
  transition: border-color .25s;
  &.active {
    border-color: var(--color-primary);
  }
}
</style>
