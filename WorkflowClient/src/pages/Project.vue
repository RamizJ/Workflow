<template lang="pug">
  page
    page-content
      page-header
        template(slot="title")
          input.title(
            placeholder="Заголовок"
            v-model="projectItem.name"
            v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
            @change="update")
        template(slot="search")
          el-input(
            v-model="query.filter"
            size="medium"
            placeholder="Поиск"
            @change="refresh")
            el-button(slot="prefix" type="text" size="mini")
              feather(type="search" size="16")
            el-button(slot="suffix" type="text" size="mini" :class="filtersVisible ? 'active' : ''" @click="filtersVisible = !filtersVisible")
              feather(type="sliders" size="16")
        template(slot="action")
          el-button(size="mini" @click="onDelete")
            feather(type="trash-2" size="14")
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

      el-tabs(ref="tabs" v-model="activeTab" @tab-click="onTabClick")
        el-tab-pane(v-for="(tab, index) in tabs" :key="index" :label="tab.label" :name="tab.name")
          page-toolbar
            template(v-if="filtersVisible" slot="filters")
              el-row
                el-select(
                  v-model="filters.performer.value"
                  size="small"
                  placeholder="Ответственный"
                  @change="refresh"
                  multiple collapse-tags)
                  el-option(v-for="option in filters.performer.items" :key="option.value" :value="option.value", :label="option.label")
                el-select(
                  v-model="filters.priority.value"
                  size="small"
                  placeholder="Приоритет"
                  @change="refresh"
                  multiple collapse-tags)
                  el-option(v-for="option in filters.priority.items" :key="option.value" :value="option.value", :label="option.label")
                el-select(
                  v-model="filters.status.value"
                  size="small"
                  placeholder="Статус"
                  @change="refresh"
                  multiple collapse-tags)
                  el-option(v-for="option in filters.status.items" :key="option.value" :value="option.value", :label="option.label")
              el-row
                el-select(
                  v-model="filters.project.value"
                  size="small"
                  placeholder="Проект"
                  @change="refresh"
                  multiple collapse-tags)
                  el-option(v-for="option in filters.project.items" :key="option.value" :value="option.value", :label="option.label")
                el-select(
                  v-model="filters.tags.value"
                  size="small"
                  placeholder="Тег"
                  @change="refresh"
                  multiple collapse-tags)
                  el-option(v-for="option in filters.tags.items" :key="option.value" :value="option.value", :label="option.label")

            template(slot="actions")
              el-button(size="mini" @click="dialogOpened = true; selectedItemId = null")
                feather(type="plus" size="16")
              el-button(size="mini")
                feather(type="edit-3" size="16")
              el-button(size="mini")
                feather(type="check" size="16")
              el-button(size="mini")
                feather(type="trash" size="16")

            template(slot="view")
              el-select(
                v-model="filters.sort.value"
                size="medium"
                placeholder="По дате создания"
                @change="refresh"
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

          table-content
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
import PageToolbar from '~/components/PageToolbar';
import PageContent from '~/components/PageContent';
import TableContent from '~/components/TableContent';
import TaskDialog from '~/components/TaskDialog';
import TeamDialog from '~/components/TeamDialog';
import tableMixin from '~/mixins/table.mixin';

export default {
  name: 'Project',
  components: {
    Page,
    PageHeader,
    PageToolbar,
    PageContent,
    TableContent,
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
      filtersVisible: false,
      filters: {
        sort: {
          value: null,
          items: [
            { value: 'title', label: 'Названию' },
            { value: 'goalState', label: 'Статусу' },
            { value: 'scopeId', label: 'Проекту' },
            { value: 'creationDate', label: 'Дате создания' }
          ]
        },
        status: {
          fieldName: 'goalState',
          value: null,
          items: [
            { value: 'New', label: 'Новое' },
            { value: 'Completed', label: 'Завершённое' }
          ]
        },
        performer: {
          fieldName: 'performer',
          value: null,
          items: []
        },
        project: {
          fieldName: 'projectId',
          value: null,
          items: []
        },
        priority: {
          fieldName: 'priority',
          value: null,
          items: [
            { value: 'Low', label: 'Низкий' },
            { value: 'Normal', label: 'Обычный' },
            { value: 'High', label: 'Высокий' }
          ]
        },
        tags: {
          fieldName: 'tag',
          value: null,
          items: []
        }
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
    async onDelete() {
      await this.deleteProject(this.projectItem.id);
      await this.fetchSidebarProjects({ reload: true });
      await this.$router.push({ name: 'Projects' });
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
