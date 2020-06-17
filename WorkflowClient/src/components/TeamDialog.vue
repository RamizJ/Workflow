<template lang="pug">
  base-dialog(v-if="visible" @close="$emit('close')")
    div(slot="title") Команда
    div(slot="body")
      el-form(:model="form" :rules="rules" ref="form")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="name")
              el-input(v-model="form.name" size="medium" placeholder="Новая команда")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item(prop="description")
              el-input(v-model="form.description" size="medium" type="textarea" placeholder="Заметки")
        el-row(:gutter="20")
          el-col(:span="16")
            el-form-item(prop="teamMembers")
              el-select(
                v-model="form.teamMembers"
                size="medium"
                placeholder="Участники"
                :remote-method="searchUsers"
                multiple filterable remote clearable default-first-option)
                el-option(v-for="item in userList" :key="item.id" :label="item.value" :value="item.id")
          el-col(:span="8")
            el-form-item(prop="projectId")
              el-select(
                v-model="form.projectId"
                size="medium"
                placeholder="Проект"
                :remote-method="searchProjects"
                filterable remote clearable default-first-option)
                el-option(v-for="item in projectList" :key="item.id" :label="item.value" :value="item.id")

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
        projectId: null,
        teamMembers: []
      },
      rules: {
        name: [
          {
            required: true,
            message: 'Введите название команды',
            trigger: 'blur'
          }
        ]
      }
    };
  },
  computed: {
    ...mapGetters({
      item: 'teams/getTeam'
    })
  },
  methods: {
    ...mapActions({
      fetchItem: 'teams/fetchTeam',
      createItem: 'teams/createTeam',
      updateItem: 'teams/updateTeam'
    })
  }
};
</script>
