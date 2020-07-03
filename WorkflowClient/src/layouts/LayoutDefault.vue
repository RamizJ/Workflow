<template lang="pug">
  div.layout-container
    app-sidebar
    main
      transition(name="page-fade" mode="out-in")
        keep-alive
          router-view(:key="$route.path")
</template>

<script>
import AppSidebar from '~/components/AppSidebar.vue';

export default {
  name: 'LayoutDefault',
  components: {
    AppSidebar
  }
};
</script>

<style lang="scss" scoped>
.layout-container {
  width: 100vw;
  height: 100vh;
  display: flex;
}
main {
  display: flex;
  width: calc(100% - var(--sidebar-width));
}
</style>
