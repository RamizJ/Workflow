<template lang="pug">
  div.container
    base-header
      template(slot="title")
        input.title(
          v-model="projectItem.name"
          v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
          @change="update")
      template(slot="action")
        el-dropdown(placement="bottom" :show-timeout="0" @command="quickAction")
          el-button.actions(type="text" icon="el-icon-more" circle)
          el-dropdown-menu(slot="dropdown")
            el-dropdown-item(command="addTask") Добавить задачу
            el-dropdown-item(command="addTeam") Добавить команду
            el-dropdown-item(command="addTags") Добавить теги
            el-dropdown-item(command="addEndDate") Добавить крайний срок
            el-dropdown-item(command="complete") Завершить проект
            el-dropdown-item(command="delete") Удалить проект
      template(slot="subtitle")
        div.tags(v-if="projectItem.tags && projectItem.tags.length")
          el-tag.tag(
            :key="tag"
            v-for="tag in projectItem.tags"
            :disable-transitions="false"
            @close="deleteTag(tag)"
            closable) {{ tag }}
          el-input.tag-input(
            v-if="inputVisible"
            v-model="inputValue"
            ref="saveTagInput"
            size="mini"
            @keyup.enter.native="handleInputConfirm"
            @blur="handleInputConfirm")
          el-button.tag-add(v-else size="small" @click="showInput") Добавить тег
        input.subtitle(
          v-model="projectItem.description"
          v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
          @change="update")

    base-list
      el-table(
        :data="tableData"
        ref="table"
        height="100%"
        v-loading="loading"
        @row-contextmenu="onItemRightClick"
        @row-dblclick="onItemDoubleClick"
        highlight-current-row)
        el-table-column(type="selection" width="55")
        el-table-column(prop="title" label="Задача")
        el-table-column(prop="description" label="Заметки")
        el-table-column(prop="creationDate" label="Добавлено" :formatter="dateFormatter")
        infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="300" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
          div(slot="no-more")
          div(slot="no-results")

      vue-context(ref="contextMenu")
        template(slot-scope="child")
          li(@click.prevent="onItemEdit($event, child.data.row)") Редактировать
          li Завершить
          li(@click.prevent="onItemDelete($event, child.data.row)") Удалить

    task-dialog(v-if="dialogOpened" :id="selectedItemId" @close="dialogOpened = false" @submit="refresh")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseHeader from '~/components/BaseHeader';
import BaseList from '~/components/BaseList';
import TaskDialog from '~/components/TaskDialog';
import tableMixin from '~/mixins/table.mixin';

export default {
  name: 'Project',
  components: { BaseHeader, BaseList, TaskDialog },
  mixins: [tableMixin],
  data() {
    return {
      query: {
        projectId: this.$route.params.projectId
      },
      projectItem: {
        name: '',
        description: '',
        tags: [],
        ownerId: null,
        ownerFio: null,
        teamId: null,
        teamName: null,
        groupId: null,
        groupName: null,
        creationDate: new Date()
      },
      tags: ['Работа', 'Важное', 'Разработка'],
      inputVisible: false,
      inputValue: ''
    };
  },
  computed: {
    ...mapGetters({
      project: 'projects/getProject',
      items: 'tasks/getTasks'
    })
  },
  async mounted() {
    await this.fetchProject(this.$route.params.projectId);
    this.projectItem = { ...this.project };
  },
  methods: {
    ...mapActions({
      fetchProject: 'projects/fetchProject',
      updateProject: 'projects/updateProject',
      fetchItems: 'tasks/fetchTasks',
      deleteItem: 'tasks/deleteTask'
    }),
    async update(e) {
      await this.updateProject(this.projectItem);
    },
    quickAction(command) {
      switch (command) {
        case 'addTask':
          this.selectedItemId = null;
          this.dialogOpened = true;
          break;
        default:
          break;
      }
    },
    deleteTag(tag) {
      this.tags.splice(this.tags.indexOf(tag), 1);
    },
    showInput() {
      this.inputVisible = true;
      this.$nextTick(_ => {
        this.$refs.saveTagInput.$refs.input.focus();
      });
    },
    handleInputConfirm() {
      let inputValue = this.inputValue;
      if (inputValue) {
        this.tags.push(inputValue);
      }
      this.inputVisible = false;
      this.inputValue = '';
    }
  }
};
</script>

<style lang="scss" scoped>
.tag {
  border-radius: 20px;
  height: 27px;
  line-height: 25px;
}
.tag + .tag {
  margin-left: 10px;
}
.tag-add {
  border-radius: 20px;
  margin-left: 10px;
  height: 27px;
  line-height: 25px;
  padding-top: 0;
  padding-bottom: 0;
}
.tag-input {
  width: 110px;
  height: 27px;
  margin-left: 10px;
  vertical-align: bottom;
}
</style>
