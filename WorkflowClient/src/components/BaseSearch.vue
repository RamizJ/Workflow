<template lang="pug">
  el-input.base-search(
    v-model="value"
    size="medium"
    :placeholder="placeholder"
    @change="$emit('update:query', value)")
    el-button(slot="prefix" type="text" size="mini" @click="$emit('update:query', value)")
      feather(type="search" size="16")
    el-popover(v-if="filters" slot="suffix" placement="bottom" transition="fade" trigger="click")
      el-button(
        slot="reference"
        type="text"
        size="mini")
        feather(type="sliders" size="16")
      slot(name="filters")

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
    };
  }
};
</script>

<style lang="scss"></style>
