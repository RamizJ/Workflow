export default interface Attachment {
  id: number;
  fileName: string;
  fileSize: number;
  fileType?: string;
  creationDate: Date;
  fileDataId: number;
}
