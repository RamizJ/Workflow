import { ElForm } from 'element-ui/types/form'

export default class WindowService {
  public loading = false
  public form?: ElForm

  constructor(form?: ElForm) {
    this.form = form
  }
}
