<template>
  <BaseWindow @closed="exit">
    <h1 slot="title">{{ windowTitle || 'Задача' }}</h1>
    <el-form
      slot="body"
      ref="form"
      v-loading="loading"
      :model="form"
      :rules="rules"
      :disabled="form.id && form.isRemoved"
      @submit.native.prevent="submit"
    >
      <el-row :gutter="20">
        <el-col :span="$route.params.projectId ? 15 : 24">
          <el-form-item prop="title">
            <el-input
              ref="title"
              v-model="form.title"
              placeholder="Название"
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
              @change="onProjectChange"
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
        <el-col :span="$route.params.projectId ? 9 : 12">
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
          <el-col v-if="performerVisible || form.performerId" :span="7">
            <el-form-item prop="performerId">
              <el-select
                v-model="form.performerId"
                placeholder="Исполнитель"
                :remote-method="searchUsers"
                @blur="searchUsers()"
                default-first-option
                filterable
                clearable
                remote
              >
                <el-option
                  v-if="form.performerId && !users.some((user) => user.id === form.performerId)"
                  :key="form.performerId"
                  :label="shortenFullName(form.performerFio)"
                  :value="form.performerId"
                ></el-option>
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
                :on-remove="onAttachmentRemove"
                :on-success="onAttachmentsUploaded"
                :on-change="onAttachmentsChange"
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
    <template v-if="!loading && (!form.id || !form.isRemoved)" slot="footer">
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
            <unicon name="file-alt" />
          </el-button>
        </el-tooltip>
        <el-tooltip
          content="Исполнитель"
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
            <unicon name="user" />
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
            <unicon name="calendar-alt" />
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
            <unicon name="paperclip" />
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
            <unicon name="save" />
          </el-button>
        </el-tooltip>
      </div>
    </template>
  </BaseWindow>
</template>

<script lang="ts">
import { Component, Prop, Ref, Mixins } from 'vue-property-decorator'
import { ElUpload, HttpRequestOptions } from 'element-ui/types/upload'
import { ElForm } from 'element-ui/types/form'
import { Input, Message } from 'element-ui'
import moment from 'moment'

import goalsStore from '@/modules/goals/store/goals.store'
import authStore from '@/modules/users/store/auth.store'
import tableStore from '@/core/store/table.store'
import DialogMixin from '@/core/mixins/dialog.mixin'
import BaseWindow from '@/core/components/base-window.vue'
import Goal, { Priority, Status } from '@/modules/goals/models/goal.type'
import Attachment from '@/modules/goals/models/attachment.type'

@Component({ components: { BaseWindow } })
export default class GoalWindow extends Mixins(DialogMixin) {
  @Prop() readonly id: number | undefined
  @Prop() readonly caption: string | undefined
  @Ref() readonly title?: Input

  private windowTitle = 'Задача'
  private form: Goal = new Goal()

  private rules = {
    title: [{ required: true, message: '!', trigger: 'blur' }],
    performerId: [{ required: true, message: '!', trigger: 'blur' }],
    projectId: [{ required: true, message: '!', trigger: 'blur' }],
  }
  private priorities = [
    { value: Priority.High, label: 'Высокий приоритет' },
    { value: Priority.Normal, label: 'Средний приоритет' },
    { value: Priority.Low, label: 'Низкий приоритет' },
  ]

  private descriptionVisible = null
  private checklistVisible = null
  private priorityVisible = null
  private performerVisible = null
  private expectedCompletedDateVisible = null
  private attachmentsVisible = null

  protected async mounted(): Promise<void> {
    this.visible = true

    this.loading = true
    if (this.id) {
      const id: number = parseInt(this.id.toString())
      this.form = await goalsStore.findOneById(id)
    } else if (goalsStore.goal) {
      this.form = goalsStore.goal
      console.log(this.form)
    }
    if (this.isSection) {
      this.form.metadataList = [{ key: 'isSection', value: 'true' }]
      this.windowTitle = 'Раздел'
    }
    this.form.projectId = this.$route.params.projectId
      ? parseInt(this.$route.params.projectId)
      : undefined
    await this.searchUsers()
    await this.searchProjects()
    this.loading = false
    ;(this.$refs.title as Input).focus()
  }

  private get isSection(): boolean {
    const hasSectionMark = this.form.metadataList?.some(
      (metadata) => metadata.key === 'isSection' && metadata.value === 'true'
    )
    const hasSectionTitle = this.caption === 'Раздел'
    return hasSectionMark || hasSectionTitle
  }

  private async submit(): Promise<void> {
    const form = this.$refs.form as ElForm
    await form.validate(async (valid) => {
      if (valid) {
        await this.sendForm()
        if (this.form.isAttachmentsExist) {
          ;(this.$refs.upload as ElUpload).submit()
        } else {
          this.$emit('submit')
          this.exit()
        }
        tableStore.requireReload()
      } else {
        Message({
          showClose: true,
          message: 'Форма заполнена некорректно',
          type: 'error',
        })
      }
    })
  }

  private async sendForm(): Promise<void> {
    this.loading = true
    const hasAttachments = this.form.isAttachmentsExist
    const hasChildren = this.form.hasChildren
    const entity: Goal = { ...this.form } as Goal
    if (!this.performerVisible && !this.form.performerId) delete entity.performerId
    console.log(this.id)
    console.log(this.form.id)
    if (this.id || this.form.id) await goalsStore.updateOne(entity)
    else this.form = await goalsStore.createOne(entity)
    this.form.isAttachmentsExist = hasAttachments
    this.form.hasChildren = hasChildren
    this.loading = false
  }

  public exit(): void {
    const form = this.$refs.form as ElForm
    form.resetFields()
    this.visible = false
    this.$emit('close')
  }

  private onProjectChange(): void {
    this.form.children?.forEach((item) => {
      item.projectId = this.form.projectId
    })
  }

  private onChecklistChange(checklist: Goal[]): void {
    this.form.children = checklist
    this.form.hasChildren = checklist.some((goal: Goal) => !goal.isRemoved)
    this.$forceUpdate()
  }

  private validateDate(date: Date): boolean {
    const currentDate = new Date()
    currentDate.setDate(currentDate.getDate() - 1)
    return date < currentDate
  }

  private async uploadAttachment(httpRequest: HttpRequestOptions): Promise<void> {
    this.loading = true
    const id = this.id || this.form.id
    if (!id) return
    const files = new FormData()
    let file: File = httpRequest.file
    if (httpRequest.file.name.length > 96) {
      const filename = this.shortenFilename(httpRequest.file.name)
      file = this.renameFile(httpRequest.file, filename)
    }
    files.append('files', file)
    await goalsStore.uploadAttachments({ id, files })
    this.loading = false
  }

  private renameFile(originalFile: File, newName: string): File {
    return new File([originalFile], newName, {
      type: originalFile.type,
      lastModified: originalFile.lastModified,
    })
  }

  private shortenFilename(filename: string): string {
    const format = filename.slice(filename.lastIndexOf('.') + 1)
    const name = filename.slice(0, 85)
    return `${name}.${format}`
  }

  private async onAttachmentClick(attachment: Attachment): Promise<void> {
    if (!attachment.id) return
    await goalsStore.downloadAttachment(attachment)
  }

  private async onAttachmentsChange(file: File, fileList: FileList): Promise<void> {
    this.form.isAttachmentsExist = !!fileList.length
  }

  private async onAttachmentsUploaded(
    response: Response,
    file: File,
    fileList: FileList
  ): Promise<void> {
    this.$emit('submit')
    this.exit()
  }

  private async onAttachmentRemove(file: Attachment, fileList: FileList): Promise<void> {
    this.form.isAttachmentsExist = !!fileList.length
    if (file.id) await goalsStore.removeAttachments([file.id])
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
