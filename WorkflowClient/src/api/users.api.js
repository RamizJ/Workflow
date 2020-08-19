import httpClient from './httpClient';
import qs from 'qs';

export default {
  findOneById: id => httpClient.get(`/api/Users/Get/${id}`),
  findAll: params => httpClient.post(`/api/Users/GetPage`, params),
  findAllByIds: ids =>
    httpClient.get(`/api/Users/GetRange?${qs.stringify(ids)}`),
  createOne: item => httpClient.post(`/api/Users/Create`, item),
  updateOne: item => httpClient.put(`/api/Users/Update`, item),
  updateMany: items => httpClient.put(`/api/Users/UpdateRange`, items),
  deleteOne: id => httpClient.delete(`/api/Users/Delete?id=${id}`),
  deleteMany: ids => httpClient.patch(`/api/Users/DeleteRange`, ids),
  restoreOne: id => httpClient.patch(`/api/Users/Restore/${id}`),
  restoreMany: ids => httpClient.patch(`/api/Users/RestoreRange`, ids),
  resetPassword: (userId, newPassword) =>
    httpClient.patch(
      `/api/Users/ResetPassword/${userId}?newPassword=${newPassword}`
    ),
  isEmailExist: email =>
    httpClient.get(`/api/Users/IsEmailExist/${email}?email=${email}`),
  isUserNameExist: userName =>
    httpClient.get(`/api/Users/IsUserNameExist/${userName}`)
};
