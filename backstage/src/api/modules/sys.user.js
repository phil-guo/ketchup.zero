import { find, assign } from 'lodash'
import util from '@/libs/util.js'

export default ({ service, request, serviceForMock, requestForMock, mock, faker, tools }) => ({
  /**
   * @description 登录
   * @param {Object} data 登录携带的信息
   */
  SYS_USER_LOGIN (data = {}) {    
    // 接口请求
    return request({
      url: util.requestUrl.auth,
      method: 'post',
      data
    })
  }
})
