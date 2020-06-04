<template lang="pug">
  base-dialog(v-if="visible" @close="$emit('close')")
    div(slot="title") Команда
    div(slot="body")
      el-form(:model="form" :rules="rules" ref="form")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item
              el-input(v-model="form.title" size="medium" placeholder="Новая команда")
        el-row(:gutter="20")
          el-col(:span="24")
            el-form-item
              el-input(v-model="form.description" size="medium" type="textarea" placeholder="Заметки")
        el-row(:gutter="20")
          el-col(:span="16")
            el-form-item
              el-select(v-model="form.teamMembers" size="medium" placeholder="Участники" multiple)
                el-option(v-for="item in users" :key="item.value" :label="item.label" :value="item.value")
          el-col(:span="8")
            el-form-item
              el-select(v-model="form.scope" size="medium" placeholder="Проект")
                el-option(v-for="item in scopes" :key="item.value" :label="item.label" :value="item.value")
    div(slot="footer")
      el-button(size="medium" type="primary" @click="submit") Создать

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseDialog from '~/components/BaseDialog';

export default {
  components: { BaseDialog },
  props: {
    id: Number,
  },
  data() {
    return {
      visible: false,
      loading: false,
      isEdit: !!this.id,
      form: {
        title: "",
        priority: null,
        description: "",
        teamMembers: [],
        responsible: null,
        team: null,
        scope: null,
        dateStart: null,
        dateEnd: null
      },
      users: [
        { value: 0, label: "Виталий" },
        { value: 1, label: "Алексей" },
        { value: 2, label: "Иван" },
        { value: 3, label: "Константин" },
        { value: 4, label: "Олег" },
        { value: 5, label: "Николай" },
        { value: 6, label: "Андрей" }
      ],
      scopes: [
        { value: 0, label: "Виталий" },
        { value: 1, label: "Алексей" },
        { value: 2, label: "Андрей" }
      ],
      rules: {
        title: [
          {
            required: true,
            message: 'Пожалуйста, укажите название',
            trigger: 'blur',
          },
        ],
        responsible: [
          {
            required: true,
            message: 'Пожалуйста, укажите ответственного',
            trigger: 'blur',
          },
        ],
      },
    };
  },
  async mounted() {
    this.visible = true;
    if (this.isEdit) {
      this.loading = true;
      await this.fetchTeam(this.id);
      this.form = this.team;
      this.loading = false;
    }
  },
  computed: {
    ...mapGetters({ team: 'teams/getTeam' })
  },
  methods: {
    ...mapActions({
      fetchTeam: 'teams/fetchTeam',
      createTeam: 'teams/createTeam',
      updateTeam: 'teams/updateTeam'
    }),
    submit() {
      const payload = { ...this.form };
      const form = this.$refs.form;
      form.validate(async (valid) => {
        if (valid) {
          try {
            if (this.isEdit)
              await this.updateTask(payload);
            else
              await this.createTask(payload);
            form.resetFields();
            this.$emit('close');
          } catch (e) {
            this.$message.error('Ошибка отправки запроса');
          }
        } else {
          this.$message.error('Укажите корректные данные');
        }
      });
    }
  }
};
</script>
