<template>
  <div class="page-header">
    <div>
      <div class="page-header__title">
        <slot name="title" />
        <div class="page-header__action">
          <slot name="action" />
        </div>
      </div>
      <div class="page-header__subtitle">
        <slot name="subtitle" />
      </div>
    </div>
    <div class="page-header__search">
      <slot name="search" />
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'

@Component
export default class BaseHeader extends Vue {}
</script>

<style lang="scss" scoped>
.page-header {
  padding: 25px 0 25px;
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
}
input {
  border: none;
  padding: 0;
  color: var(--text);
  background-color: var(--background);
  &.title {
    font-size: 28px;
    font-weight: 800;
    height: auto;
  }
  &.subtitle {
    font-size: 14px;
    font-weight: 400;
  }
}
.page-header__title {
  cursor: default;
  display: flex;
  align-items: baseline;
  font-size: 28px;
  line-height: 34px;
  font-weight: 800;
  &:hover > .page-header__action {
    //opacity: 1;
    //margin-left: 10px;
  }
}
.page-header__subtitle > * {
  margin-top: 10px;
  margin-bottom: 8px;
  font-size: 14px;
}
</style>

<style lang="scss">
.page-header__search {
  //width: 25%;
  .el-button {
    //padding: 8px 6px;
  }
  .el-input__inner {
    padding-left: 35px;
  }
  .el-button.active i {
    color: var(--color-primary) !important;
  }
}
.page-header__action {
  margin-left: 6px;
  height: 34px;
  i {
    color: var(--text);
    padding: 0;
    vertical-align: text-bottom;
  }
}
.page-header .actions {
  background-color: transparent;
  height: 21px;
  vertical-align: bottom;
  cursor: pointer;
  color: var(--text);
  opacity: 0.7;
  transition: opacity 0.25s;
  &:hover {
    opacity: 1;
  }
}
</style>
