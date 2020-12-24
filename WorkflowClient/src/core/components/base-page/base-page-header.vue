<template>
  <div class="page-header" :class="headerClass">
    <div class="page-header__left">
      <div class="page-header__title">
        <slot />
        <div class="page-header__action">
          <slot name="action" />
        </div>
      </div>
      <div class="page-header__subtitle"><slot name="subtitle" /></div>
    </div>
    <div class="page-header__right">
      <slot name="toolbar" />
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'

@Component
export default class BasePageHeader extends Vue {
  @Prop() readonly noBorder?: boolean
  @Prop() readonly height?: string
  @Prop() readonly size?: string

  private get headerClass(): string {
    const border = this.noBorder ? '' : 'bordered'
    const size = this.size
    return `${size} ${border}`
  }

  protected mounted(): void {
    const headerElement = this.$el as HTMLDivElement
    if (headerElement) headerElement.style.minHeight = `${this.height}px`
  }
}
</script>

<style lang="scss" scoped>
.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-bottom: none;
  margin: 0 -30px 12px;
  padding: 0 30px;
  min-height: var(--header-height);
  transition: border-color 0.2s;
  &.bordered {
    border-bottom: var(--input-border);
  }
  &.small {
    margin: -10px -30px 3px;
  }
  input {
    border: none;
    padding: 0;
    color: var(--text);
    background-color: var(--input-background);
  }
  &__title {
    cursor: default;
    display: flex;
    align-items: center;
    font-size: 24px;
    font-family: var(--fonts);
    line-height: 24px;
    font-weight: 600;
    input {
      font-size: 24px;
      font-weight: 600;
      font-family: var(--fonts);
      height: auto;
    }
  }
  &__subtitle > * {
    margin-top: 10px;
    margin-bottom: 8px;
    font-size: 14px;
    input {
      font-size: 14px;
      font-weight: 400;
    }
  }
  &__action {
    margin: auto auto auto 8px;
    height: 24px;
    display: flex;
    button {
      padding: 0 !important;
    }
    svg {
      fill: var(--text-muted);
      padding: 0;
      vertical-align: text-bottom;
    }
  }
}
</style>
