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
            el-input(v-model="form.description" :autosize="{ minRows: 2 }" type="textarea" placeholder="Заметки")
      el-row(:gutter="20")
        transition(name="fade")
          el-col(v-if="tagsVisible || (form.tags && form.tags.length)" :span="24")
            el-form-item(prop="tags")
              el-select(
                v-model="form.tags"
                placeholder="Теги"
                multiple filterable allow-create default-first-option)
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
        el-tooltip(content="Заметки" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(v-if="!form.description" type="text" title="Теги" @click="descriptionVisible = !descriptionVisible" circle)
            feather(type="align-left")
        el-tooltip(content="Теги" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(v-if="!(form.tags && form.tags.length)" type="text" title="Теги" @click="tagsVisible = !tagsVisible" circle)
            feather(type="tag")
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
  props: {
    id: Number
  },
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
        teamIds: [],
        tags: []
      },
      rules: {
        name: [{ required: true, message: '!', trigger: 'blur' }]
      },
      descriptionVisible: null,
      tagsVisible: null,
      teamsVisible: null
    };
  },
  computed: {
    ...mapGetters({
      item: 'projects/getProject',
      projectTeams: 'projects/getProjectTeams'
    })
  },
  async mounted() {
    await this.searchTeams();
    this.$refs.title.focus();
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
