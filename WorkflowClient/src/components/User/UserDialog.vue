<template>
  <base-dialog v-if="visible" @close="exit" ref="dialog">
    <h1 slot="title">Пользователь</h1>
    <el-form
      slot="body"
      :model="form"
      :rules="rules"
      ref="form"
      v-loading="loading"
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
          <el-col v-if="teamsVisible || (form.teamIds && form.teamIds.length)" :span="24">
            <el-form-item prop="teams">
              <el-select
                v-model="form.teamIds"
                placeholder="Команды"
                :remote-method="searchTeams"
                multiple="multiple"
                filterable="filterable"
                remote="remote"
                clearable="clearable"
                default-first-option="default-first-option"
              >
                <el-option
                  v-for="item in teams"
                  :key="item.id"
                  :label="item.value"
                  :value="item.id"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </transition>
        <transition name="fade">
          <el-col v-if="rolesVisible || (form.roles && form.roles.length)" :span="24">
            <el-form-item prop="roles">
              <el-select v-model="form.roles" placeholder="Права" multiple="multiple">
                <el-option
                  v-for="item in roles"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                ></el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </transition>
      </el-row>
    </el-form>
    <template slot="footer">
      <div class="extra">
        <!--el-tooltip(content="Команды" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
      el-button(v-if="!(form.teamIds && form.teamIds.length)" type="text" @click="teamsVisible = !teamsVisible" circle)
        feather(type="users")
      -->
        <!--el-tooltip(v-if="!(form.roles && form.roles.length)" content="Права" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
      el-button(type="text" @click="rolesVisible = !rolesVisible" circle)
        feather(type="shield")
      -->
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
            <feather type="briefcase"></feather>
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
            <feather type="phone"></feather>
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
import { Component, Prop, Ref } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'

import usersModule from '@/store/modules/users.module'
import DialogMixin from '@/mixins/dialog.mixin'
import BaseDialog from '@/components/BaseDialog.vue'
import User from '@/types/user.type'
import { Input, Message } from 'element-ui'
import { ElForm } from 'element-ui/types/form'
import Project from '@/types/project.type'
import projectsModule from '@/store/modules/projects.module'

@Component({ components: { BaseDialog } })
export default class UserDialog extends mixins(DialogMixin) {
  @Prop() readonly id: string | undefined
  @Ref() readonly title?: Input

  private isEdit = !!this.id

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
    roles: []
  }
  private rules = {
    name: [{ required: true, message: '!', trigger: 'blur' }],
    lastName: [{ required: true, message: '!', trigger: 'blur' }],
    firstName: [{ required: true, message: '!', trigger: 'blur' }],
    userName: [
      { required: true, message: '!', trigger: 'blur' },
      { validator: this.validateLogin, trigger: 'blur' }
    ],
    password: [{ validator: this.validatePassword, trigger: 'blur' }],
    email: [
      { required: true, message: '!', trigger: 'blur' },
      { type: 'email', message: 'не почта', trigger: 'blur' },
      { validator: this.validateEmail, trigger: 'blur' }
    ]
  }
  private roles: string[] = []
  private teamsVisible = null
  private rolesVisible = null
  private positionVisible = null
  private phoneVisible = null

  private async mounted() {
    this.visible = true

    this.loading = true
    if (this.id) this.form = await usersModule.findOneById(this.id.toString())
    await this.searchTeams()
    this.loading = false

    this.title?.focus()
    ;(this.$refs.title as HTMLInputElement).focus()
  }

  async submit() {
    const form = this.$refs.form as ElForm
    await form.validate(async valid => {
      if (valid) {
        await this.sendForm()
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
    const entity: User = { ...this.form } as User
    if (this.isEdit) await usersModule.updateOne(entity)
    else await usersModule.createOne(entity)
    this.loading = false
  }

  private async validateLogin(rule: any, value: string, callback: any) {
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

  private async validateEmail(rule: any, value: string, callback: any) {
    const emailAlreadyExist = await usersModule.isEmailExist(value)
    if (emailAlreadyExist && usersModule.user?.email !== value) callback(new Error('занято'))
    else callback()
  }

  private validatePassword(rule: any, value: string, callback: any) {
    const length = value?.trim().length
    const symbolsLeft = 6 - length
    if (!value && this.isEdit) callback()
    if (!value) callback(new Error('!'))
    else if (length < 6)
      callback(
        new Error(`ещё ${symbolsLeft}
        ${symbolsLeft > 1 ? (symbolsLeft > 4 ? 'символов' : 'символа') : 'символ'}`)
      )
    else if (!/[a-z]/.test(value)) callback(new Error('нужна буква'))
    else callback()
  }
}
</script>
