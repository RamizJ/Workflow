import moment from 'moment'

export default class Attachment {
  id?: number
  name?: string
  fileName?: string
  fileSize?: number
  fileType?: string
  creationDate: Date = moment().toDate()
  fileDataId?: number
}
