<template>
  <li>
    <a @click.prevent="onClick">
      <slot />
    </a>
  </li>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'

@Component
export default class BaseContextMenuItem extends Vue {
  private onClick(): void {
    this.$emit('click')
  }
}
</script>

<style scoped></style>
