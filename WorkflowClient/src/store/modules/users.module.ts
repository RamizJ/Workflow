import { Action, getModule, Module, Mutation, VuexModule } from 'vuex-module-decorators';
// import { Message } from 'element-ui';
import store from '@/store';
import usersAPI from '@/api/users.api';
import User from '@/types/user.type';
import Query from '@/types/query.type';

@Module({
  dynamic: true,
  namespaced: true,
  name: 'usersModule',
  store
})
class UsersModule extends VuexModule {
  _users: User[] = [];
  _user: User | null = null;

  public get user() {
    return this._user;
  }
  public get users() {
    return this._users;
  }

  @Mutation
  setUser(user: User) {
    this._user = user;
  }

  @Mutation
  setUsers(users: User[]) {
    this._users = users;
  }

  @Action
  async findAll(query: Query): Promise<User[]> {
    const response = await usersAPI.findAll(query);
    const results = response.data as User[];
    this.context.commit('setUsers', results);
    return results;
  }

  @Action
  async findOneById(id: string): Promise<User> {
    const response = await usersAPI.findOneById(id);
    const result = response.data as User;
    this.context.commit('setUser', result);
    return result;
  }
}

export default getModule(UsersModule);

// import usersAPI from '@/api/users.api';
// import { Message } from 'element-ui';
//
// export default {
//   namespaced: true,
//   state: () => ({
//     users: [],
//     user: {}
//   }),
//   mutations: {
//     setUsers(state, users) {
//       state.users = users;
//     },
//     setUser(state, user) {
//       state.user = user;
//     }
//   },
//   actions: {
//     async findOneById({ commit }, id) {
//       const response = await usersAPI.findOneById(id);
//       commit('setUser', response.data);
//       return response.data;
//     },
//     async findAll({ commit }, params) {
//       const response = await usersAPI.findAll(params);
//       commit('setUsers', response.data);
//       return response.data;
//     },
//     async findAllByIds({ commit }, ids) {
//       const response = await usersAPI.findAllByIds(ids);
//       commit('setUsers', response.data);
//       return response.data;
//     },
//     async createOne({ commit }, payload) {
//       const response = await usersAPI.createOne(payload);
//       commit('setUser', response.data);
//       return response.data;
//     },
//     async updateOne({ commit }, user) {
//       const response = await usersAPI.updateOne(user);
//       commit('setUser', response.data);
//       return response.data;
//     },
//     async updateMany({ commit }, users) {
//       const response = await usersAPI.updateMany(users);
//       commit('setUser', response.data);
//       return response.data;
//     },
//     async deleteOne({ commit }, id) {
//       await usersAPI.deleteOne(id);
//     },
//     async deleteMany({ commit }, ids) {
//       await usersAPI.deleteMany(ids);
//     },
//     async restoreOne({ commit }, id) {
//       await usersAPI.restoreOne(id);
//     },
//     async restoreMany({ commit }, ids) {
//       await usersAPI.restoreMany(ids);
//     },
//     async resetPassword({ commit }, { userId, newPassword }) {
//       await usersAPI.resetPassword(userId, newPassword);
//     },
//     async isLoginExist({ commit }, login) {
//       try {
//         const response = await usersAPI.isUserNameExist(login);
//         return response.data;
//       } catch (error) {
//         Message.warning('Не удалось проверить уникальность логина');
//         return false;
//       }
//     },
//     async isEmailExist({ commit }, email) {
//       try {
//         const response = await usersAPI.isEmailExist(email);
//         return response.data;
//       } catch (error) {
//         Message.warning('Не удалось проверить уникальность почтового ящика');
//         return false;
//       }
//     }
//   },
//   getters: {
//     getUsers: state => state.users,
//     getUser: state => state.user
//   }
// };
