<template lang="pug">
  div.table-container
    el-table(
      ref="table"
      height="100%"
      v-loading="loading"
      :data="tableData"
      :row-class-name="onSetIndex"
      @select="onItemSelect"
      @row-click="onItemSingleClick"
      @row-contextmenu="onItemRightClick"
      @row-dblclick="onItemDoubleClick"
      highlight-current-row border)
      el-table-column(type="selection" width="38")
      el-table-column(prop="name" label="Проект")
      el-table-column(prop="ownerFio" label="Руководитель" width="250")
      el-table-column(prop="creationDate" label="Дата создания" width="180" :formatter="dateFormatter")
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
          a(@click.prevent="onItemCreate") Новый проект
        el-divider
        li
          a(v-if="isDeleteVisible" @click.prevent="onItemDelete($event, child.data.row)") Переместить в корзину
        li
          a(v-if="isRestoreVisible" @click.prevent="onItemRestore($event, child.data.row)") Восстановить

    project-dialog(v-if="dialogVisible" :data="dialogData" @close="dialogVisible = false" @submit="refresh")

</template>

<script>
import tableMixin from '@/mixins/table.mixin';
import ProjectDialog from '@/components/Project/ProjectDialog';

export default {
  name: 'ProjectList',
  components: { ProjectDialog },
  mixins: [tableMixin],
  data() {
    return {
      getters: {
        items: this.$route.params.teamId
          ? 'teams/getTeamProjects'
          : 'projects/getProjects'
      },
      actions: {
        fetchItems: this.$route.params.teamId
          ? 'teams/findProjects'
          : 'projects/findAll',
        deleteItem: 'projects/deleteOne',
        deleteItems: 'projects/deleteMany',
        restoreItem: 'projects/restoreOne',
        restoreItems: 'projects/restoreMany'
      }
    };
  },
  methods: {
    onItemDoubleClick(row, column, event) {
      if (!row.isRemoved) this.$router.push(`/projects/${row.id}`);
    }
  }
};
</script>
