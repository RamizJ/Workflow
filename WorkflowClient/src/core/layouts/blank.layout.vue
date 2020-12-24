<template>
  <main>
    <router-view :key="$route.path" />
  </main>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'

@Component
export default class BlankLayout extends Vue {}
</script>

<style lang="scss" scoped>
main {
  width: 100vw;
  height: 100vh;
  display: flex;
  justify-content: center;
}
</style>
