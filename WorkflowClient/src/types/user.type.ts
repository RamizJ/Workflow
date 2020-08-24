export default interface User {
  id: string;
  firstName: string;
  middleName?: string;
  lastName?: string;
  userName: string;
  email: string;
  phone?: string;
  positionId?: number;
  position?: string;
  isRemoved: boolean;
  roles: string[];

  index?: number;
}
