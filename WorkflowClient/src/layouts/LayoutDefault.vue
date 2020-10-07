<template>
  <div class="layout-container">
    <sidebar />
    <main>
      <transition name="page-fade" mode="out-in">
        <router-view :key="$route.path" />
      </transition>
    </main>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import Sidebar from '@/components/Sidebar/Sidebar.vue'

@Component({ components: { Sidebar } })
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
