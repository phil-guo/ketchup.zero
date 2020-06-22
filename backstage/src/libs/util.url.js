import globalSetting from "./util.setting";

const requestUrl = {
  //登陆授权
  auth: globalSetting.host + "/auth/token",

  //operates
  getMenuOfOperate: globalSetting.host + "/zero/operates/GetMenuOfOperate",

  //menu
  roleMenus: globalSetting.host + "/zero/menus/GetMenusByRole"
};

export default requestUrl;
