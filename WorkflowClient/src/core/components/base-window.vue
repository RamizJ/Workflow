<template>
  <el-dialog
    ref="window"
    custom-class="window"
    :top="top || '10vh'"
    :width="width || '50%'"
    :visible.sync="isWindowOpened"
    :before-close="confirmClose"
    :append-to-body="appendToBody"
    @closed="onClosed"
  >
    <div class="window__header" slot="title">
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
        <el-button type="text" @click="onClose">
          <unicon name="times" />
        </el-button>
      </el-tooltip>
    </div>
    <div class="window__body">
      <slot name="body" />
      <div class="footer">
        <slot name="footer" />
      </div>
    </div>
  </el-dialog>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { MessageBox } from 'element-ui'
import { DialogType } from '@/core/types/dialog.type'
import settingsModule from '@/modules/settings/store/settings.store'

@Component
export default class BaseWindow extends Vue {
  @Prop() readonly appendToBody?: boolean
  @Prop() readonly width?: string
  @Prop() readonly top?: string
  private isWindowOpened = false

  protected mounted(): void {
    this.isWindowOpened = true
  }

  private confirmClose(done: CallableFunction): void {
    if (settingsModule.confirmDialogClose) {
      MessageBox.confirm('Вы действительно хотите закрыть окно?', 'Предупреждение', {
        confirmButtonText: 'Закрыть',
        cancelButtonText: 'Отменить',
        type: 'warning',
      }).then(() => {
        done()
      })
    } else done()
  }

  private onClose(): void {
    ;(this.$refs.dialog as DialogType).hide()
    this.$emit('close')
  }

  private onClosed(): void {
    this.isWindowOpened = false
    this.$emit('closed')
  }
}
</script>

<style lang="scss" scoped>
.window__header {
  padding: 14px 16px 0;
}
.window__body {
  padding: 0 16px;
}
.title {
  font-size: 24px;
  font-weight: 700;
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
.footer {
  display: flex;
  justify-content: space-between;
  margin-bottom: 10px;
  margin-left: -12px;
  margin-right: -12px;
}
</style>

<style lang="scss">
.window {
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
    border-color: var(--text-muted);
    background-color: var(--card-background);
  }
  .el-upload,
  .el-upload-dragger {
    width: 100%;
    height: 90px;
  }
  .el-upload-dragger .el-upload__text {
    line-height: 20px;
  }
  .el-upload-dragger .el-icon-upload {
    font-size: 40px;
    line-height: 40px;
    margin-top: 5px;
    margin-bottom: 0;
  }
  .close,
  .extra,
  .send {
    svg {
      fill: var(--color-primary);
    }
  }
  .extra {
    display: flex;
    justify-content: flex-end;
    svg {
      height: 20px;
      width: 20px;
    }
  }
  .close,
  .send {
    svg {
      height: 24px;
      width: 24px;
    }
  }
}
</style>
