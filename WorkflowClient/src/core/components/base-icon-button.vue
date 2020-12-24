<template>
  <el-button :size="size || 'small'" class="icon-button" plain @click="onClick">
    <el-badge v-if="badgeNumber" :value="badgeNumber" class="item"
      ><unicon :name="icon"
    /></el-badge>
    <unicon v-else :name="icon" />
  </el-button>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'

@Component
export default class IconButton extends Vue {
  @Prop() icon!: string
  @Prop() size?: string
  @Prop() badge?: number

  private badgeNumber = 0

  protected mounted(): void {
    this.onBadgeChange(this.badge)
  }

  @Watch('badge')
  onBadgeChange(value?: number): void {
    this.badgeNumber = value || 0
  }

  private onClick(): void {
    this.$emit('click')
  }
}
</script>

<style lang="scss">
.icon-button.el-button.is-plain {
  padding: 7px 6px 6px;
  &:hover {
    border: var(--button-hover-border) !important;
    background-color: var(--button-background);
  }
  &:focus {
    border: var(--button-hover-border) !important;
    background-color: var(--button-background);
    //background-color: var(--sidebar-item-hover-background);
  }
  svg {
    height: 20px;
  }
}
</style>
