<template>
  <el-menu
    class="menu"
    :router="!!router"
    :mode="mode || 'vertical'"
    :default-active="defaultActive || $route.path"
  >
    <div class="menu__header">
      <slot name="header"></slot>
    </div>
    <slot></slot>
    <div class="menu__footer">
      <slot name="footer"></slot>
    </div>
  </el-menu>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'

@Component
export default class Menu extends Vue {
  @Prop() readonly mode?: string
  @Prop() readonly router?: boolean
  @Prop() readonly defaultActive?: string
}
</script>

<style lang="scss" scoped>
.menu {
  overflow: auto;
  border: none;
  height: 100%;
  position: relative;
  background-color: transparent;
  padding: 24px;
  &__footer {
    width: 100%;
    position: absolute;
    bottom: 0;
    left: 0;
    padding: 20px;
  }
}
</style>
