<template lang="pug">
  base-dialog(v-if="visible" @close="exit")
    div(slot="title") Задача
    div(slot="body")
      el-form(:model="form" :rules="rules" ref="form" v-loading="loading" @submit.native.prevent="submit")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="title")
              el-input(v-model="form.title" size="medium" placeholder="Новая задача")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="description")
              el-input(v-model="form.description" size="medium" type="textarea" placeholder="Заметки")
        el-row(:gutter="20")
          transition(name="fade")
            el-col(v-if="tagsVisible || (form.tags && form.tags.length)" :span="$route.params.projectId ? 24 : 16")
              el-form-item(prop="tags")
                el-select(
                  v-model="form.tags"
                  size="medium"
                  placeholder="Теги"
                  multiple filterable allow-create default-first-option)
          transition(name="fade")
            el-col(v-if="priorityVisible || form.priority" :span="8")
              el-form-item(prop="priority")
                el-select(
                  v-model="form.priority"
                  size="medium"
                  placeholder="Приоритет")
                  el-option(v-for="item in priorities" :key="item.value" :label="item.label" :value="item.value")
          transition(name="fade")
            el-col(v-if="performerVisible || form.performerId" :span="8")
              el-form-item(prop="performerId")
                el-select(
                  v-model="form.performerId"
                  size="medium"
                  placeholder="Ответственный"
                  :remote-method="searchUsers"
                  filterable remote clearable default-first-option)
                  el-option(v-for="item in userList" :key="item.id" :label="item.value" :value="item.id")
          el-col(v-if="!$route.params.projectId" :span="8")
            el-form-item(prop="projectId")
              el-select(
                v-model="form.projectId"
                size="medium"
                placeholder="Проект"
                :remote-method="searchProjects"
                filterable remote clearable default-first-option)
                el-option(v-for="item in projectList" :key="item.id" :label="item.value" :value="item.id")
          transition(name="fade")
            el-col(v-if="expectedCompletedDateVisible || form.expectedCompletedDate" :span="8")
              el-form-item(prop="expectedCompletedDate")
                el-date-picker(
                  v-model="form.expectedCompletedDate"
                  size="medium"
                  prefix-icon="el-icon-arrow-down"
                  suffix-icon="el-icon-arrow-down"
                  placeholder="Крайний срок")
          transition(name="fade")
            el-col(v-if="attachmentsVisible || attachmentList.length" :span="24")
              el-form-item
                el-upload(
                  action="https://demo.girngm.ru/workflow_dev/api/Goals/AddAttachments/"
                  :http-request="uploadAttachment"
                  :on-remove="removeAttachment"
                  :file-list="attachmentList"
                  drag multiple)
                  i.el-icon-upload
                  div.el-upload__text
                    em Выберите файл
                    span  или перетащите его сюда
          el-col(:span="24")
            div.extra
              el-tooltip(content="Теги" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="400")
                el-button(v-if="!(form.tags && form.tags.length)" type="text" title="Теги" @click="tagsVisible = !tagsVisible" circle)
                  feather(type="tag")
              el-tooltip(content="Приоритет" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="400")
                el-button(v-if="!form.priority" type="text" @click="priorityVisible = !priorityVisible" circle)
                  feather(type="zap")
              el-tooltip(content="Ответственный" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="400")
                el-button(v-if="!form.performerId" type="text" @click="performerVisible = !performerVisible" circle)
                  feather(type="user")
              el-tooltip(content="Крайний срок" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="400")
                el-button(v-if="!form.expectedCompletedDate" type="text" @click="expectedCompletedDateVisible = !expectedCompletedDateVisible" circle)
                  feather(type="calendar")
              el-tooltip(content="Вложения" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="400")
                el-button(v-if="!attachmentList.length" type="text" @click="attachmentsVisible = !attachmentsVisible" circle)
                  feather(type="paperclip")

    div(slot="footer")
      el-button(size="medium" type="default" @click="submit") {{ isEdit ? 'Сохранить' : 'Создать' }}
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseDialog from '~/components/BaseDialog';
import dialogMixin from '~/mixins/dialog.mixin';

export default {
  components: { BaseDialog },
  props: {
    id: Number
  },
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
        priority: null,
        ownerId: null,
        ownerFio: null,
        performerId: null,
        performerFio: null,
        observerIds: [],
        childGoalIds: [],
        tags: []
      },
      rules: {
        title: [
          {
            required: true,
            message: 'Введите название задачи',
            trigger: 'blur'
          }
        ],
        performerId: [
          { required: true, message: 'Укажите ответственного', trigger: 'blur' }
        ],
        projectId: [
          { required: true, message: 'Укажите проект', trigger: 'blur' }
        ]
      },
      priorities: [
        { value: 'High', label: 'Высокий' },
        { value: 'Normal', label: 'Обычный' },
        { value: 'Low', label: 'Низкий' }
      ],
      attachmentList: [],
      tagsVisible: null,
      priorityVisible: null,
      performerVisible: null,
      expectedCompletedDateVisible: null,
      attachmentsVisible: null
    };
  },
  computed: {
    ...mapGetters({
      item: 'tasks/getTask',
      me: 'auth/me',
      attachments: 'tasks/getTaskAttachments'
    })
  },
  async mounted() {
    this.loading = true;
    if (this.isEdit) {
      await this.fetchAttachments(this.id);
      this.attachmentList = this.attachments.map(attachment => {
        return {
          name: attachment.fileName,
          id: attachment.id
        };
      });
    } else {
      this.attachmentList = [];
    }
    this.loading = false;
    await this.searchUsers();
    await this.searchProjects();
  },
  methods: {
    ...mapActions({
      fetchItem: 'tasks/fetchTask',
      createItem: 'tasks/createTask',
      updateItem: 'tasks/updateTask',
      fetchAttachments: 'tasks/fetchAttachments',
      addAttachments: 'tasks/addAttachments',
      removeAttachments: 'tasks/removeAttachments'
    }),
    async uploadAttachment(request) {
      let files = new FormData();
      files.append('files', request.file);
      await this.addAttachments({
        taskId: this.id,
        files
      });
    },
    async removeAttachment(file) {
      await this.removeAttachments([file.id]);
      console.log(file);
    }
  }
};
</script>

<style lang="scss">
.extra {
  display: flex;
  justify-content: flex-end;
  i {
    height: 18px;
  }
}
.el-upload-dragger {
  border-color: var(--popover-border);
  background-color: var(--card-background);
}
.el-upload,
.el-upload-dragger {
  width: 100%;
  height: 120px;
}
.el-upload-dragger .el-icon-upload {
  font-size: 50px;
  margin: 10px 0 10px;
}
</style>
