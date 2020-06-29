<template lang="pug">
  div.container
    base-header
      template(slot="title") Команды
      template(slot="action")
        a(href="#" @click="dialogOpened = true; selectedItemId = null") Создать

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
        base-toolbar-item
          el-select(v-model="filters.status.value" size="small" placeholder="Участник")
            el-option(v-for="option in filters.status.items" :key="option.value" :value="option.value", :label="option.label")
        base-toolbar-item
          el-select(v-model="filters.performer.value" size="small" placeholder="Проект")
            el-option(v-for="option in filters.performer.items" :key="option.value" :value="option.value", :label="option.label")

    base-list
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
        el-table-column(prop="name" label="Название")
        infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="300" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
          div(slot="no-more")
          div(slot="no-results")

      vue-context(ref="contextMenu")
        template(slot-scope="child")
          li Добавить участника
          li(@click.prevent="onItemEdit($event, child.data.row)") Редактировать
          li(v-if="!isMultipleSelected" @click.prevent="onItemDelete($event, child.data.row)") Удалить
          li(v-if="isMultipleSelected" @click.prevent="onItemMultipleDelete($event, child.data.row)") Удалить выделенное

    team-dialog(v-if="dialogOpened" :id="selectedItemId" @close="dialogOpened = false" @submit="refresh")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseHeader from '~/components/BaseHeader';
import BaseToolbar from '~/components/BaseToolbar';
import BaseToolbarItem from '~/components/BaseToolbarItem';
import BaseList from '~/components/BaseList';
import TeamDialog from '~/components/TeamDialog';
import tableMixin from '~/mixins/table.mixin';

export default {
  name: 'Teams',
  components: {
    BaseHeader,
    BaseToolbar,
    BaseToolbarItem,
    BaseList,
    TeamDialog
  },
  mixins: [tableMixin],
  data() {
    return {
      query: {
        filter: '',
        pageNumber: 0,
        pageSize: 30
      },
      filters: {
        sort: {
          value: null,
          items: [
            { value: 'name', label: 'Название' },
            { value: 'groupName', label: 'Область' },
            { value: 'creationDate', label: 'Дата создания' }
          ]
        },
        status: {
          value: null,
          field: 'goalState',
          items: [
            { value: 0, label: 'Новое' },
            { value: 1, label: 'Завершённое' }
          ]
        },
        performer: {
          value: null,
          field: 'performer',
          items: []
        },
        project: {
          value: null,
          field: 'project',
          items: []
        },
        priority: {
          value: null,
          field: 'priority',
          items: [
            { value: 0, label: 'Низкий' },
            { value: 1, label: 'Средний' },
            { value: 2, label: 'Высокий' }
          ]
        },
        tag: {
          value: null,
          field: 'tag',
          items: []
        }
      },
      value: ''
    };
  },
  computed: {
    ...mapGetters({ items: 'teams/getTeams' })
  },
  methods: {
    ...mapActions({
      fetchItems: 'teams/fetchTeams',
      deleteItem: 'teams/deleteTeam',
      deleteItems: 'teams/deleteTeams'
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
