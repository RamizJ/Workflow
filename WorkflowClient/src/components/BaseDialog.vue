<template>
  <el-dialog
    ref="dialog"
    custom-class="base-dialog"
    :visible.sync="showDialog"
    :before-close="confirmClose"
    @closed="$emit('close')"
  >
    <div class="header" slot="title">
      <div class="title">
        <slot name="title" />
      </div>
      <el-tooltip
        class="close"
        content="Закрыть"
        effect="dark"
        placement="top"
        transition="fade"
        :visible-arrow="false"
        :open-delay="800"
      >
        <el-button type="text">
          <feather type="x" @click="close"></feather>
        </el-button>
      </el-tooltip>
    </div>
    <div class="body">
      <slot name="body" />
      <div class="footer">
        <slot name="footer" />
      </div>
    </div>
  </el-dialog>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import { MessageBox } from 'element-ui'

@Component
export default class BaseDialog extends Vue {
  private showDialog = true

  private confirmClose(done: Function) {
    const needConfirm = localStorage.confirmDialogClose === 'true'
    if (needConfirm) {
      MessageBox.confirm('Вы действительно хотите закрыть окно?', 'Предупреждение', {
        confirmButtonText: 'Закрыть',
        cancelButtonText: 'Отменить',
        type: 'warning'
      }).then(() => {
        done()
      })
    } else done()
  }

  private close() {
    ;(this.$refs.dialog as any).hide()
  }
}
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
  border-radius: 6px !important;
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
