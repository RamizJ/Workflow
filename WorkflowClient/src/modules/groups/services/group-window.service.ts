import { ElForm } from 'element-ui/types/form'
import WindowService from '@/core/services/window.service'
import Group from '@/modules/groups/models/group.model'
import groupsStore from '../store/groups.store'
import { Message } from 'element-ui'

export default class GroupWindowService extends WindowService {
  public entity: Group

  constructor(entity: Group = new Group(), form?: ElForm) {
    super(form)
    this.entity = entity
    if (groupsStore.group) this.entity = { ...groupsStore.group }
  }

  public async submit(): Promise<void> {
    const isFormValid = await this.validateForm()
    if (isFormValid) await this.saveEntity()
  }

  public async validateForm(): Promise<boolean> {
    const isFormValid = await this.form?.validate()
    if (!isFormValid)
      Message({ showClose: true, message: 'Форма заполнена некорректно', type: 'error' })
    return !!isFormValid
  }

  public async saveEntity(): Promise<void> {
    this.loading = true
    const isNewEntity = !this.entity.id
    if (isNewEntity) await groupsStore.create(this.entity)
    else await groupsStore.update(this.entity)
    this.loading = false
  }
}
