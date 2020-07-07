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
            el-popover(slot="suffix" placement="bottom" width="500" transition="fade" trigger="click")
              el-button(
                slot="reference"
                type="text"
                size="mini"
                :class="filtersActive ? 'active' : ''")
                feather(type="sliders" size="16")

              el-row(:gutter="10")
                el-col(:span="8")
                  el-select(
                    v-model="filterFields.priority.values"
                    size="small"
                    placeholder="Приоритет"
                    @change="applyFilters"
                    multiple collapse-tags)
                    el-option(v-for="option in priorities" :key="option.value" :value="option.value", :label="option.label")
                el-col(:span="8")
                  el-select(
                    v-model="filterFields.performer.values[0]"
                    size="small"
                    placeholder="Ответственный"
                    :remote-method="searchUsers"
                    @focus="searchUsers('')"
                    @change="applyFilters"
                    filterable remote clearable default-first-option collapse-tags)
                    el-option(v-for="option in userList" :key="option.value" :value="option.value", :label="option.label")
                el-col(:span="8")
                  el-select(
                    v-model="filterFields.project.values[0]"
                    size="small"
                    placeholder="Проект"
                    :remote-method="searchProjects"
                    @focus="searchProjects('')"
                    @change="applyFilters"
                    filterable remote clearable default-first-option collapse-tags)
                    el-option(v-for="option in projectList" :key="option.value" :value="option.value", :label="option.label")
              
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
            placeholder="Описание"
            v-model="projectItem.description"
            v-autowidth="{ maxWidth: '960px', minWidth: '20px', comfortZone: 0 }"
            @change="update")

      page-toolbar
        template(slot="actions")
          transition(name="fade")
            el-button(
              v-if="addButtonVisible"
              size="small"
              @click="dialogVisible = true; dialogData = null")
              feather(type="plus" size="12")
              span Добавить задачу
          transition(name="fade")
            el-button(
              size="small"
              @click="teamDialogVisible = true")
              feather(type="plus" size="12")
              span Добавить команду
          transition(name="fade")
            el-button(
              v-if="completeButtonVisible"
              size="small"
              @click="isMultipleSelected ? onItemMultipleComplete(null, selectedRow) : onItemComplete(null, selectedRow)")
              feather(type="check" size="12")
              span {{ isMultipleSelected ? 'Завершить выделенное' : 'Завершить' }}
          transition(name="fade")
            el-button(
              v-if="deleteButtonVisible"
              size="small"
              @click="isMultipleSelected ? onItemMultipleDelete(null, selectedRow) : onItemDelete(null, selectedRow)")
              feather(type="trash" size="12")
              span {{ isMultipleSelected ? 'Удалить выделенное' : 'Удалить' }}
          transition(name="fade")
            el-button(
              v-if="editButtonVisible && !isMultipleSelected"
              size="small"
              @click="onItemEdit(null, selectedRow)")
              feather(type="edit-3" size="12")
              span Редактировать

        template(slot="view")
          el-button(type="text" size="mini" @click="switchOrder")
            feather(:type="query.sortFields[0].sortType === 'Ascending' ? 'align-left' : 'align-right'" size="20")
          el-select(
            v-model="query.sortFields[0].fieldName"
            size="medium"
            placeholder="По дате создания"
            @change="applySort"
            clearable)
            el-option(v-for="option in sortFields" :key="option.value" :value="option.value", :label="option.label")
          el-button(type="text" size="mini")
            feather(type="grid" size="20")
          el-button.active(type="text" size="mini")
            feather(type="list" size="20")

      el-tabs(ref="tabs" v-model="activeTab" @tab-click="onTabClick")
        el-tab-pane(v-for="(tab, index) in statuses" :key="index" :label="tab.label" :name="tab.value")
          table-content
            el-table(
              v-if="tab.value === activeTab"
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
              el-table-column(prop="title" label="Задача" width="500")
              el-table-column(prop="projectName" label="Проект")
              //el-table-column(prop="state" label="Статус" width="120" :formatter="stateFormatter")
              el-table-column(prop="priority" label="Приоритет" width="120" :formatter="priorityFormatter")
              el-table-column(prop="creationDate" label="Добавлено" width="170" :formatter="dateFormatter")
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

    task-dialog(v-if="dialogVisible" :data="dialogData" @close="dialogVisible = false" @submit="refresh")
    team-dialog(v-if="teamDialogVisible" @close="teamDialogVisible = false" @submit="refresh")

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
        sortFields: [{ fieldName: 'creationDate', sortType: 'Descending' }],
        filterFields: [{ fieldName: 'state', values: ['New'] }]
      },
      sortFields: [
        { value: 'creationDate', label: 'По дате создания' },
        { value: 'name', label: 'По названию' },
        { value: 'projectName', label: 'По проекту' }
      ],
      filterFields: {
        state: { fieldName: 'state', values: [] },
        priority: { fieldName: 'priority', values: [] },
        performer: { fieldName: 'performerFio', values: [] },
        project: { fieldName: 'projectName', values: [] }
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
      activeTab: 'New',
      inputVisible: false,
      inputValue: '',
      teamDialogVisible: false
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
      this.query.filterFields[0].values[0] = tab.name;
      this.refresh();
    },
    async applyFilters() {
      this.query.filterFields = [];
      if (this.filterFields.priority.values.length)
        this.query.filterFields.push({
          fieldName: this.filterFields.priority.fieldName,
          values: this.filterFields.priority.values
        });
      if (this.filterFields.performer.values.length)
        this.query.filterFields.push({
          fieldName: this.filterFields.performer.fieldName,
          values: this.filterFields.performer.values
        });
      if (this.filterFields.project.values.length)
        this.query.filterFields.push({
          fieldName: this.filterFields.project.fieldName,
          values: this.filterFields.project.values
        });

      this.filtersActive = !!this.query.filterFields.filter(
        field => !!field.values[0]
      ).length;

      this.query.filterFields.push({
        fieldName: 'state',
        values: [this.activeTab]
      });

      if (this.query.filterFields.length) await this.refresh();
    },
    async update(e) {
      await this.updateProject(this.projectItem);
    },
    async quickAction(command) {
      switch (command) {
        case 'addTask':
          this.dialogData = null;
          this.dialogVisible = true;
          break;
        case 'addTeam':
          this.teamDialogVisible = true;
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
