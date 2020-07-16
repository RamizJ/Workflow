<template lang="pug">
  base-dialog(v-if="visible" @close="exit" ref="dialog")
    h1(slot="title") Проект
    el-form(slot="body" :model="form" :rules="rules" ref="form" v-loading="loading" @submit.native.prevent="submit")
      el-row(:gutter="20")
        el-col(:span="24")
          el-form-item(prop="name")
            el-input(ref="title" v-model="form.name" placeholder="Новый проект")
      el-row(:gutter="20")
        el-col(v-if="descriptionVisible || form.description" :span="24")
          el-form-item(prop="description")
            el-input(v-model="form.description" :autosize="{ minRows: 2 }" type="textarea" placeholder="Описание")
      el-row(:gutter="20")
        transition(name="fade")
          el-col(v-if="teamsVisible || (form.teamIds && form.teamIds.length)" :span="24")
            el-form-item(prop="teamIds")
              el-select(
                v-model="form.teamIds"
                placeholder="Команды"
                :remote-method="searchTeams"
                multiple filterable remote clearable default-first-option)
                el-option(v-for="item in teamList" :key="item.id" :label="item.value" :value="item.id")
    template(slot="footer")
      div.extra
        el-tooltip(content="Описание" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(v-if="!form.description" type="text" title="Теги" @click="descriptionVisible = !descriptionVisible" circle)
            feather(type="align-left")
        el-tooltip(content="Команды" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(v-if="!(form.teamIds && form.teamIds.length)" type="text" title="Теги" @click="teamsVisible = !teamsVisible" circle)
            feather(type="users")
      div.send
        el-tooltip(content="Сохранить" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(type="text" @click="submit" circle)
            feather(type="arrow-right")
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseDialog from '~/components/BaseDialog';
import dialogMixin from '~/mixins/dialog.mixin';

export default {
  components: { BaseDialog },
  mixins: [dialogMixin],
  data() {
    return {
      form: {
        name: '',
        description: '',
        ownerId: null,
        ownerFio: null,
        creationDate: new Date(),
        groupId: null,
        groupName: null,
        teamIds: []
      },
      rules: {
        name: [{ required: true, message: '!', trigger: 'blur' }]
      },
      descriptionVisible: null,
      teamsVisible: null
    };
  },
  computed: {
    ...mapGetters({
      projectTeams: 'projects/getProjectTeams'
    })
  },
  async mounted() {
    this.loading = true;
    await this.searchTeams();
    if (this.isEdit) {
      await this.fetchProjectTeams({
        projectId: this.data.id,
        pageNumber: 0,
        pageSize: 10
      });
      this.form.teamIds = this.projectTeams.map(team => team.id);
    }
    this.loading = false;
  },
  methods: {
    ...mapActions({
      fetchItem: 'projects/fetchProject',
      createItem: 'projects/createProject',
      updateItem: 'projects/updateProject',
      fetchProjectTeams: 'projects/fetchProjectTeams'
    })
  }
};
</script>
