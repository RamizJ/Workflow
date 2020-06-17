<template lang="pug">
  base-dialog(v-if="visible" @close="$emit('close')")
    div(slot="title") Пользователь
    div(slot="body")
      el-form(:model="form" :rules="rules" ref="form" v-loading="loading")
        el-row(:gutter="20")
          el-col(:span="8")
            el-form-item(prop="lastName")
              el-input(v-model="form.lastName" size="medium" placeholder="Фамилия")
          el-col(:span="8")
            el-form-item(prop="firstName")
              el-input(v-model="form.firstName" size="medium" placeholder="Имя")
          el-col(:span="8")
            el-form-item(prop="middleName")
              el-input(v-model="form.middleName" size="medium" placeholder="Отчество")
        el-row(:gutter="20")
          el-col(:span="8")
            el-form-item(prop="userName")
              el-input(v-model="form.userName" size="medium" placeholder="Логин")
          el-col(:span="8")
            el-form-item(prop="password")
              el-input(v-model="form.password" type="password" size="medium" placeholder="Пароль")
          el-col(:span="8")
            el-form-item(prop="email")
              el-input(v-model="form.email" size="medium" placeholder="Почта")
        el-row(:gutter="20")
          el-col(:span="16")
            el-form-item(prop="teams")
              el-select(
                v-model="form.teamIds"
                size="medium"
                placeholder="Команды"
                :remote-method="searchTeams"
                multiple filterable remote clearable default-first-option)
                el-option(v-for="item in teamList" :key="item.id" :label="item.value" :value="item.id")
          el-col(:span="8")
            el-form-item(prop="phone")
              el-input(v-model="form.phone" size="medium" placeholder="Телефон")
        el-row(:gutter="20")
          el-col(:span="16")
            el-form-item(prop="roles")
              el-select(v-model="form.roles" size="medium" placeholder="Права" multiple)
                el-option(v-for="item in roles" :key="item.value" :label="item.label" :value="item.value")
          el-col(:span="8")
            el-form-item(prop="positionId")
              el-select(v-model="form.positionId" size="medium" placeholder="Должность")
                el-option(v-for="item in positions" :key="item.value" :label="item.label" :value="item.value")
    div(slot="footer")
      el-button(size="medium" type="primary" @click="submit") Создать

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseDialog from '~/components/BaseDialog';
import dialogMixin from '~/mixins/dialog.mixin';

export default {
  components: { BaseDialog },
  props: {
    id: String
  },
  mixins: [dialogMixin],
  data() {
    return {
      form: {
        lastName: '',
        firstName: '',
        middleName: '',
        userName: '',
        password: '',
        email: '',
        phone: '',
        positionId: null,
        position: '',
        teamIds: [],
        roles: []
      },
      roles: [
        { value: 0, label: 'Управление проектами' },
        { value: 1, label: 'Управление пользователями' },
        { value: 2, label: 'Управление областями' }
      ],
      positions: [
        { value: 0, label: 'Начальник' },
        { value: 1, label: 'Уборщик' },
        { value: 2, label: 'Оператор' }
      ],
      rules: {
        lastName: [
          { required: true, message: 'Введите фамилию', trigger: 'blur' }
        ],
        firstName: [
          { required: true, message: 'Введите имя', trigger: 'blur' }
        ],
        userName: [
          { required: true, message: 'Введите логин', trigger: 'blur' }
        ],
        password: [{ validator: this.validatePassword, trigger: 'blur' }],
        email: [
          { required: true, message: 'Введите эл. почту', trigger: 'blur' },
          {
            type: 'email',
            message: 'Некорректный адрес эл. почты',
            trigger: 'blur'
          }
        ],
        phone: [
          { required: true, message: 'Введите номер телефона', trigger: 'blur' }
        ]
      }
    };
  },
  computed: {
    ...mapGetters({ item: 'users/getUser' })
  },
  methods: {
    ...mapActions({
      fetchItem: 'users/fetchUser',
      createItem: 'users/createUser',
      updateItem: 'users/updateUser'
    }),
    validatePassword(rule, value, callback) {
      const length = value?.trim().length;
      const symbolsLeft = 6 - length;
      if (!value && this.isEdit) callback();
      if (!value) callback(new Error('Введите пароль'));
      else if (length < 6)
        callback(
          new Error(`Введите ещё ${symbolsLeft}
        ${
          symbolsLeft > 1
            ? symbolsLeft > 4
              ? 'символов'
              : 'символа'
            : 'символ'
        }`)
        );
      else if (!/[a-z]/.test(value))
        callback(new Error('Введите хотя бы одну букву'));
      else callback();
    }
  }
};
</script>
