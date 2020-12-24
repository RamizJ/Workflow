<template>
  <el-input
    class="chat-input"
    ref="chat-input"
    v-bind="$attrs"
    v-on="$listeners"
    placeholder="Напишите сообщение..."
  />
</template>

<script lang="ts">
import { Component, Prop, Ref, Vue } from 'vue-property-decorator'
import { ElInput } from 'element-ui/types/input'

@Component
export default class Chat extends Vue {
  @Ref('chat-input') readonly chatInput!: ElInput

  protected mounted(): void {
    this.chatInput.focus()
  }
}
</script>

<style lang="scss">
.chat-input {
  margin: 10px 0;
  border: none;
  border-top: var(--input-border);
  .el-input__inner {
    font-size: 13px;
    padding: 0;
    border: none;
    box-shadow: none;
    &:hover,
    &:focus {
      border: none !important;
      box-shadow: none !important;
    }
    &::placeholder {
      color: var(--text-muted);
    }
  }
}
</style>
