<template>
  <base-window @closed="exit">
    <h1 slot="title">Изменить статус</h1>
    <el-form
      slot="body"
      ref="form"
      v-loading="loading"
      label-position="top"
      :model="form"
      @submit.native.prevent="submit"
    >
      <el-row :gutter="20">
        <el-col :span="24">
          <el-form-item label="Статус" prop="state">
            <el-select v-model="form.goalState" placeholder="Выбрать статус">
              <el-option
                v-for="item in statuses"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              ></el-option>
            </el-select>
          </el-form-item>
          <el-form-item label="Комментарий" prop="comment">
            <el-input
              v-model="form.comment"
              :autosize="{ minRows: 2 }"
              type="textarea"
              placeholder="Напишите комментарий..."
            ></el-input>
          </el-form-item>
          <el-form-item label="Время выполнения (час.)" prop="actualPerformingHours">
            <el-input-number
              v-model="form.actualPerformingHours"
              controls-position="right"
              placeholder="Кол-во часов"
              :min="1"
            />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <template v-if="!loading" slot="footer">
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
  </base-window>
</template>

<script lang="ts">
import { Component, Mixins, Prop } from 'vue-property-decorator'
import BaseWindow from '../../../core/components/base-window.vue'
import DialogMixin from '@/core/mixins/dialog.mixin'
import Goal, { Status, statuses } from '@/modules/goals/models/goal.type'
import goalsStore from '@/modules/goals/store/goals.store'
import { Message } from 'element-ui'
import tableStore from '@/core/store/table.store'
import { ChangeStatusForm } from '@/modules/goals/models/change-status-form.interface'

@Component({
  components: { BaseWindow },
})
export default class GoalChangeStatus extends Mixins(DialogMixin) {
  @Prop() readonly goal!: Goal
  private form: ChangeStatusForm = { comment: '' }

  protected mounted(): void {
    const goal = goalsStore.goal ? { ...goalsStore.goal } : this.goal
    this.form = {
      goalId: goal.id,
      goalState: goal.state,
      comment: '',
      estimatedPerformingHours: goal.estimatedPerformingHours,
      actualPerformingHours: goal.actualPerformingHours,
    }
  }

  private get statuses(): { value: Status; label: string }[] {
    return statuses
  }

  private async submit(): Promise<void> {
    this.loading = true
    try {
      await goalsStore.changeStates([this.form])
      tableStore.requireReload()
      this.exit()
    } catch (e) {
      Message({
        showClose: true,
        message: 'Ошибка обновления задачи',
        type: 'error',
      })
    }
    this.loading = false
  }
}
</script>

<style scoped></style>
