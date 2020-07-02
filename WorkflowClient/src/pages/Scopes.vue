<template lang="pug">
  page
    page-header
      template(slot="title") Области
      template(slot="search")
        el-input(
          v-model="query.filter"
          size="medium"
          placeholder="Поиск"
          @change="applyFilters")
          el-button(slot="prefix" type="text" size="mini")
            feather(type="search" size="16")
          el-button(slot="suffix" type="text" size="mini" @click="filtersVisible = !filtersVisible")
            feather(type="sliders" size="16")
      template(slot="action")
        el-button(type="text" size="mini" @click="dialogOpened = true; selectedItemId = null")
          feather(type="edit" size="18")

    page-toolbar
      template(v-if="filtersVisible" slot="filters")

      template(slot="view")
        el-select(
          v-model="filters.sort.value"
          size="medium"
          placeholder="По дате создания"
          @change="applyFilters"
          clearable)
          el-button(slot="prefix" type="text" size="mini")
            feather(type="align-right" size="18")
          el-option(v-for="option in filters.sort.items" :key="option.value" :value="option.value", :label="option.label")
          el-divider
          el-option(value="acs" label="Возрастанию")
          el-option(value="desc" label="Убыванию")
        el-button(type="text" size="mini")
          feather(type="grid" size="20")
        el-button.active(type="text" size="mini")
          feather(type="list" size="20")

    page-content
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
        infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="300" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
          div(slot="no-more")
          div(slot="no-results")

      vue-context(ref="contextMenu")
        template(slot-scope="child")
          li(@click.prevent="onItemEdit($event, child.data.row)") Редактировать
          li Добавить проект
          li(@click.prevent="onItemDelete($event, child.data.row)") Удалить

    scope-dialog(v-if="dialogOpened" :id="selectedItemId" @close="dialogOpened = false")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import Page from '~/components/Page';
import PageHeader from '~/components/PageHeader';
import PageToolbar from '~/components/PageToolbar';
import PageContent from '~/components/PageContent';
import ScopeDialog from '~/components/ScopeDialog';
import tableMixin from '~/mixins/table.mixin';

export default {
  name: 'Teams',
  components: {
    Page,
    PageHeader,
    PageToolbar,
    PageContent,
    ScopeDialog
  },
  mixins: [tableMixin],
  data() {
    return {
      query: {
        filter: '',
        pageNumber: 1,
        pageSize: 30
      },
      filtersVisible: false,
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
    ...mapGetters({ items: 'scopes/getScopes' })
  },
  methods: {
    ...mapActions({
      fetchItems: 'scopes/fetchScopes',
      deleteItem: 'scopes/deleteScope'
    }),
    async applyFilters() {}
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
