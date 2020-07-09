<template lang="pug">
  el-dialog(
    :visible.sync="showDialog"
    custom-class="base-dialog"
    @closed="$emit('close')")
    div.header(slot="title")
      div.title
        slot(name="title")
      el-tooltip.submit(content="Сохранить" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="800")
        slot(name="submit")
      el-tooltip.close(content="Закрыть" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="800")
        slot(name="close")
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
  padding: 14px 16px 0;
}
.header {
  padding-bottom: 0;
}
.title {
  font-size: 24px;
  font-weight: 800;
  cursor: default;
}
.submit,
.close {
  position: absolute;
  top: 25px;
}
.submit {
  right: 90px;
}
.close {
  right: 35px;
}
.body {
  padding-top: 0;
}
.footer {
  display: flex;
  justify-content: space-between;
  margin-bottom: 10px;
  margin-left: -12px;
  margin-right: -12px;
}
</style>

<style lang="scss">
.base-dialog {
  border-radius: 12px !important;
  border-width: var(--border-width);
  border-color: var(--card-border);
  background-color: var(--card-background);
  .el-dialog__headerbtn {
    display: none;
  }
  .el-select,
  .el-autocomplete {
    width: 100%;
  }
  .el-form-item {
    margin-bottom: 16px;
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
    padding: 15px 20px 5px !important;
  }
  .el-upload-dragger {
    border-color: var(--popover-border);
    background-color: var(--card-background);
  }
  .el-upload,
  .el-upload-dragger {
    width: 100%;
    height: 120px;
  }
  .el-upload-dragger .el-icon-upload {
    font-size: 50px;
    margin: 10px 0 10px;
  }
  .extra {
    display: flex;
    justify-content: flex-end;
    i {
      height: 18px;
    }
  }
}
</style>
