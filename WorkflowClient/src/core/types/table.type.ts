import { Table } from 'element-ui'
import Entity from '@/core/types/entity.type'

class TableType extends Table {
  public selection: Entity[] = []
}

export default TableType
