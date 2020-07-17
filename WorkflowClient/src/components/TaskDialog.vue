<template lang="pug">
  base-dialog(v-if="visible" @close="exit" ref="dialog")
    h1(slot="title") Задача
    el-form(slot="body" :model="form" :rules="rules" ref="form" v-loading="loading" @submit.native.prevent="submit")
      el-row(:gutter="20")
        el-col(:span="$route.params.projectId ? 15 : 24")
          el-form-item(prop="title")
            el-input(ref="title" v-model="form.title" placeholder="Новая задача" @keyup.enter.native="submit")
        el-col(v-if="!$route.params.projectId" :span="12")
          el-form-item(prop="projectId")
            el-select(
              v-model="form.projectId"
              placeholder="Проект"
              :remote-method="searchProjects"
              filterable remote clearable default-first-option)
              el-option(v-for="item in projectList" :key="item.id" :label="item.value" :value="item.id")
        el-col(v-if="priorityVisible || form.priority" :span="$route.params.projectId ? 9 : 12")
          el-form-item(prop="priority")
            el-select(
              v-model="form.priority"
              placeholder="Приоритет")
              el-option(v-for="item in priorities" :key="item.value" :label="item.label" :value="item.value")
      el-row(:gutter="20")
        transition(name="fade")
          el-col(v-if="descriptionVisible || form.description" :span="24")
            el-form-item(prop="description")
              el-input(v-model="form.description" :autosize="{ minRows: 2 }" type="textarea" placeholder="Описание")
        transition(name="fade")
          el-col(v-if="checklistVisible || form.childGoalIds" :span="24")
            el-form-item
              el-card.checklist(shadow="never" :body-style="{ padding: '0px 10px' }")
                el-input(
                  v-model="checklistNewItem"
                  placeholder="Новый пункт"
                  @keyup.enter.native="addToChecklist")
                  el-button(slot="prefix" type="text" size="mini" @click="addToChecklist")
                    feather(type="plus" size="18")
                el-checkbox(
                  v-for="(checklistItem, index) in checklist"
                  :key="index"
                  v-model="checklist[index].checked") {{ checklistItem.title }}
        transition(name="fade")
          el-col(v-if="performerVisible || form.performerId" :span="7")
            el-form-item(prop="performerId")
              el-select(
                v-model="form.performerId"
                placeholder="Ответственный"
                :remote-method="searchUsers"
                filterable remote clearable default-first-option)
                el-option(v-for="item in userList" :key="item.id" :label="item.value" :value="item.id")
        transition(name="fade")
          el-col(v-if="expectedCompletedDateVisible || form.expectedCompletedDate" :span="8")
            el-form-item(prop="expectedCompletedDate")
              el-date-picker(
                type="datetime"
                v-model="form.expectedCompletedDate"
                prefix-icon="el-icon-arrow-down"
                format="dd.MM.yyyy HH:mm"
                default-time="12:00:00"
                placeholder="Крайний срок")
        transition(name="fade")
          el-col(v-if="attachmentsVisible || attachmentList.length" :span="24")
            el-form-item
              el-upload(
                action="https://demo.girngm.ru/workflow_dev/api/Goals/AddAttachments/"
                ref="upload"
                :http-request="uploadAttachment"
                :on-preview="onAttachmentClick"
                :on-remove="removeAttachment"
                :file-list="attachmentList"
                :auto-upload="false"
                drag multiple)
                i.el-icon-upload
                div.el-upload__text
                  em Выберите файл
                  span  или перетащите его сюда

    template(slot="footer")
      div.extra
        el-tooltip(content="Описание" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(v-if="!form.description" type="text" @click="descriptionVisible = !descriptionVisible" circle)
            feather(type="align-left")
        //el-tooltip(content="Чек-лист" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(v-if="!form.childGoalIds" type="text" @click="checklistVisible = !checklistVisible" circle)
            feather(type="check-square")
        el-tooltip(content="Приоритет" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(v-if="!form.priority" type="text" @click="priorityVisible = !priorityVisible" circle)
            feather(type="zap")
        el-tooltip(content="Ответственный" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(v-if="!form.performerId" type="text" @click="performerVisible = !performerVisible" circle)
            feather(type="user")
        el-tooltip(content="Крайний срок" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(v-if="!form.expectedCompletedDate" type="text" @click="expectedCompletedDateVisible = !expectedCompletedDateVisible" circle)
            feather(type="calendar")
        el-tooltip(content="Вложения" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(v-if="!attachmentList.length" type="text" @click="attachmentsVisible = !attachmentsVisible" circle)
            feather(type="paperclip")
      div.send
        el-tooltip(content="Сохранить" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(type="text" @click="submit" circle)
            feather(type="arrow-right")
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseDialog from '~/components/BaseDialog';
import dialogMixin from '~/mixins/dialog.mixin';

export default {
  components: { BaseDialog },
  mixins: [dialogMixin],
  data() {
    return {
      form: {
        title: '',
        description: '',
        projectId: this.$route.params.projectId
          ? parseInt(this.$route.params.projectId)
          : null,
        projectName: null,
        creationDate: new Date(),
        expectedCompletedDate: null,
        state: 'New',
        priority: 'Normal',
        ownerId: null,
        ownerFio: null,
        performerId: null,
        performerFio: null,
        observerIds: []
      },
      checklistNewItem: '',
      checklist: [],
      rules: {
        title: [{ required: true, message: '!', trigger: 'blur' }],
        performerId: [{ required: true, message: '!', trigger: 'blur' }],
        projectId: [{ required: true, message: '!', trigger: 'blur' }]
      },
      priorities: [
        { value: 'High', label: 'Высокий приоритет' },
        { value: 'Normal', label: 'Средний приоритет' },
        { value: 'Low', label: 'Низкий приоритет' }
      ],
      attachmentList: [],
      descriptionVisible: null,
      checklistVisible: null,
      priorityVisible: null,
      performerVisible: null,
      expectedCompletedDateVisible: null,
      attachmentsVisible: null
    };
  },
  computed: {
    ...mapGetters({
      task: 'tasks/getTask',
      me: 'auth/me',
      attachments: 'tasks/getTaskAttachments'
    })
  },
  async mounted() {
    this.loading = true;
    if (this.isEdit) {
      await this.loadChecklist();
      await this.fetchAttachments(this.form.id);
      this.attachmentList = this.attachments.map(attachment => {
        return {
          name: attachment.fileName,
          id: attachment.id
        };
      });
    } else {
      this.attachmentList = [];
    }
    await this.searchUsers();
    await this.searchProjects();
    this.loading = false;
    this.$refs.title?.focus();
  },
  methods: {
    ...mapActions({
      fetchItem: 'tasks/fetchTask',
      createItem: 'tasks/createTask',
      updateItem: 'tasks/updateTask',
      fetchChildTasks: 'tasks/fetchChildTasks',
      addChildTasks: 'tasks/addChildTasks',
      fetchAttachments: 'tasks/fetchAttachments',
      addAttachments: 'tasks/addAttachments',
      removeAttachments: 'tasks/removeAttachments',
      downloadAttachment: 'tasks/downloadAttachment'
    }),
    async submit() {
      if (this.isFormValid) {
        await this.sendForm();
        await this.saveChecklist();
        this.$refs.upload?.submit();
        this.$emit('submit');
        this.exit();
      }
    },
    async loadChecklist() {
      if (!this.form.childGoalIds) return;
      const childTasks = await this.fetchChildTasks(this.form.id);
      this.checklist = childTasks.map(task => {
        task.checked = task.state === 'Succeed';
        return task;
      });
    },
    async addToChecklist() {
      if (!this.checklistNewItem) return;
      this.checklist.unshift({
        title: this.checklistNewItem,
        projectId: this.form.projectId,
        creationDate: new Date(),
        state: 'New',
        parentGoalId: this.form.id,
        checked: false
      });
      this.checklistNewItem = '';
    },
    async saveChecklist() {
      if (!this.checklist.length) return;
      const parentId = this.isEdit ? this.form.id : this.task.id;
      const tasks = this.checklist.filter(item => !item.id);
      await this.addChildTasks({ parentId, tasks });
    },
    async uploadAttachment(request) {
      this.loading = true;
      let files = new FormData();
      files.append('files', request.file);
      await this.addAttachments({
        taskId: this.form.id || this.task.id,
        files
      });
      this.loading = false;
    },
    async onAttachmentClick(file) {
      await this.downloadAttachment(file);
    },
    async removeAttachment(file) {
      await this.removeAttachments([file.id]);
    }
  }
};
</script>

<style lang="scss">
.checklist {
  .el-input__inner,
  .el-input__inner:hover,
  .el-input__inner:focus {
    background-color: transparent;
  }
  .el-input__prefix {
    left: 0;
  }
  .el-input--prefix .el-input__inner {
    padding-left: 25px;
  }
  .el-input .el-button {
    height: 100%;
    padding: 0;
  }
  .el-checkbox {
    margin-left: 2px;
    font-weight: 400;
    width: 100%;
  }
}
</style>
