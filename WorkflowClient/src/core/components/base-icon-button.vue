<template>
  <el-button :size="size || 'small'" class="icon-button" plain @click="onClick">
    <unicon :name="icon" />
  </el-button>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'

@Component
export default class IconButton extends Vue {
  @Prop() icon!: string
  @Prop() size?: string

  private onClick(): void {
    this.$emit('click')
  }
}
</script>

<style lang="scss">
.icon-button.el-button.is-plain {
  padding: 7px 6px 6px;
  &:hover {
    border-color: var(--sidebar-item-hover-background);
    background-color: transparent;
  }
  &:focus {
    border-color: var(--sidebar-item-hover-background);
    background-color: var(--sidebar-item-hover-background);
  }
  svg {
    height: 20px;
  }
}
</style>
