<template>
  <div class="main-layout">
    <Sidebar />
    <main>
      <router-view :key="$route.path" />
    </main>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import Sidebar from '@/core/components/app-sidebar.vue'

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
