export default interface Attachment {
  id: number
  name?: string
  fileName: string
  fileSize: number
  fileType?: string
  creationDate: Date
  fileDataId: number
}
