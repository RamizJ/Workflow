<template lang="pug">
  table-content
    el-table(
      :data="tableData"
      ref="table"
      height="100%"
      v-loading="loading"
      @select="onItemSelect"
      @row-click="onItemSingleClick"
      @row-contextmenu="onItemRightClick"
      @row-dblclick="onItemDoubleClick"
      @sort-change="onSortChange"
      @filter-change="onFilterChange"
      highlight-current-row border)
      el-table-column(type="selection" width="38")
      el-table-column(prop="name" label="Проект" sortable="custom")
      el-table-column(prop="ownerFio" label="Руководитель" width="250" sortable="custom")
      el-table-column(prop="creationDate" label="Добавлено" width="170" :formatter="dateFormatter" sortable="custom")
      infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="300" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
        div(slot="no-more")
        div(slot="no-results")

    vue-context(ref="contextMenu")
      template(slot-scope="child")
        li(v-if="isEditVisible" @click.prevent="onItemEdit($event, child.data.row)") Редактировать
        li(v-if="isCompleteVisible && !isMultipleSelected" @click.prevent="onItemComplete($event, child.data.row)") Завершить
        li(v-if="isCompleteVisible && isMultipleSelected" @click.prevent="onItemMultipleComplete($event, child.data.row)") Завершить выделенное
        li(v-if="isDeleteVisible && !isMultipleSelected" @click.prevent="onItemDelete($event, child.data.row)") Удалить
        li(v-if="isDeleteVisible && isMultipleSelected" @click.prevent="onItemMultipleDelete($event, child.data.row)") Удалить выделенное
        li(v-if="isRestoreVisible && !isMultipleSelected" @click.prevent="onItemRestore($event, child.data.row)") Восстановить
        li(v-if="isRestoreVisible && isMultipleSelected" @click.prevent="onItemMultipleRestore($event, child.data.row)") Восстановить выделенное

    dialog-project(v-if="dialogVisible" :data="dialogData" @close="dialogVisible = false" @submit="refresh")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import TableContent from '~/components/TableContent';
import DialogProject from '~/components/DialogProject';
import BaseSearch from '~/components/BaseSearch';
import tableMixin from '~/mixins/table.mixin';

export default {
  name: 'TableProjects',
  props: {
    search: {
      type: String
    },
    status: {
      type: String
    }
  },
  components: {
    BaseSearch,
    TableContent,
    DialogProject
  },
  mixins: [tableMixin],
  computed: {
    ...mapGetters({ items: 'projects/getProjects' })
  },
  watch: {
    search() {
      this.query.filter = this.search;
      if (this.query.filter.length > 2) this.refresh();
    }
  },
  mounted() {
    switch (this.status) {
      case 'All':
        this.isEditVisible = true;
        this.isDeleteVisible = true;
        break;
      case 'Completed':
        break;
      case 'Deleted':
        this.isRestoreVisible = true;
        this.query.filterFields.push({
          fieldName: 'isRemoved',
          values: [true]
        });
        this.query.withRemoved = true;
        break;
      default:
        this.isEditVisible = true;
        this.isDeleteVisible = true;
        break;
    }
  },
  methods: {
    ...mapActions({
      fetchItems: 'projects/fetchProjects',
      deleteItem: 'projects/deleteProject',
      deleteItems: 'projects/deleteProjects',
      restoreItem: 'projects/restoreProject',
      restoreItems: 'projects/restoreProjects',
      completeItem: 'projects/completeProject',
      completeItems: 'projects/completeProjects'
    }),
    onItemDoubleClick(row, column, event) {
      this.$router.push(`/projects/${row.id}`);
    }
  }
};
</script>
