<template lang="pug">
  el-dialog(:visible.sync="showDialog" custom-class="base-dialog" @closed="$emit('close')")
    div.header(slot="title")
      div.title
        slot(name="title")
    div.body
      slot(name="body")
      div.footer
        slot(name="footer")
</template>

<script>
export default {
  name: 'BaseDialog',
  data() {
    return {
      showDialog: false
    };
  },
  mounted() {
    this.showDialog = true;
  }
};
</script>

<style lang="scss" scoped>
.header,
.body {
  padding: 14px 18px;
}
.header {
  padding-bottom: 0;
}
.body {
  padding-top: 0;
}
.title {
  font-size: 24px;
  font-weight: 800;
  cursor: default;
}
.footer {
  margin-top: 10px;
  display: flex;
  justify-content: flex-end;
}
</style>

<style lang="scss">
.base-dialog {
  border-radius: 10px !important;
  border-width: var(--border-width);
  border-color: var(--card-border);
  background-color: var(--card-background);
  .el-select,
  .el-autocomplete {
    width: 100%;
  }
  .el-form-item {
    margin-bottom: 16px;
  }
  .el-form-item__error {
    font-size: 10px;
    padding-top: 0;
  }
  .el-form-item.is-error .el-input__inner,
  .el-form-item.is-error .el-input__inner:focus,
  .el-form-item.is-error .el-textarea__inner,
  .el-form-item.is-error .el-textarea__inner:focus,
  .el-message-box__input input.invalid,
  .el-message-box__input input.invalid:focus {
    background-color: #f56c6c26;
    border-color: transparent;
  }
  .el-range-editor.el-input__inner {
    width: 100%;
  }
  .el-date-editor.el-input,
  .el-date-editor.el-input__inner {
    width: auto;
  }
  .el-date-editor .el-input__prefix {
    left: unset;
    right: 5px;
  }
  .el-date-editor .el-input__inner {
    padding-left: 15px;
  }
  .el-dialog__headerbtn {
    top: 34px;
    right: 38px;
  }
  .el-dialog__body {
    padding: 15px 20px !important;
  }
}
</style>
