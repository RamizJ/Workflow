<template>
  <div class="layout-container">
    <app-sidebar></app-sidebar>
    <main>
      <transition name="page-fade" mode="out-in">
        <router-view :key="$route.path"></router-view>
      </transition>
    </main>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import AppSidebar from '@/components/AppSidebar.vue'

@Component({ components: { AppSidebar } })
export default class LayoutLogin extends Vue {}
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
