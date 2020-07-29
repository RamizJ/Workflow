<template lang="pug">
  div.table-container
    el-table(
      :data="tableData"
      ref="table"
      height="100%"
      v-loading="loading"
      @select="onItemSelect"
      @row-click="onItemSingleClick"
      @row-contextmenu="onItemRightClick"
      @row-dblclick="onItemDoubleClick"
      highlight-current-row border)
      el-table-column(type="selection" width="38")
      el-table-column(prop="name" label="Команда")
      infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="300" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
        div(slot="no-more")
        div(slot="no-results")

    vue-context(ref="contextMenu")
      template(slot-scope="child")
        li
          a(v-if="isEditVisible" @click.prevent="onItemDoubleClick(child.data.row)") Открыть
        li
          a(v-if="isEditVisible" @click.prevent="onItemEdit($event, child.data.row)") Изменить
        el-divider(v-if="isEditVisible")
        li
          a(@click.prevent="onItemCreate") Новая команда
        el-divider
        li
          a(v-if="isDeleteVisible" @click.prevent="onItemDelete($event, child.data.row)") Переместить в корзину
        li
          a(v-if="isRestoreVisible" @click.prevent="onItemRestore($event, child.data.row)") Восстановить

    team-dialog(v-if="dialogVisible" :data="dialogData" @close="dialogVisible = false" @submit="refresh")

</template>

<script>
import tableMixin from '@/mixins/table.mixin';
import TeamDialog from '@/components/Teams/TeamDialog';

export default {
  name: 'TeamList',
  components: { TeamDialog },
  mixins: [tableMixin],
  data() {
    return {
      getters: {
        items: 'teams/getTeams'
      },
      actions: {
        fetchItems: this.$route.params.projectId
          ? 'projects/fetchProjectTeams'
          : 'teams/fetchTeams',
        deleteItem: 'teams/deleteTeam',
        deleteItems: 'teams/deleteTeams',
        restoreItem: 'teams/restoreTeam',
        restoreItems: 'teams/restoreTeams'
      }
    };
  },
  methods: {
    onItemDoubleClick(row, column, event) {
      if (!row.isRemoved) this.$router.push(`/teams/${row.id}`);
    }
  }
};
</script>
