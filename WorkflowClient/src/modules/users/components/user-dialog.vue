<template>
  <base-dialog v-if="visible" @close="exit" ref="dialog">
    <h1 slot="title">Пользователь</h1>
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
        <el-col :span="8">
          <el-form-item prop="lastName">
            <el-input ref="title" v-model="form.lastName" placeholder="Фамилия"></el-input>
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item prop="firstName">
            <el-input v-model="form.firstName" placeholder="Имя"></el-input>
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item prop="middleName">
            <el-input v-model="form.middleName" placeholder="Отчество"></el-input>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="8">
          <el-form-item prop="userName">
            <el-input v-model="form.userName" placeholder="Логин"></el-input>
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item prop="password">
            <el-input
              v-model="form.password"
              type="password"
              placeholder="Пароль"
              readonly="readonly"
              onfocus="this.removeAttribute('readonly')"
            ></el-input>
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item prop="email">
            <el-input v-model="form.email" placeholder="Почта"></el-input>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <transition name="fade">
          <el-col v-if="phoneVisible || form.phone" :span="8">
            <el-form-item prop="phone">
              <el-input v-model="form.phone" placeholder="Телефон"></el-input>
            </el-form-item>
          </el-col>
        </transition>
        <transition name="fade">
          <el-col v-if="positionVisible || form.position" :span="8">
            <el-form-item prop="positionId">
              <el-input v-model="form.position" placeholder="Должность"></el-input>
            </el-form-item>
          </el-col>
        </transition>
        <transition name="fade">
          <el-col
            v-if="
              $route.params.teamId &&
              (rolesVisible || form.canEditGoals || form.canCloseGoals || form.canEditUsers)
            "
            :span="24"
          >
            <el-form-item>
              <el-checkbox v-model="form.canEditGoals">Изменение задач</el-checkbox>
              <el-checkbox v-model="form.canCloseGoals">Завершение задач</el-checkbox>
              <el-checkbox v-model="form.canEditUsers">Изменение пользователей</el-checkbox>
            </el-form-item>
          </el-col>
        </transition>
      </el-row>
    </el-form>
    <template v-if="!loading && (!form.id || !form.isRemoved)" slot="footer">
      <div class="extra">
        <!-- <el-tooltip
          v-if="
            $route.params.teamId && !(form.canEditGoals || form.canCloseGoals || form.canEditUsers)
          "
          content="Права"
          effect="dark"
          placement="top"
          transition="fade"
          :visible-arrow="false"
          :open-delay="500"
        >
          <el-button type="text" @click="rolesVisible = !rolesVisible" circle="circle">
            <feather type="shield"></feather>
          </el-button>
        </el-tooltip> -->
        <el-tooltip
          v-if="!form.position"
          content="Должность"
          effect="dark"
          placement="top"
          transition="fade"
          :visible-arrow="false"
          :open-delay="500"
        >
          <el-button type="text" @click="positionVisible = !positionVisible" circle="circle">
            <unicon name="suitcase" />
          </el-button>
        </el-tooltip>
        <el-tooltip
          v-if="!form.phone"
          content="Телефон"
          effect="dark"
          placement="top"
          transition="fade"
          :visible-arrow="false"
          :open-delay="500"
        >
          <el-button type="text" @click="phoneVisible = !phoneVisible" circle="circle">
            <unicon name="phone" />
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
  </base-dialog>
</template>

<script lang="ts">
import { Component, Prop, Ref, Mixins } from 'vue-property-decorator'
import { Input, Message } from 'element-ui'
import { ElForm } from 'element-ui/types/form'

import usersModule from '@/modules/users/store/users.store'
import DialogMixin from '@/core/mixins/dialog.mixin'
import BaseDialog from '@/core/components/BaseDialog.vue'
import User, { Role } from '@/modules/users/models/user.type'
import { ValidationRule } from '@/core/types/validation-rule.type'

@Component({ components: { BaseDialog } })
export default class UserDialog extends Mixins(DialogMixin) {
  @Prop() readonly id: string | undefined
  @Ref() readonly title?: Input

  private form: User = {
    lastName: '',
    firstName: '',
    middleName: '',
    userName: '',
    password: '',
    email: '',
    phone: '',
    position: '',
    teamIds: this.$route.params.teamId ? [parseInt(this.$route.params.teamId)] : [],
    roles: [],
    isRemoved: false,
  }
  private rules = {
    name: [{ required: true, message: '!', trigger: 'blur' }],
    lastName: [{ required: true, message: '!', trigger: 'blur' }],
    firstName: [{ required: true, message: '!', trigger: 'blur' }],
    userName: [
      { required: true, message: '!', trigger: 'blur' },
      { validator: this.validateLogin, trigger: 'blur' },
    ],
    password: [{ validator: this.validatePassword, trigger: 'blur' }],
    email: [
      { required: true, message: '!', trigger: 'blur' },
      { type: 'email', message: 'не почта', trigger: 'blur' },
      { validator: this.validateEmail, trigger: 'blur' },
    ],
  }
  private roles: { value: Role; label: string }[] = [
    { value: Role.EDIT_PROJECTS, label: 'Редактирование проектов' },
    { value: Role.EDIT_TASKS, label: 'Редактирование задач' },
    { value: Role.EDIT_TEAMS, label: 'Редактирование команд' },
    { value: Role.EDIT_USERS, label: 'Редактирование пользователей' },
  ]
  private rolesVisible = null
  private positionVisible = null
  private phoneVisible = null

  protected async mounted(): Promise<void> {
    this.visible = true

    this.loading = true
    try {
      if (this.id) this.form = await usersModule.findOneById(this.id.toString())
    } catch (e) {
      Message.error('Не удаётся загрузить пользователя')
    }
    this.loading = false

    this.title?.focus()
    ;(this.$refs.title as HTMLInputElement).focus()
  }

  private async submit(): Promise<void> {
    const form = this.$refs.form as ElForm
    await form.validate(async (valid) => {
      if (valid) {
        await this.sendForm()
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
    const entity: User = { ...this.form } as User
    if (this.id) await usersModule.updateOne(entity)
    else await usersModule.createOne(entity)
    this.loading = false
  }

  private async validateLogin(
    rule: ValidationRule,
    value: string,
    callback: CallableFunction
  ): Promise<void> {
    console.log(rule)
    const loginPattern = /^[a-zA-Z0-9_-]+$/
    const isLoginChanged = usersModule.user?.userName !== value
    const isLoginValid = loginPattern.test(value)

    if (!isLoginValid) callback(new Error('!'))

    if (isLoginChanged && isLoginValid) {
      const isLoginAlreadyExist = await usersModule.isLoginExist(value)
      if (isLoginAlreadyExist) callback(new Error('занято'))
      else callback()
    }
  }

  private async validateEmail(
    rule: ValidationRule,
    value: string,
    callback: CallableFunction
  ): Promise<void> {
    const emailAlreadyExist = await usersModule.isEmailExist(value)
    if (emailAlreadyExist && usersModule.user?.email !== value) callback(new Error('занято'))
    else callback()
  }

  private validatePassword(rule: ValidationRule, value: string, callback: CallableFunction): void {
    const length = value?.trim().length
    const symbolsLeft = 6 - length
    if (!value && this.id) callback()
    if (!value) callback(new Error('!'))
    else if (length < 6)
      callback(
        new Error(`ещё ${symbolsLeft}
        ${symbolsLeft > 1 ? (symbolsLeft > 4 ? 'символов' : 'символа') : 'символ'}`)
      )
    else if (!/[a-z]/.test(value)) callback(new Error('нужна буква'))
    else callback()
  }

  public exit(): void {
    this.visible = false
    usersModule.closeUserWindow()
    this.$emit('close')
  }
}
</script>
