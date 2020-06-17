<template lang="pug">
  base-dialog(v-if="visible" @close="$emit('close')")
    div(slot="title") Проект
    div(slot="body")
      el-form(:model="form" :rules="rules" ref="form" v-loading="loading")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="name")
              el-input(v-model="form.name" size="medium" placeholder="Новый проект")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="description")
              el-input(v-model="form.description" size="medium" type="textarea" placeholder="Заметки")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="tags")
              el-select(
                v-model="form.tags"
                size="medium"
                placeholder="Теги"
                multiple filterable allow-create default-first-option)
        el-row(:gutter="20")
          el-col(:span="12")
            el-form-item(prop="ownerId")
              el-select(
                v-model="form.ownerId"
                size="medium"
                placeholder="Руководитель"
                :remote-method="searchUsers"
                filterable remote clearable default-first-option)
                el-option(v-for="item in userList" :key="item.id" :label="item.value" :value="item.id")
          el-col(:span="12")
            el-form-item(prop="teamId")
              el-select(
                v-model="form.teamId"
                size="medium"
                placeholder="Команда"
                :remote-method="searchTeams"
                filterable remote clearable default-first-option)
                el-option(v-for="item in teamList" :key="item.id" :label="item.value" :value="item.id")
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
    id: Number
  },
  mixins: [dialogMixin],
  data() {
    return {
      form: {
        name: '',
        description: '',
        tags: [],
        ownerId: null,
        ownerFio: null,
        teamId: null,
        teamName: null,
        groupId: null,
        groupName: null,
        creationDate: new Date()
      },
      rules: {
        name: [
          {
            required: true,
            message: 'Введите название проекта',
            trigger: 'blur'
          }
        ],
        ownerId: [
          { required: true, message: 'Укажите руководителя', trigger: 'blur' }
        ]
      }
    };
  },
  computed: {
    ...mapGetters({
      item: 'projects/getProject'
    })
  },
  methods: {
    ...mapActions({
      fetchItem: 'projects/fetchProject',
      createItem: 'projects/createProject',
      updateItem: 'projects/updateProject'
    })
  }
};
</script>
