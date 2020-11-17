<template>
  <vue-context ref="vueContext">
    <template slot-scope="child">
      <slot :data="child.data" />
    </template>
  </vue-context>
</template>

<script lang="ts">
import { Component, Ref, Vue } from 'vue-property-decorator'
import { ContextMenu } from '@/core/types/context-menu.type'
import settingsModule from '@/modules/settings/store/settings.store'
import { MessageBox } from 'element-ui'

@Component
export default class BaseContextMenu extends Vue {
  @Ref() readonly vueContext!: ContextMenu

  public open(event: Event, data: any): void {
    this.vueContext.open(event, data)
  }

  public async confirmDelete(): Promise<boolean> {
    if (!settingsModule.confirmDelete) return true
    try {
      await MessageBox.confirm('Вы действительно хотите удалить элемент?', 'Предупреждение', {
        confirmButtonText: 'Удалить',
        cancelButtonText: 'Отменить',
        type: 'warning',
      })
      return true
    } catch (e) {
      return false
    }
  }
}
</script>

<style lang="scss">
@import '~vue-context/src/sass/vue-context';

.v-context {
  min-width: 210px;
  &,
  & ul {
    background: var(--popover-background);
    padding: 5px 0;
    border-radius: 5px;
    border: var(--popover-border);
    box-shadow: var(--popover-shadow);
    overflow-y: unset;
    > li > a {
      padding: 9px 12px;
      margin: 0 5px;
      font-size: 14px;
      cursor: pointer;
      border-radius: 4px;
      transition: background-color 0.05s;
      color: var(--text);
      &:hover {
        color: var(--text);
        background-color: var(--dropdown-item-hover);
      }
    }
    > li > span {
      display: inline-block;
      height: 29px;
      padding: 8px 12px;
      margin: 0 5px;
      font-size: 13px;
      cursor: pointer;
      border-radius: 4px;
      transition: background-color 0.08s;
      color: var(--text);
      &:hover {
        color: var(--text);
        background-color: var(--dropdown-item-hover);
      }
      a {
        color: var(--text);
      }
    }
  }
  &__sub {
    > a::before,
    > a::after {
      float: unset;
      padding-left: 0;
      border-right: 1px solid;
      content: '';
      display: block;
      height: 8px;
      margin-top: -6px;
      position: absolute;
      transform: rotate(135deg);
      right: 22px;
      top: 50%;
      width: 0;
    }
    > a::after {
      margin-top: -1px;
      transform: rotate(45deg);
    }
    > ul {
      margin-top: -3px;
      margin-left: 10px;
    }
  }
  .el-divider {
    margin: 4px 16px;
    width: auto;
  }
}
</style>
