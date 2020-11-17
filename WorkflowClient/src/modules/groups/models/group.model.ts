import Entity from '@/core/types/entity.type'
import Project from '@/modules/projects/models/project.type'
import { Metadata } from '@/core/types/metadata.model'

export default class Group extends Entity {
  id?: number
  name?: string
  description?: string
  creationDate?: string
  isRemoved?: boolean
  ownerId?: string
  ownerFio?: string
  parentGroupId?: string
  children?: Array<Group>
  projects: Array<Project> = []
  metadataList: Array<Metadata> = []

  constructor(name?: string) {
    super()
    this.name = name
  }
}
