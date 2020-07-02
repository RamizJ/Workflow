import httpClient from './httpClient';
import qs from 'qs';

export default {
  get: id => httpClient.get(`/api/Users/Get/${id}`),
  create: user => httpClient.post(`/api/Users/Create`, user),
  update: user => httpClient.put(`/api/Users/Update`, user),
  delete: id => httpClient.delete(`/api/Users/Delete`, id),
  resetPassword: id => httpClient.patch(`/api/Users/ResetPassword/${id}`),
  isEmailExist: email => httpClient.get(`/api/Users/IsEmailExist/${email}`),
  isUserNameExist: userName =>
    httpClient.get(`/api/Users/IsUserNameExist/${userName}`),
  getPage: query =>
    httpClient.get(
      `/api/Users/GetPage${qs.stringify(query, { addQueryPrefix: true })}`
    ),
  getRange: query =>
    httpClient.get(
      `/api/Users/GetRange${qs.stringify(query, { addQueryPrefix: true })}`
    )
};
