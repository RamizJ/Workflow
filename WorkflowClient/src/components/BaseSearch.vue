<template>
  <el-input
    class="base-search"
    v-model="value"
    size="medium"
    :placeholder="placeholder"
    @change="onChange"
  >
    <el-button slot="prefix" type="text" size="mini" @click="onChange">
      <feather type="search" size="16"></feather>
    </el-button>
    <el-popover
      v-if="filters"
      slot="suffix"
      popper-class="filters-panel"
      placement="bottom"
      transition="fade"
      trigger="click"
    >
      <el-button slot="reference" type="text" size="mini">
        <feather type="sliders" size="16"></feather>
      </el-button>
      <slot name="filters" />
    </el-popover>
  </el-input>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'

@Component
export default class BaseSearch extends Vue {
  @Prop() readonly query: string | undefined
  @Prop({ default: 'Поиск' }) readonly placeholder!: string
  @Prop() readonly filters: boolean | undefined

  private value = ''

  private onChange() {
    this.$emit('update:query', this.value)
    this.$emit('change', this.value)
  }
}
</script>
