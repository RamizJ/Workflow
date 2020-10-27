<template>
  <div class="main-layout">
    <Sidebar />

    <router-view :key="$route.path" />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import Sidebar from '@/core/components/app-sidebar/app-sidebar.vue'

@Component({ components: { Sidebar } })
export default class MainLayout extends Vue {}
</script>

<style lang="scss" scoped>
.main-layout {
  width: 100vw;
  height: 100vh;
  display: flex;
}
main {
  display: flex;
  width: calc(100% - var(--sidebar-width));
}
</style>
