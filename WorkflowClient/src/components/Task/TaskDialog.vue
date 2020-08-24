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
              el-option(v-for="item in projects" :key="item.id" :label="item.value" :value="item.id")
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
          el-col(v-if="checklistVisible || form.child" :span="24")
            el-form-item
              checklist(:task="form" @change="onChecklistChange")
              //el-card.checklist(shadow="never" :body-style="{ padding: '0px 10px' }")
                el-input(
                  v-model="checklistNewItem"
                  placeholder="Новый пункт"
                  @keyup.enter.native="addToChecklist")
                  el-button(slot="prefix" type="text" size="mini" @click="addToChecklist")
                    feather(type="plus" size="18")
                div.checklist__item(
                  v-for="(checklistItem, index) in checklist"
                  //:key="index"
                  //:class="checklist[index].checked ? 'completed' : 'new'")
                  el-checkbox(v-model="checklist[index].checked")
                  el-input(v-model="checklist[index].title" @keyup.delete.native="onChecklistItemDelete(index)")

        transition(name="fade")
          el-col(v-if="performerVisible || form.performerId" :span="7")
            el-form-item(prop="performerId")
              el-select(
                v-model="form.performerId"
                placeholder="Ответственный"
                :remote-method="searchUsers"
                filterable remote clearable default-first-option)
                el-option(v-for="item in users" :key="item.id" :label="item.value" :value="item.id")
        transition(name="fade")
          el-col(v-if="expectedCompletedDateVisible || form.expectedCompletedDate" :span="8")
            el-form-item(prop="expectedCompletedDate")
              el-date-picker(
                v-model="form.expectedCompletedDate"
                :picker-options="{ disabledDate: validateDate }"
                type="datetime"
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
        el-tooltip(content="Чек-лист" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(v-if="!checklistVisible" type="text" @click="checklistVisible = !checklistVisible" circle)
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

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import { ElForm } from 'element-ui/types/form';
import { Input, Message } from 'element-ui';
import { Route } from 'vue-router';

import tasksModule from '@/store/modules/tasks.module';
import BaseDialog from '@/components/BaseDialog.vue';
import Task, { Priority, Status } from '@/types/task.type';
import Attachment from '@/types/attachment.type';
import projectsModule from '@/store/modules/projects.module';
import usersModule from '@/store/modules/users.module';
import teamsModule from '@/store/modules/teams.module';
import { ElUpload } from 'element-ui/types/upload';
import Checklist from '@/components/Checklist.vue';

@Component({ components: { Checklist, BaseDialog } })
export default class TaskDialog extends Vue {
  @Prop()
  id: number | undefined;

  // @Prop()
  // data: Task | undefined;

  private loading = false;
  private visible = false;
  private isEdit = !!this.id;
  private form: Task = {
    title: '',
    description: '',
    projectId: parseInt(this.$route.params.projectId) || undefined,
    projectName: '',
    performerId: '',
    performerFio: '',
    creationDate: new Date(),
    state: Status.New,
    priority: Priority.Normal,
    isChildsExist: false,
    isRemoved: false
  };

  private attachmentList: any[] | undefined = [];
  private childTasks: Task[] = [];
  private checklist: Task[] = [];
  private checklistNewItem = '';

  private rules = {
    title: [{ required: true, message: '!', trigger: 'blur' }],
    performerId: [{ required: true, message: '!', trigger: 'blur' }],
    projectId: [{ required: true, message: '!', trigger: 'blur' }]
  };
  private priorities = [
    { value: Priority.High, label: 'Высокий приоритет' },
    { value: Priority.Normal, label: 'Средний приоритет' },
    { value: Priority.Low, label: 'Низкий приоритет' }
  ];

  private descriptionVisible = null;
  private checklistVisible = null;
  private priorityVisible = null;
  private performerVisible = null;
  private expectedCompletedDateVisible = null;
  private attachmentsVisible = null;

  private get projects() {
    return projectsModule.projects.map(project => {
      return {
        value: project.name,
        id: project.id
      };
    });
  }

  private get teams() {
    return teamsModule.teams.map(team => {
      return {
        value: team.name,
        id: team.id
      };
    });
  }

  private get users() {
    return usersModule.users.map(user => {
      return {
        value: `${user.lastName} ${user.firstName}`,
        id: user.id
      };
    });
  }

  async mounted() {
    this.visible = true;
    // if (this.isEdit) this.form = { ...this.data } as Task;

    this.loading = true;
    if (this.isEdit) this.form = (await tasksModule.findOneById(this.id!)) as Task;
    // if (this.isEdit) {
    //   // await this.loadChecklist();
    //   // const attachments = await tasksModule.findAttachments(this.form.id as number);
    //   this.attachmentList = this.form.attachments?.map(attachment => {
    //     return {
    //       name: attachment.fileName,
    //       id: attachment.id
    //     };
    //   });
    // } else {
    //   this.attachmentList = [];
    // }
    await this.searchUsers();
    await this.searchProjects();
    this.loading = false;
    (this.$refs.title as Input).focus();
  }

  async submit() {
    const form = this.$refs.form as ElForm;
    await form.validate(async valid => {
      if (valid) {
        await this.sendForm();
        // await this.saveChecklist();
        (this.$refs.upload as ElUpload).submit();
        this.$emit('submit');
        this.exit();
      } else {
        Message({
          showClose: true,
          message: 'Форма заполнена некорректно',
          type: 'error'
        });
      }
    });
  }

  async sendForm() {
    this.loading = true;
    const entity: Task = { ...this.form } as Task;
    console.log(entity);
    entity.isChildsExist = !!entity.child?.length;
    entity.isAttachmentsExist = !!entity.attachments?.length;
    if (this.isEdit) await tasksModule.updateOne(entity);
    else await tasksModule.createOne(entity);
    this.loading = false;
  }

  exit() {
    const form = this.$refs.form as ElForm;
    form.resetFields();
    this.visible = false;
    this.$emit('close');
  }

  onChecklistChange(checklist: Task[]) {
    console.log('checklist change');
    this.form.child = checklist;
  }

  // async loadChecklist() {
  //   if (!this.form.isChildsExist) return;
  //   this.loading = true;
  //   const childTasks: Task[] = await tasksModule.findChild(this.form.id as number);
  //   this.childTasks = Array.from(
  //     childTasks.map(task => {
  //       task.completed = task.state === 'Succeed';
  //       return { ...task };
  //     })
  //   );
  //   this.checklist = childTasks.map(task => {
  //     task.completed = task.state === 'Succeed';
  //     return task;
  //   });
  //   this.loading = false;
  // }
  //
  // async addToChecklist() {
  //   if (!this.checklistNewItem) return;
  //   this.checklist.unshift({
  //     title: this.checklistNewItem,
  //     projectId: this.form.projectId,
  //     creationDate: new Date(),
  //     state: Status.New,
  //     parentGoalId: this.form.id,
  //     completed: false
  //   } as Task);
  //   this.checklistNewItem = '';
  // }
  //
  // onChecklistItemDelete(itemIndex: number) {
  //   if (this.checklist[itemIndex].title === undefined) this.checklist.splice(itemIndex, 1);
  //   if (this.checklist[itemIndex] && this.checklist[itemIndex].title === '')
  //     this.checklist[itemIndex].title = undefined;
  // }
  //
  // async saveChecklist() {
  //   if (!this.checklist.length) return;
  //   this.loading = true;
  //
  //   const previousChecklist = this.childTasks;
  //   const currentChecklist = this.checklist
  //     .map(item => {
  //       item.state = item.completed ? Status.Succeed : Status.New;
  //       return item;
  //     })
  //     .reverse();
  //
  //   this.form.isChildsExist = true;
  //
  //   const parentId = this.isEdit ? this.form.id : this.id;
  //   const tasksToCreate = currentChecklist.filter(item => !item.id);
  //   if (tasksToCreate.length)
  //     await tasksModule.addChild({ id: parentId as number, entities: tasksToCreate });
  //
  //   const removedItems = this.getRemovedChecklistItems(previousChecklist, currentChecklist);
  //   await tasksModule.deleteMany(removedItems.map(item => item.id as number));
  //
  //   const changedItems = this.getChangedChecklistItems(previousChecklist, currentChecklist);
  //   await tasksModule.updateMany(changedItems);
  //
  //   this.loading = false;
  // }
  //
  // getRemovedChecklistItems(previousChecklist: Task[], currentChecklist: Task[]) {
  //   const removedItems: Task[] = [];
  //   for (const previousItem of previousChecklist) {
  //     const existingItem = currentChecklist.find(currentItem => currentItem.id === previousItem.id);
  //     if (!existingItem) removedItems.push(previousItem);
  //   }
  //   return removedItems;
  // }
  //
  // getChangedChecklistItems(previousChecklist: Task[], currentChecklist: Task[]) {
  //   const changedItems = [];
  //   for (const previousItem of previousChecklist) {
  //     const changedItem = currentChecklist.find(
  //       currentItem =>
  //         currentItem.id &&
  //         currentItem.id === previousItem.id &&
  //         (currentItem.completed !== previousItem.completed ||
  //           currentItem.title !== previousItem.title)
  //     );
  //     if (changedItem) changedItems.push(changedItem);
  //   }
  //   return changedItems;
  // }

  validateDate(date: Date) {
    const currentDate = new Date();
    currentDate.setDate(currentDate.getDate() - 1);
    return date < currentDate;
  }

  async uploadAttachment(request: any): Promise<void> {
    console.log(typeof request);
    this.loading = true;
    const id = this.id;
    if (!id) return;
    const files = new FormData();
    files.append('files', request.file);
    await tasksModule.uploadAttachments({ id, files });
    this.loading = false;
  }

  async onAttachmentClick(attachment: Attachment) {
    await tasksModule.downloadAttachment(attachment);
  }

  async removeAttachment(attachment: Attachment) {
    if (attachment.id) await tasksModule.removeAttachments([attachment.id]);
  }

  async searchProjects(query = '') {
    await projectsModule.findAll({
      filter: query,
      pageNumber: 0,
      pageSize: 10
    });
  }

  async searchTeams(query = '') {
    await teamsModule.findAll({
      filter: query,
      pageNumber: 0,
      pageSize: 10
    });
  }

  async searchUsers(query = '') {
    await usersModule.findAll({
      filter: query,
      pageNumber: 0,
      pageSize: 10
    });
  }
}

/*export default {
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
        observerIds: [],
        isChildsExist: false
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
      childTasks: [],
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
      fetchItem: 'tasks/findOneById',
      createItem: 'tasks/createOne',
      updateItem: 'tasks/updateOne',
      updateTasks: 'tasks/updateMany',
      deleteTasks: 'tasks/deleteMany',
      fetchChildTasks: 'tasks/findChild',
      addChildTasks: 'tasks/addChild',
      fetchAttachments: 'tasks/findAttachments',
      addAttachments: 'tasks/uploadAttachments',
      removeAttachments: 'tasks/removeAttachments',
      downloadAttachment: 'tasks/downloadAttachment'
    }),
    async submit() {
      await this.$refs.form.validate(async valid => {
        if (valid) {
          await this.sendForm();
          await this.saveChecklist();
          this.$refs.upload?.submit();
          this.$emit('submit');
          this.exit();
        } else {
          this.$message({
            showClose: true,
            message: 'Форма заполнена некорректно',
            type: 'error'
          });
        }
      });
    },
    async loadChecklist() {
      if (!this.form.isChildsExist) return;
      this.loading = true;
      const childTasks = await this.fetchChildTasks(this.form.id);
      this.childTasks = Array.from(childTasks.map(task => {
        task.checked = task.state === 'Succeed';
        return { ...task };
      }));
      this.checklist = childTasks.map(task => {
        task.checked = task.state === 'Succeed';
        return task;
      });
      this.loading = false;
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
    onChecklistItemDelete(itemIndex) {
      if (this.checklist[itemIndex].title === null)
        this.checklist.splice(itemIndex, 1)
      if (this.checklist[itemIndex] && this.checklist[itemIndex].title === '')
        this.checklist[itemIndex].title = null;
    },
    async saveChecklist() {
      if (!this.checklist.length) return;
      this.loading = true;

      const previousChecklist = this.childTasks;
      const currentChecklist = this.checklist.map(item => {
        item.state = item.checked ? 'Succeed' : 'New';
        return item;
      }).reverse();

      this.form.isChildsExist = true;

      const parentId = this.isEdit ? this.form.id : this.task.id;
      const tasksToCreate = currentChecklist.filter(item => !item.id)
      if (tasksToCreate.length)
        await this.addChildTasks({ parentId, tasks: tasksToCreate });

      const removedItems = this.getRemovedChecklistItems(previousChecklist, currentChecklist);
      await this.deleteTasks(removedItems.map(item => item.id));

      const changedItems = this.getChangedChecklistItems(previousChecklist, currentChecklist);
      await this.updateTasks(changedItems);

      this.loading = false;
    },
    getRemovedChecklistItems(previousChecklist, currentChecklist) {
      let removedItems = [];
      for (let previousItem of previousChecklist) {
        const existingItem = currentChecklist.find(currentItem => currentItem.id === previousItem.id)
        if (!existingItem)
          removedItems.push(previousItem);
      }
      return removedItems;
    },
    getChangedChecklistItems(previousChecklist, currentChecklist) {
      let changedItems = [];
      for (let previousItem of previousChecklist) {
        const changedItem = currentChecklist.find(currentItem =>
          currentItem.id &&
          (currentItem.id === previousItem.id) &&
          ((currentItem.checked !== previousItem.checked) || (currentItem.title !== previousItem.title))
        )
        if (changedItem)
          changedItems.push(changedItem);
      }
      return changedItems;
    },
    validateDate(date) {
      const currentDate = new Date();
      currentDate.setDate(currentDate.getDate() - 1);
      return date < currentDate;
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
      if (file.id) await this.removeAttachments([file.id]);
    }
  }
};*/
</script>

<style lang="scss">
.checklist {
  &__item {
    display: flex;
    .el-input__inner {
      padding-left: 9px;
    }
    &.completed .el-input__inner {
      text-decoration: line-through;
    }
    &:last-child {
      margin-bottom: 2px;
    }
  }
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
    color: var(--text);
    margin-left: 2px;
    font-weight: 400;
    width: 14px;
  }
}
</style>
