<template lang="pug">
  div.toolbar-view
    div.toolbar-view__sort
      el-button(type="text" size="mini" @click="onOrderChange")
        //feather(:class="order" type="bar-chart" size="16")
        feather(:class="order" type="code" size="13")
      el-select(
        v-model="sort"
        size="small"
        placeholder="По умолчанию"
        @change="onSortChange")
        el-option(v-for="option in sortFields" :key="option.value" :value="option.value", :label="option.label")
    div.toolbar-view__display
      el-button(type="text" size="mini" :class="view === 'board' ? 'active' : ''" @click="onViewChange('board')")
        feather(type="columns" size="18")
      //el-button(type="text" size="mini" :class="view === 'grid' ? 'active' : ''" @click="onViewChange('grid')")
        feather(type="grid" size="18")
      el-button(type="text" size="mini" :class="view === 'list' ? 'active' : ''" @click="onViewChange('list')")
        feather(type="menu" size="18")
</template>

<script>
export default {
  name: 'ToolbarView',
  props: {
    sortFields: Array
  },
  data() {
    return {
      order: this.$route.query.order || 'Descending',
      sort: this.$route.query.sort || '',
      view: this.$route.query.view || 'list'
    };
  },
  methods: {
    onOrderChange() {
      const value = this.order === 'Ascending' ? 'Descending' : 'Ascending';
      this.order = value;
      this.$emit('order', value);
    },
    onSortChange(value) {
      this.$emit('sort', value);
    },
    onViewChange(value) {
      this.view = value;
      this.$emit('view', value);
    }
  }
};
</script>

<style lang="scss">
.toolbar-view {
  width: 100%;
  display: flex;
  justify-content: flex-end;
  align-items: center;
  margin-top: 20px;
}
.toolbar-view__sort,
.toolbar-view__display {
  display: flex;
  justify-content: flex-end;
  align-items: flex-start;
  .Ascending {
    transform: rotate(90deg);
    margin-top: 0px;
    margin-right: -5px;
  }
  .Descending {
    transform: rotate(270deg) scaleY(-1);
    margin-top: 1px;
    margin-right: -5px;
  }
  .el-select {
    width: 150px;
    margin-bottom: 3px;
    .el-input__inner {
      padding: 0 5px;
      font-size: 14px;
      line-height: 21px;
      height: 21px;
      font-weight: 500;
    }
    .el-input__suffix {
      display: none;
    }
  }
  .el-input {
    i {
      color: var(--text-muted) !important;
    }
  }
  .el-input__inner {
    color: var(--text-muted) !important;
    font-weight: 500;
    background-color: transparent !important;
    height: 21px;
    line-height: 21px;
    font-size: 13.5px;
    &::placeholder {
      color: var(--text-muted) !important;
      opacity: 1;
    }
  }
  .el-button {
    width: 21px;
    height: 21px;
    padding: 0;
    color: var(--text-muted);
    &.is-disabled {
      opacity: 0.3;
    }
    &.active {
      color: var(--text-placeholder);
    }
  }
  .el-button + .el-button {
    margin-left: 5px;
  }
}
</style>
