import { mapActions, mapGetters } from 'vuex';

export default {
  data() {
    return {
      visible: false,
      loading: false,
      isEdit: !!this.id
    };
  },
  async mounted() {
    if (this.isEdit) {
      this.loading = true;
      await this.fetchItem(this.id);
      this.form = this.item;
      this.loading = false;
    }
    this.visible = true;
  },
  computed: {
    ...mapGetters({ item: '' })
  },
  methods: {
    ...mapActions({
      fetchItem: '',
      createItem: '',
      updateItem: '',
      deleteItem: ''
    }),
    submit() {
      const payload = { ...this.form };
      const form = this.$refs.form;
      form.validate(async valid => {
        if (valid) {
          try {
            if (this.isEdit) await this.updateItem(payload);
            else await this.createItem(payload);
            form.resetFields();
            this.$emit('submit');
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
