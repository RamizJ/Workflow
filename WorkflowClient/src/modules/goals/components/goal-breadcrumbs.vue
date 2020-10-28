<template>
  <el-breadcrumb class="goal-breadcrumbs" :class="breadcrumbsActive ? 'active' : ''" separator="/">
    <el-breadcrumb-item
      v-for="breadcrumb in breadcrumbs"
      :key="breadcrumb.path"
      :to="{ path: breadcrumb.path }"
      @click.native="goto(breadcrumb.path)"
    >
      {{ breadcrumb.label }}
    </el-breadcrumb-item>
  </el-breadcrumb>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import breadcrumbStore from '@/modules/goals/store/breadcrumb.store'

@Component
export default class GoalBreadcrumbs extends Vue {
  private get breadcrumbs(): { path: string; label: string }[] {
    return breadcrumbStore.breadcrumbs
  }

  private goto(path: string): void {
    breadcrumbStore.goto(path)
  }

  private get breadcrumbsActive(): boolean {
    return breadcrumbStore.breadcrumbs.length > 1
  }
}
</script>

<style lang="scss">
.goal-breadcrumbs {
  padding-top: 5px;
  .el-breadcrumb__item:first-child .el-breadcrumb__inner {
    font-size: 24px !important;
    font-weight: 600 !important;
    color: var(--text);
    transition: 0.2s;
  }
  &.active {
    .el-breadcrumb__item .el-breadcrumb__inner {
      font-size: 15px;
    }
    .el-breadcrumb__separator {
      font-size: 16px;
    }
    .el-breadcrumb__item:first-child .el-breadcrumb__inner {
      font-size: 16px !important;
      font-weight: 700 !important;
      color: unset;
    }
  }
}
</style>
