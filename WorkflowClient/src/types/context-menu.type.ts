import Vue from 'vue'

export interface ContextMenu extends Vue {
  open: (event: Event, data: object) => void
}
