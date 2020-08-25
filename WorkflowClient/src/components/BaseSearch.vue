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

<script>
export default {
  name: 'BaseSearch',
  props: {
    query: String,
    placeholder: { type: String, default: 'Поиск' },
    filters: Boolean
  },
  data() {
    return {
      value: ''
    }
  },
  methods: {
    onChange() {
      this.$emit('update:query', this.value)
      this.$emit('change', this.value)
    }
  }
}
</script>
