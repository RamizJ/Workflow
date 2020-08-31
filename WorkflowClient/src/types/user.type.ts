import Entity from '@/types/entity.type'

export default class User extends Entity {
  id?: string
  firstName?: string
  middleName?: string
  lastName?: string
  userName?: string
  password?: string
  email?: string
  phone?: string
  positionId?: number
  position?: string
  isRemoved?: boolean
  roles?: string[]

  teamIds?: number[]
}
