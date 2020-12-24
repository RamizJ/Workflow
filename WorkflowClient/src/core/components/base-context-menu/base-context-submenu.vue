<template>
  <li class="v-context__sub">
    <a>{{ title }}</a>
    <ul class="v-context">
      <slot />
    </ul>
  </li>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'

@Component
export default class BaseContextSubmenu extends Vue {
  @Prop() readonly title!: string
}
</script>

<style scoped></style>
