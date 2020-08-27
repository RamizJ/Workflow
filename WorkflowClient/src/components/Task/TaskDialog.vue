<template>
  <base-dialog v-if="visible" @close="exit" ref="dialog">
    <h1 slot="title">Задача</h1>
    <el-form
      slot="body"
      :model="form"
      :rules="rules"
      ref="form"
      v-loading="loading"
      @submit.native.prevent="submit"
    >
      <el-row :gutter="20">
        <el-col :span="$route.params.projectId ? 15 : 24">
          <el-form-item prop="title">
            <el-input
              ref="title"
              v-model="form.title"
              placeholder="Новая задача"
              @keyup.enter.native="submit"
            ></el-input>
          </el-form-item>
        </el-col>
        <el-col v-if="!$route.params.projectId" :span="12">
          <el-form-item prop="projectId">
            <el-select
              v-model="form.projectId"
              placeholder="Проект"
              :remote-method="searchProjects"
              filterable="filterable"
              remote="remote"
              clearable="clearable"
              default-first-option="default-first-option"
            >
              <el-option
                v-for="item in projects"
                :key="item.id"
                :label="item.value"
                :value="item.id"
              ></el-option>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col v-if="priorityVisible || form.priority" :span="$route.params.projectId ? 9 : 12">
          <el-form-item prop="priority">
            <el-select v-model="form.priority" placeholder="Приоритет">
              <el-option
                v-for="item in priorities"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              ></el-option>
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <transition name="fade">
          <el-col v-if="descriptionVisible || form.description" :span="24">
            <el-form-item prop="description">
              <el-input
                v-model="form.description"
                :autosize="{ minRows: 2 }"
                type="textarea"
                placeholder="Описание"
              ></el-input>
            </el-form-item>
          </el-col>
        </transition>
        <transition name="fade">
          <el-col v-if="checklistVisible || (form.childTasks && form.childTasks.length)" :span="24">
            <el-form-item>
              <checklist
                :items="form.childTasks"
                :task="form"
                @change="onChecklistChange"
              ></checklist>
            </el-form-item>
          </el-col>
        </transition>
        <transition name="fade">
          <el-col v-if="performerVisible || form.performerId" :span="7">
            <el-form-item prop="performerId">
              <el-select
                v-model="form.performerId"
                placeholder="Ответственный"
                :remote-method="searchUsers"
                filterable="filterable"
                remote="remote"
                clearable="clearable"
                default-first-option="default-first-option"
              >
                <el-option
                  v-for="item in users"
                  :key="item.id"
                  :label="item.value"
                  :value="item.id"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </transition>
        <transition name="fade">
          <el-col v-if="expectedCompletedDateVisible || form.expectedCompletedDate" :span="8">
            <el-form-item prop="expectedCompletedDate">
              <el-date-picker
                v-model="form.expectedCompletedDate"
                :picker-options="{ disabledDate: validateDate }"
                type="datetime"
                prefix-icon="el-icon-arrow-down"
                format="dd.MM.yyyy HH:mm"
                default-time="12:00:00"
                placeholder="Крайний срок"
              ></el-date-picker>
            </el-form-item>
          </el-col>
        </transition>
        <transition name="fade">
          <el-col
            v-if="attachmentsVisible || (form.attachments && form.attachments.length)"
            :span="24"
          >
            <el-form-item>
              <el-upload
                ref="upload"
                action="https://demo.girngm.ru/workflow_dev/api/Goals/AddAttachments/"
                :http-request="uploadAttachment"
                :on-preview="onAttachmentClick"
                :on-remove="removeAttachment"
                :file-list="form.attachments"
                :auto-upload="false"
                drag
                multiple
                ><i class="el-icon-upload"></i>
                <div class="el-upload__text">
                  <em>Выберите файл</em><span> или перетащите его сюда</span>
                </div>
              </el-upload>
            </el-form-item>
          </el-col>
        </transition>
      </el-row>
    </el-form>
    <template slot="footer">
      <div class="extra">
        <el-tooltip
          content="Описание"
          effect="dark"
          placement="top"
          transition="fade"
          :visible-arrow="false"
          :open-delay="500"
        >
          <el-button
            v-if="!form.description"
            type="text"
            @click="descriptionVisible = !descriptionVisible"
            circle="circle"
          >
            <feather type="align-left"></feather>
          </el-button>
        </el-tooltip>
        <el-tooltip
          content="Чек-лист"
          effect="dark"
          placement="top"
          transition="fade"
          :visible-arrow="false"
          :open-delay="500"
        >
          <el-button
            v-if="!(form.childTasks && form.childTasks.length)"
            type="text"
            @click="checklistVisible = !checklistVisible"
            circle="circle"
          >
            <feather type="check-square"></feather>
          </el-button>
        </el-tooltip>
        <el-tooltip
          content="Приоритет"
          effect="dark"
          placement="top"
          transition="fade"
          :visible-arrow="false"
          :open-delay="500"
        >
          <el-button
            v-if="!form.priority"
            type="text"
            @click="priorityVisible = !priorityVisible"
            circle="circle"
          >
            <feather type="zap"></feather>
          </el-button>
        </el-tooltip>
        <el-tooltip
          content="Ответственный"
          effect="dark"
          placement="top"
          transition="fade"
          :visible-arrow="false"
          :open-delay="500"
        >
          <el-button
            v-if="!form.performerId"
            type="text"
            @click="performerVisible = !performerVisible"
            circle="circle"
          >
            <feather type="user"></feather>
          </el-button>
        </el-tooltip>
        <el-tooltip
          content="Крайний срок"
          effect="dark"
          placement="top"
          transition="fade"
          :visible-arrow="false"
          :open-delay="500"
        >
          <el-button
            v-if="!form.expectedCompletedDate"
            type="text"
            @click="expectedCompletedDateVisible = !expectedCompletedDateVisible"
            circle="circle"
          >
            <feather type="calendar"></feather>
          </el-button>
        </el-tooltip>
        <el-tooltip
          content="Вложения"
          effect="dark"
          placement="top"
          transition="fade"
          :visible-arrow="false"
          :open-delay="500"
        >
          <el-button
            v-if="!(form.attachments && form.attachments.length)"
            type="text"
            @click="attachmentsVisible = !attachmentsVisible"
            circle="circle"
          >
            <feather type="paperclip"></feather>
          </el-button>
        </el-tooltip>
      </div>
      <div class="send">
        <el-tooltip
          content="Сохранить"
          effect="dark"
          placement="top"
          transition="fade"
          :visible-arrow="false"
          :open-delay="500"
        >
          <el-button type="text" @click="submit" circle="circle">
            <feather type="arrow-right"></feather>
          </el-button>
        </el-tooltip>
      </div>
    </template>
  </base-dialog>
</template>

<script lang="ts">
import { Component, Prop, Ref, Vue } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'
import { ElUpload } from 'element-ui/types/upload'
import { ElForm } from 'element-ui/types/form'
import { Input, Message } from 'element-ui'
import { Route } from 'vue-router'
import moment from 'moment'

import tasksModule from '@/store/modules/tasks.module'
import DialogMixin from '@/mixins/dialog.mixin'
import BaseDialog from '@/components/BaseDialog.vue'
import Checklist from '@/components/Checklist.vue'
import Task, { Priority, Status } from '@/types/task.type'
import Attachment from '@/types/attachment.type'

@Component({ components: { Checklist, BaseDialog } })
export default class TaskDialog extends mixins(DialogMixin) {
  @Prop() readonly id: number | undefined
  @Ref() readonly title?: Input

  private isEdit = !!this.id

  private form: Task = {
    title: '',
    description: '',
    projectId: parseInt(this.$route.params.projectId) || undefined,
    performerId: undefined,
    performerFio: undefined,
    creationDate: moment.utc(moment()).format(),
    state: Status.New,
    priority: Priority.Normal,
    isChildsExist: false,
    isRemoved: false,
    attachments: []
  }

  private rules = {
    title: [{ required: true, message: '!', trigger: 'blur' }],
    performerId: [{ required: true, message: '!', trigger: 'blur' }],
    projectId: [{ required: true, message: '!', trigger: 'blur' }]
  }
  private priorities = [
    { value: Priority.High, label: 'Высокий приоритет' },
    { value: Priority.Normal, label: 'Средний приоритет' },
    { value: Priority.Low, label: 'Низкий приоритет' }
  ]

  private descriptionVisible = null
  private checklistVisible = null
  private priorityVisible = null
  private performerVisible = null
  private expectedCompletedDateVisible = null
  private attachmentsVisible = null

  async mounted() {
    this.visible = true

    this.loading = true
    if (this.id) {
      const id: number = parseInt(this.id.toString())
      this.form = await tasksModule.findOneById(id)
    }

    await this.searchUsers()
    await this.searchProjects()
    this.loading = false
    ;(this.$refs.title as Input).focus()
  }

  async submit() {
    const form = this.$refs.form as ElForm
    await form.validate(async valid => {
      if (valid) {
        await this.sendForm()
        if (this.form.attachments?.length) (this.$refs.upload as ElUpload).submit()
        this.$emit('submit')
        this.exit()
      } else {
        Message({
          showClose: true,
          message: 'Форма заполнена некорректно',
          type: 'error'
        })
      }
    })
  }

  async sendForm() {
    this.loading = true
    const entity: Task = { ...this.form } as Task
    entity.isChildsExist = !!entity.childTasks?.length
    entity.isAttachmentsExist = !!entity.attachments?.length
    if (this.isEdit) await tasksModule.updateOne(entity)
    else this.form = await tasksModule.createOne(entity)
    this.loading = false
  }

  exit() {
    const form = this.$refs.form as ElForm
    form.resetFields()
    this.visible = false
    this.$emit('close')
  }

  onChecklistChange(checklist: Task[]) {
    this.form.childTasks = checklist
  }

  validateDate(date: Date) {
    const currentDate = new Date()
    currentDate.setDate(currentDate.getDate() - 1)
    return date < currentDate
  }

  async uploadAttachment(request: any): Promise<void> {
    this.loading = true
    const id = this.id || this.form.id
    if (!id) return
    const files = new FormData()
    files.append('files', request.file)
    await tasksModule.uploadAttachments({ id, files })
    this.form.isAttachmentsExist = true
    this.loading = false
  }

  async onAttachmentClick(attachment: Attachment) {
    if (!attachment.id) return
    await tasksModule.downloadAttachment(attachment)
  }

  async removeAttachment(attachment: Attachment) {
    if (attachment.id) await tasksModule.removeAttachments([attachment.id])
  }
}
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
