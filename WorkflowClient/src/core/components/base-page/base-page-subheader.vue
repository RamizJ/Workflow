<template>
  <div class="page-subheader">
    <div class="page-subheader__left">
      <div class="page-subheader__title">
        <slot />
      </div>
    </div>
    <div class="page-subheader__right">
      <slot name="right" />
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'

@Component
export default class BasePageSubheader extends Vue {}
</script>

<style lang="scss" scoped>
.page-subheader {
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-bottom: var(--input-border);
  margin: -12px -30px 12px;
  padding: 6px 30px 10px;
  input {
    border: none;
    padding: 0;
    color: var(--text);
    background-color: var(--input-background);
  }
  &__title {
    margin-top: 10px;
    margin-bottom: 8px;
    font-size: 14px;
    input {
      font-size: 14px;
      font-weight: 400;
    }
  }
}
</style>
