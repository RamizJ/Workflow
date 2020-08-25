<template>
  <div class="table-container">
    <el-table
      ref="table"
      height="100%"
      v-loading="loading"
      :data="tableData"
      :row-class-name="onSetIndex"
      @select="onItemSelect"
      @row-click="onItemSingleClick"
      @row-contextmenu="onItemRightClick"
      @row-dblclick="onItemDoubleClick"
      highlight-current-row="highlight-current-row"
      border="border"
    >
      <el-table-column type="selection" width="38"></el-table-column>
      <el-table-column prop="lastName" label="Фамилия" width="150"></el-table-column>
      <el-table-column prop="firstName" label="Имя" width="150"></el-table-column>
      <el-table-column prop="middleName" label="Отчество" width="150"></el-table-column>
      <el-table-column prop="userName" label="Логин" width="150"></el-table-column>
      <el-table-column prop="email" label="Почта"></el-table-column>
      <el-table-column prop="phone" label="Телефон" width="120"></el-table-column>
      <el-table-column prop="position" label="Должность" width="180"></el-table-column>
      <infinite-loading
        slot="append"
        ref="loader"
        spinner="waveDots"
        :distance="300"
        @infinite="load"
        force-use-infinite-wrapper=".el-table__body-wrapper"
      >
        <div slot="no-more"></div>
        <div slot="no-results"></div>
      </infinite-loading>
    </el-table>
    <vue-context ref="contextMenu">
      <template slot-scope="child">
        <li>
          <a v-if="isEditVisible" @click.prevent="onItemEdit($event, child.data.row)">Изменить</a>
        </li>
        <el-divider v-if="isEditVisible"></el-divider>
        <li><a @click.prevent="onItemCreate">Новый пользователь</a></li>
        <el-divider></el-divider>
        <li>
          <a
            v-if="isRemoveFromTeamVisible"
            @click.prevent="onItemRemoveFromTeam($event, child.data.row)"
            >Убрать из команды</a
          >
        </li>
        <li>
          <a v-if="isDeleteVisible" @click.prevent="onItemDelete($event, child.data.row)"
            >Переместить в корзину</a
          >
        </li>
        <li>
          <a v-if="isRestoreVisible" @click.prevent="onItemRestore($event, child.data.row)"
            >Восстановить</a
          >
        </li>
      </template>
    </vue-context>
    <user-dialog
      v-if="dialogVisible"
      :data="dialogData"
      @close="dialogVisible = false"
      @submit="refresh"
    ></user-dialog>
  </div>
</template>

<script>
// import tableMixin from '@/mixins/table.mixin';
import UserDialog from '@/components/User/UserDialog'

export default {
  name: 'UserList',
  components: { UserDialog },
  // mixins: [tableMixin],
  data() {
    return {
      getters: {
        items: this.$route.params.teamId ? 'teams/getTeamUsers' : 'users/getUsers'
      },
      actions: {
        fetchItems: this.$route.params.teamId ? 'teams/findUsers' : 'users/findAll',
        deleteItem: 'users/deleteOne',
        deleteItems: 'users/deleteMany',
        restoreItem: 'users/restoreOne',
        restoreItems: 'users/restoreMany',
        removeFromTeam: 'teams/removeUser'
      }
    }
  },
  computed: {
    isRemoveFromTeamVisible() {
      return !!this.$route.params.teamId
    }
  },
  methods: {
    async onItemRemoveFromTeam(event, row) {
      const teamId = this.$route.params.teamId
      const userId = row.id
      if (this.isMultipleSelected) {
        const userIds = this.table.selection.map(item => item.id)
        for (const id of userIds) {
          await this.$store.dispatch(this.actions.removeFromTeam, {
            teamId,
            userId: id
          })
        }
      } else
        await this.$store.dispatch(this.actions.removeFromTeam, {
          teamId,
          userId
        })
      this.refresh()
    }
  }
}
</script>
