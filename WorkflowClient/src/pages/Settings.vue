<template lang="pug">
  page
    page-header
      template(slot="title") Настройки

    div.item
      div.item__title Оформление
      div.item__content
        img.theme-icon(@click="switchTheme('light')" :class="appearance === 'light' ? 'active' : ''" src="../assets/icons/theme-light.png")
        img.theme-icon(@click="switchTheme('dark')" :class="appearance === 'dark' ? 'active' : ''" src="../assets/icons/theme-dark.png")
</template>

<script>
import Page from '~/components/Page';
import PageHeader from '~/components/PageHeader';

export default {
  name: 'Settings',
  components: {
    Page,
    PageHeader
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
.item__title {
  margin-top: 15px;
  font-size: 14px;
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
