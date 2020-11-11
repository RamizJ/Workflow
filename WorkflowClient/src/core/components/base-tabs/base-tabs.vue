<template>
  <el-tabs v-model="tab" @tab-click="onTabClick">
    <el-tab-pane
      v-for="(tab, index) in tabs"
      :key="index"
      :label="tab.label"
      :name="tab.name"
      :lazy="true"
    >
      <Component v-if="tab.component" :is="tab.component" />
    </el-tab-pane>
  </el-tabs>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator'

@Component
export default class BaseTabs extends Vue {
  @Prop() readonly tabs!: Array<{ label: string; name: string; component?: any }>
  @Prop() readonly value!: string
  @Prop() readonly routing?: boolean
  private tab = ''

  @Watch('value')
  onValueChange(value: string): void {
    this.tab = value
  }

  protected mounted(): void {
    this.tab = this.value
    if (this.routing) this.$el.querySelector<HTMLDivElement>('.el-tabs__header')!.style.margin = '0'
  }

  private onTabClick(): void {
    this.$emit('input', this.tab)
    this.$emit('tab-click', this.tab)
  }
}
</script>

<style lang="scss">
.el-tabs {
  margin-top: -10px !important;
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
  border-bottom: var(--input-border);
  transition: 0.2s;
  &:after {
    background-color: var(--input-focus-background);
    height: 1px;
    transition: 0.2s;
  }
}
.el-tabs__active-bar {
  border-radius: 2px;
  background-color: var(--color-primary);
}
</style>
