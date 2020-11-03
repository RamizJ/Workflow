<template>
  <el-tabs v-model="tab" @tab-click="onTabClick">
    <el-tab-pane v-for="(tab, index) in tabs" :key="index" :label="tab.label" :name="tab.name">
      <component :is="tab.component" />
    </el-tab-pane>
  </el-tabs>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'

@Component
export default class BaseTabs extends Vue {
  @Prop() readonly tabs!: Array<{ label: string; name: string; component: any }>
  @Prop() readonly value!: string
  private tab = ''

  @Watch('input')
  onInputChange(value: string): void {
    this.tab = value
  }

  protected mounted(): void {
    this.tab = this.value
  }

  private onTabClick(tab: string): void {
    this.$emit('input', this.tab)
    this.$emit('tab-click', this.tab)
  }
}
</script>

<style lang="scss">
.el-tabs {
  margin-top: -12px !important;
  padding: 0 30px;
  margin: 0 -30px;
  width: calc(100% + 60px);
  display: flex;
  flex-direction: column;
  overflow: hidden;
  flex: 1;
}

.el-tabs__content,
.el-tab-pane {
  padding-left: 10px;
  padding-right: 10px;
  margin-left: -10px;
  margin-right: -10px;
}
.el-tab-pane {
  height: 100%;
  display: flex;
  flex-direction: column;
  overflow-y: auto;
  overflow-x: hidden;
}
.el-tabs__content {
  height: 100%;
}
.el-tabs__item {
  color: var(--text);
  height: 46px;
  line-height: 45px;
  &:hover,
  &.is-active {
    color: var(--color-primary);
  }
  &:focus.is-active.is-focus:not(:active) {
    box-shadow: none;
  }
}
.el-tabs__nav-wrap {
  padding: 0 30px;
  margin: 0 -30px;
  border-bottom: var(--dropdown-border);
  &:after {
    background-color: var(--input-focus-background);
    height: 1px;
  }
}
.el-tabs__active-bar {
  border-radius: 2px;
  background-color: var(--color-primary);
}
</style>
