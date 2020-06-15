<template lang="pug">
  div.container
    base-header
      template(slot="title") Области
      template(slot="action")
        a(href="#" @click="dialogOpened = true") Создать

    base-toolbar
      template(slot="filters")
        base-toolbar-item
          el-input(v-model="query.filter" size="small" placeholder="Поиск" @change="refresh")
        base-toolbar-item
          el-select(
            v-model="filters.sort.value"
            size="small"
            placeholder="Сортировать"
            @change="applyFilters" clearable)
            el-option(v-for="option in filters.sort.items" :key="option.value" :value="option.value", :label="option.label")

    div.content
      el-table(
        :data="tableData"
        ref="table"
        height="auto"
        v-loading="loading"
        @row-contextmenu="onItemRightClick"
        @row-dblclick="onItemDoubleClick"
        highlight-current-row
        stripe)
        el-table-column(type="selection" width="55")
        el-table-column(prop="name" label="Дата добавления")
        el-table-column(prop="description" label="Заголовок")
        el-table-column(prop="language" label="Статус")
        infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="400" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
          div(slot="no-more")
          div(slot="no-results")

      vue-context(ref="contextMenu")
        template(slot-scope="child")
          li(@click.prevent="onItemEdit($event, child.data.row)") Редактировать
          li Добавить проект
          li Удалить

    scope-dialog(v-if="dialogOpened" :id="selectedItemId" @close="dialogOpened = false")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseHeader from '~/components/BaseHeader';
import BaseToolbar from '~/components/BaseToolbar';
import BaseToolbarItem from '~/components/BaseToolbarItem';
import ScopeDialog from '~/components/ScopeDialog';

export default {
  name: 'Teams',
  components: {
    BaseHeader,
    BaseToolbar,
    BaseToolbarItem,
    ScopeDialog
  },
  data() {
    return {
      loading: false,
      tableData: [],
      query: {
        filter: '',
        pageNumber: 1,
        pageSize: 15
      },
      dialogOpened: false,
      selectedItemId: null,
      filters: {
        sort: {
          value: null,
          items: [
            { value: 'name', label: 'Название' },
            { value: 'creationDate', label: 'Дата создания' }
          ]
        }
      }
    };
  },
  computed: {
    ...mapGetters({ scopes: 'scopes/getScopes' })
  },
  methods: {
    ...mapActions({ fetchScopes: 'scopes/fetchScopes' }),
    async applyFilters() {},
    async refresh() {
      this.tableData = [];
      this.$refs.loader.stateChanger.reset();
    },
    async load($state) {
      const firstLoad = !this.tableData.length;
      if (firstLoad) this.loading = true;
      await this.fetch(this.query);
      if (this.scopes.length) $state.loaded();
      else $state.complete();
      this.tableData = firstLoad
        ? this.scopes
        : this.tableData.concat(this.scopes);
      if (firstLoad) this.loading = false;
    },
    async fetch(params) {
      try {
        await this.fetchScopes(params);
        this.query.pageNumber++;
      } catch (e) {
        this.$message.error('Ошибка получения данных');
      }
    },
    onItemRightClick(row, column, event) {
      this.$refs.contextMenu.open(event, { row, column });
      event.preventDefault();
    },
    onItemDoubleClick(row, column, event) {
      this.onItemEdit(event, row);
    },
    onItemEdit(event, row) {
      this.selectedItemId = row.id;
      this.dialogOpened = true;
    }
  }
};
</script>

<style lang="scss" scoped>
.content {
  display: flex;
  position: relative;
  overflow: hidden;
  flex: 1;
  height: 100%;
  padding: 0 30px;
  .el-table {
    overflow: auto;
    position: unset !important;
  }
}
</style>
