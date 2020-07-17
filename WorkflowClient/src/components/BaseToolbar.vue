<template lang="pug">
  div.toolbar
    div.toolbar-left
      div.toolbar__search
        base-search(:query.sync="search" filters @change="onSearch")
          template(slot="filters")
            //el-button(type="text" size="mini")
              feather(type="grid" size="20")
      div.toolbar__sort
        el-button(type="text" size="mini" @click="onOrderChange")
          feather(:class="order" type="bar-chart" size="20")
        el-select(
          v-model="sort"
          size="small"
          placeholder="По умолчанию"
          @change="onSortChange")
          el-option(v-for="option in sortFields" :key="option.value" :value="option.value", :label="option.label")
    div.toolbar-right
      div.toolbar__view
        el-button(type="text" size="mini" :class="view === 'board' ? 'active' : ''" @click="onViewChange('board')" disabled)
          feather(type="columns" size="20")
        el-button(type="text" size="mini" :class="view === 'grid' ? 'active' : ''" @click="onViewChange('grid')" disabled)
          feather(type="grid" size="20")
        el-button(type="text" size="mini" :class="view === 'list' ? 'active' : ''" @click="onViewChange('list')")
          feather(type="menu" size="20")
</template>

<script>
import BaseSearch from '~/components/BaseSearch';

export default {
  name: 'BaseToolbar',
  components: { BaseSearch },
  props: {
    sortFields: Array
  },
  data() {
    return {
      search: this.$route.query.q || '',
      order: this.$route.query.order || 'Descending',
      sort: this.$route.query.sort || '',
      view: this.$route.query.view || 'list'
    };
  },
  methods: {
    onSearch() {
      this.$emit('search', this.search);
    },
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
.toolbar {
  padding: 5px 0 15px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.toolbar-left {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}
.toolbar__sort,
.toolbar__view {
  display: flex;
  justify-content: flex-end;
  align-items: flex-start;
  .Ascending {
    transform: rotate(90deg);
    margin-top: -1px;
  }
  .Descending {
    transform: rotate(270deg) scaleY(-1);
  }
  .el-select {
    width: 150px;
    .el-input__inner {
      padding: 0 5px;
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
}
</style>
