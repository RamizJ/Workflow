<template>
  <BaseWindow @closed="$emit('close')">
    <h1 slot="title">Область</h1>
    <el-form
      slot="body"
      ref="form"
      label-position="top"
      v-loading="windowService.loading"
      :model="windowService.entity"
      :disabled="windowService.entity.id && windowService.entity.isRemoved"
      @submit.native.prevent.capture="submit"
    >
      <el-row :gutter="20">
        <el-col :span="24">
          <el-form-item label="Название" prop="title">
            <el-input
              ref="title"
              v-model="windowService.entity.name"
              placeholder="Название"
            ></el-input>
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="Описание" prop="title">
            <el-input
              ref="title"
              type="textarea"
              v-model="windowService.entity.description"
              placeholder="Описание"
            ></el-input>
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <template
      v-if="!windowService.loading && (!windowService.entity.id || !windowService.entity.isRemoved)"
      slot="footer"
    >
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
          <el-button type="text" @click.capture="submit" circle="circle">
            <unicon name="save" />
          </el-button>
        </el-tooltip>
      </div>
    </template>
  </BaseWindow>
</template>

<script lang="ts">
import { Component, Ref, Vue } from 'vue-property-decorator'
import BaseWindow from '@/core/components/base-window.vue'
import GroupWindowService from '@/modules/groups/services/group-window.service'
import { ElForm } from 'element-ui/types/form'

@Component({
  components: { BaseWindow },
})
export default class GroupWindow extends Vue {
  @Ref('form') private readonly form!: ElForm
  private readonly windowService: GroupWindowService

  constructor() {
    super()
    this.windowService = new GroupWindowService()
  }

  protected async mounted(): Promise<void> {
    this.windowService.form = await this.getFormComponent()
  }

  private async submit(): Promise<void> {
    await this.windowService.submit()
    this.$emit('submit')
    this.$emit('close')
  }

  private async getFormComponent(): Promise<ElForm> {
    return new Promise((resolve) => setTimeout(() => resolve(this.form), 1))
  }
}
</script>

<style scoped></style>
