<template>
  <div class="page-header">
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
import { Component, Vue } from 'vue-property-decorator'

@Component
export default class BasePageHeader extends Vue {}
</script>

<style lang="scss" scoped>
.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-bottom: var(--dropdown-border);
  margin: 0 -30px 12px;
  padding: 18px 30px 20px;
  min-height: 70px;
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
    font-family: var(--header-font);
    line-height: 24px;
    font-weight: 600;
    input {
      font-size: 24px;
      font-weight: 600;
      font-family: var(--header-font);
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
