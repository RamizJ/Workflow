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
      el-table-column(prop="name" label="Команда" sortable="custom")
      infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="300" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
        div(slot="no-more")
        div(slot="no-results")

    vue-context(ref="contextMenu")
      template(slot-scope="child")
        li(v-if="isEditVisible" @click.prevent="onItemEdit($event, child.data.row)") Изменить
        el-divider(v-if="isEditVisible")
        li(@click.prevent="onItemCreate") Новая команда
        el-divider
        li(
          v-if="isDeleteVisible"
          @click.prevent="onItemDelete($event, child.data.row)") Переместить в корзину
        li(
          v-if="isRestoreVisible"
          @click.prevent="onItemRestore($event, child.data.row)") Восстановить

    dialog-team(v-if="dialogVisible" :data="dialogData" @close="dialogVisible = false" @submit="refresh")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import TableContent from '~/components/TableContent';
import DialogTeam from '~/components/DialogTeam';
import BaseSearch from '~/components/BaseSearch';
import tableMixin from '~/mixins/table.mixin';

export default {
  name: 'TableTeams',
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
    DialogTeam
  },
  mixins: [tableMixin],
  computed: {
    ...mapGetters({ items: 'teams/getTeams' })
  },
  mounted() {
    switch (this.status) {
      case 'All':
        this.isEditVisible = true;
        this.isDeleteVisible = true;
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
      fetchItems: 'teams/fetchTeams',
      deleteItem: 'teams/deleteTeam',
      deleteItems: 'teams/deleteTeams',
      restoreItem: 'teams/restoreTeam',
      restoreItems: 'teams/restoreTeams'
    })
  }
};
</script>
