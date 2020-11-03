<template>
  <div class="page-header">
    <div class="page-header__left">
      <slot />
    </div>
    <div class="page-header__left">
      <slot name="right" />
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'

@Component
export default class BasePageHeader extends Vue {}
</script>

<style lang="scss" scoped>
.page-header {
  padding-top: 20px;
  padding-bottom: 10px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  &__left {
    cursor: default;
    display: flex;
    align-items: baseline;
    font-size: 24px;
    font-family: var(--header-font);
    line-height: 29px;
    font-weight: 600;
  }
  &__subtitle > * {
    margin-top: 10px;
    margin-bottom: 8px;
    font-size: 14px;
  }
  &__action {
    margin-left: 8px;
    margin-top: auto;
    margin-bottom: 4px;
    .el-button--text {
      padding: 0;
    }
    svg {
      fill: var(--text-muted);
      padding: 0;
      vertical-align: text-bottom;
    }
  }
  input {
    border: none;
    padding: 0;
    color: var(--text);
    background-color: var(--input-background);
    &.title {
      font-size: 24px;
      font-weight: 600;
      font-family: var(--header-font);
      height: auto;
    }
    &.subtitle {
      font-size: 14px;
      font-weight: 400;
    }
  }
}
</style>
