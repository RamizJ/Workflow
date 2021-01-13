<template>
  <BaseWindow @closed="exit">
    <h1 slot="title">Задача</h1>
    <el-form
      slot="body"
      ref="form"
      v-loading="loading"
      label-position="top"
      :model="form"
      :rules="rules"
      :disabled="form.id && form.isRemoved"
      @submit.native.prevent="submit"
    >
      <el-tabs v-model="activeTab">
        <el-tab-pane label="Описание" name="description">
          <el-row :gutter="20">
            <el-col :span="$route.params.projectId ? 15 : 24">
              <el-form-item label="Название" prop="title">
                <el-input
                  ref="title"
                  v-model="form.title"
                  placeholder="Введите название"
                  @keyup.enter.native="submit"
                ></el-input>
              </el-form-item>
            </el-col>
            <el-col v-if="!$route.params.projectId" :span="12">
              <el-form-item label="Проект" prop="projectId">
                <el-select
                  v-model="form.projectId"
                  placeholder="Найти проект..."
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
              <el-form-item label="Приоритет" prop="priority">
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
            <el-col :span="24">
              <el-form-item label="Детали" prop="description">
                <el-input
                  v-model="form.description"
                  :autosize="{ minRows: 2 }"
                  type="textarea"
                  placeholder="Дополнительная информация..."
                ></el-input>
              </el-form-item>
            </el-col>
          </el-row>
        </el-tab-pane>
        <el-tab-pane label="Исполнение" name="execution">
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="Ответственный" prop="ownerId" required>
                <el-select
                  v-model="form.ownerId"
                  placeholder="Ответственный"
                  :remote-method="searchUsers"
                  @blur="searchUsers()"
                  default-first-option
                  filterable
                  clearable
                  remote
                  disabled
                >
                  <el-option
                    v-if="form.ownerId && !users.some((user) => user.id === form.ownerId)"
                    :key="form.ownerId"
                    :label="form.ownerFio"
                    :value="form.ownerId"
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
            <el-col :span="12">
              <el-form-item label="Исполнитель" prop="performerId">
                <el-select
                  v-model="form.performerId"
                  placeholder="Найти исполнителя..."
                  :remote-method="findProjectUsers"
                  default-first-option
                  filterable
                  clearable
                  remote
                >
                  <el-option
                    v-if="
                      form.performerId && !projectUsers.some((user) => user.id === form.performerId)
                    "
                    :key="form.performerId"
                    :label="shortenFullName(form.performerFio)"
                    :value="form.performerId"
                  ></el-option>
                  <el-option
                    v-for="item in projectUsers"
                    :key="item.id"
                    :label="item.value"
                    :value="item.id"
                  ></el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="Крайний срок" prop="expectedCompletedDate">
                <el-date-picker
                  v-model="form.expectedCompletedDate"
                  :picker-options="{ disabledDate: validateDate }"
                  type="datetime"
                  prefix-icon="el-icon-arrow-down"
                  format="dd.MM.yyyy HH:mm"
                  default-time="12:00:00"
                  placeholder="Выбрать крайний срок"
                ></el-date-picker>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="Ориентировочное время выполнения" prop="expectedCompletedDate">
                <el-date-picker
                  v-model="form.estimatedPerformingTime"
                  :picker-options="{ disabledDate: validateDate }"
                  type="datetime"
                  prefix-icon="el-icon-arrow-down"
                  format="dd.MM.yyyy HH:mm"
                  default-time="12:00:00"
                  placeholder="Выбрать время выполнения"
                ></el-date-picker>
              </el-form-item>
            </el-col>
          </el-row>
        </el-tab-pane>
        <el-tab-pane label="Вложения" name="attachments" :lazy="true">
          <el-col :span="24">
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
        </el-tab-pane>
        <el-tab-pane v-if="form.id" label="Согласование" name="approval" :lazy="true">
          <goal-approval :goal-id="form.id" :goal-title="form.title" />
        </el-tab-pane>
      </el-tabs>
    </el-form>
    <template v-if="!loading && (!form.id || !form.isRemoved)" slot="footer">
      <div class="extra"></div>
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
import { Component, Mixins, Prop, Ref } from 'vue-property-decorator'
import { HttpRequestOptions } from 'element-ui/types/upload'
import { Input, Message, Upload } from 'element-ui'
import { ElForm } from 'element-ui/types/form'
import tableStore from '@/core/store/table.store'
import goalsStore from '@/modules/goals/store/goals.store'
import authStore from '@/modules/users/store/auth.store'
import projectsStore from '@/modules/projects/store/projects.store'
import BaseWindow from '../../../../core/components/base-window.vue'
import DialogMixin from '../../../../core/mixins/dialog.mixin'
import Goal, { Priority } from '@/modules/goals/models/goal.type'
import Attachment from '@/modules/goals/models/attachment.type'
import User from '@/modules/users/models/user.type'
import GoalApproval from '@/modules/goals/components/goal-window/goal-approval.vue'
import usersModule from '@/modules/users/store/users.store'

@Component({ components: { GoalApproval, BaseWindow } })
export default class GoalWindow extends Mixins(DialogMixin) {
  @Prop() readonly id: number | undefined
  @Ref() readonly title?: Input

  private activeTab = 'description'

  private form: Goal = new Goal()

  private rules = {
    title: [{ required: true, message: '!', trigger: 'blur' }],
    // performerId: [{ required: true, message: '!', trigger: 'blur' }],
    projectId: [{ required: true, message: '!', trigger: 'blur' }],
    priority: [{ required: true }],
  }
  private priorities = [
    { value: Priority.High, label: 'Высокий приоритет' },
    { value: Priority.Normal, label: 'Средний приоритет' },
    { value: Priority.Low, label: 'Низкий приоритет' },
  ]

  private isReloadRequested = false

  private get me(): User | null {
    return authStore.me
  }

  protected async mounted(): Promise<void> {
    this.visible = true
    this.loading = true

    this.form.projectId = this.$route.params.projectId
      ? parseInt(this.$route.params.projectId)
      : null
    if (goalsStore.goal) this.form = { ...goalsStore.goal }
    if (goalsStore.goal?.id) {
      const fullGoal = await goalsStore.findOneById(goalsStore.goal.id)
      this.form = { ...fullGoal, ...this.form }
    } else {
      this.form.ownerId = this.me?.id
      this.form.ownerFio = `${this.me?.lastName} ${this.me?.firstName} ${this.me?.middleName}`
    }

    await this.searchProjects()
    this.loading = false
    ;(this.$refs.title as Input).focus()
  }

  private get projectUsers(): { value: string | undefined; id: string | undefined }[] {
    return projectsStore.projectUsers.map((user) => {
      return {
        value: `${user.lastName} ${user.firstName} ${user.middleName}`,
        id: user.id,
      }
    })
  }

  private async findProjectUsers(filter = ''): Promise<void> {
    try {
      await projectsStore.findUsers({
        filter,
        projectId: this.form.projectId || undefined,
        pageNumber: 0,
        pageSize: 10,
      })
    } catch (e) {
      Message.error('Ошибка загрузки списка пользователей')
    }
  }

  private async submit(): Promise<void> {
    const form = this.$refs.form as ElForm
    await form.validate(async (valid) => {
      if (valid) {
        const uploadComponent = this.$refs.upload as Upload
        await this.sendForm()
        if (this.form.isAttachmentsExist) uploadComponent.submit()
        if (!this.isReloadRequested) tableStore.requireReload()
        this.$emit('submit')
        this.exit()
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
    if (!this.form.performerId) delete entity.performerId
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

  private async onProjectChange(): Promise<void> {
    this.form.children?.forEach((item) => {
      item.projectId = this.form.projectId
    })
    await this.findProjectUsers()
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
    tableStore.requireReload()
    this.isReloadRequested = true
  }

  private async onAttachmentRemove(file: Attachment, fileList: FileList): Promise<void> {
    this.form.isAttachmentsExist = !!fileList.length
    if (file.id) await goalsStore.removeAttachments([file.id])
  }
}
</script>

<style scoped></style>
