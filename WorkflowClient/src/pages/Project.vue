<template lang="pug">
  page
    page-header
      template(slot="title")
        input.title(
          placeholder="Заголовок"
          v-model="projectItem.name"
          v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
          @change="update")
      template(slot="action")
        el-dropdown(v-if="projectItem.id" placement="bottom" :show-timeout="0" @command="quickAction")
          feather.actions(type="chevron-down")
          el-dropdown-menu(slot="dropdown")
            el-dropdown-item(command="addTask") Добавить задачу
            el-dropdown-item(command="addTeam") Добавить команду
            //el-dropdown-item(command="addTags") Добавить теги
            //el-dropdown-item(command="addEndDate") Добавить крайний срок
            //el-dropdown-item(command="completeProject") Завершить проект
            el-dropdown-item(command="deleteProject") Удалить проект
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
          placeholder="Заметки"
          v-model="projectItem.description"
          v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
          @change="update")

    page-content
      el-tabs(ref="tabs" v-model="activeTab" @tab-click="onTabClick")
        el-tab-pane(v-for="(tab, index) in tabs" :key="index" :label="tab.label" :name="tab.name")
          base-list
            el-table(
              v-if="tab.name === activeTab"
              :data="tableData"
              ref="table"
              height="100%"
              v-loading="loading"
              @row-contextmenu="onItemRightClick"
              @row-dblclick="onItemDoubleClick"
              highlight-current-row)
              el-table-column(type="selection" width="55")
              el-table-column(prop="title" label="Задача" width="450")
              el-table-column(prop="description" label="Заметки")
              el-table-column(prop="creationDate" label="Добавлено" :formatter="dateFormatter" width="165")
              infinite-loading(slot="append" ref="loader" spinner="waveDots" :distance="300" @infinite="load" force-use-infinite-wrapper=".el-table__body-wrapper")
                div(slot="no-more")
                div(slot="no-results")

    vue-context(ref="contextMenu")
      template(slot-scope="child")
        li(@click.prevent="onItemEdit($event, child.data.row)") Редактировать
        li(v-if="!isMultipleSelected" @click.prevent="onItemComplete($event, child.data.row)") Завершить
        li(v-if="!isMultipleSelected" @click.prevent="onItemDelete($event, child.data.row)") Удалить
        li(v-if="isMultipleSelected" @click.prevent="onItemMultipleComplete($event, child.data.row)") Завершить выделенное
        li(v-if="isMultipleSelected" @click.prevent="onItemMultipleDelete($event, child.data.row)") Удалить выделенное

    task-dialog(v-if="dialogOpened" :id="selectedItemId" @close="dialogOpened = false" @submit="refresh")
    team-dialog(v-if="teamDialogOpened" @close="teamDialogOpened = false" @submit="refresh")

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import Page from '~/components/Page';
import PageHeader from '~/components/PageHeader';
import PageContent from '~/components/PageContent';
import BaseList from '~/components/BaseList';
import TaskDialog from '~/components/TaskDialog';
import TeamDialog from '~/components/TeamDialog';
import tableMixin from '~/mixins/table.mixin';

export default {
  name: 'Project',
  components: {
    Page,
    PageHeader,
    PageContent,
    BaseList,
    TaskDialog,
    TeamDialog
  },
  mixins: [tableMixin],
  data() {
    return {
      query: {
        projectId: this.$route.params.projectId,
        pageNumber: 0,
        pageSize: 20,
        filterFields: [
          {
            fieldName: 'state',
            values: ['New']
          }
        ]
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
      inputVisible: false,
      inputValue: '',
      teamDialogOpened: false,
      tabs: [
        { label: 'Новое', name: 'New' },
        { label: 'В работе', name: 'Perform' },
        { label: 'Отложенное', name: 'Delay' },
        { label: 'Тестируется', name: 'Testing' },
        { label: 'Выполненное', name: 'Succeed' },
        { label: 'Отклонённое', name: 'Rejected' }
      ],
      activeTab: 'New'
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
      deleteProject: 'projects/deleteProject',
      fetchSidebarProjects: 'projects/fetchSidebarProjects',
      fetchItems: 'tasks/fetchTasks',
      searchItems: 'tasks/searchTasks',
      deleteItem: 'tasks/deleteTask',
      deleteItems: 'tasks/deleteTasks',
      completeItem: 'tasks/completeTask',
      completeItems: 'tasks/completeTasks'
    }),
    onTabClick(tab) {
      console.log(tab.name);
      this.query.filterFields[0].values[0] = tab.name;
      this.refresh();
    },
    async update(e) {
      await this.updateProject(this.projectItem);
    },
    async quickAction(command) {
      switch (command) {
        case 'addTask':
          this.selectedItemId = null;
          this.dialogOpened = true;
          break;
        case 'addTeam':
          this.teamDialogOpened = true;
          break;
        case 'deleteProject':
          await this.deleteProject(this.projectItem.id);
          await this.fetchSidebarProjects({ reload: true });
          await this.$router.push({ name: 'Projects' });
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
