import globalSetting from "./util.setting";

const requestUrl = {
  //登陆授权
  auth: globalSetting.host + "/auth/token",

  //operates
  getMenuOfOperate: globalSetting.host + "/api/zero/operates/GetMenuOfOperate",
  pageSearchOperate:
    globalSetting.host + "/api/zero/operates/PageSerachOperate",
  operateAddOrEdit:
    globalSetting.host + "/api/zero/operates/CreateOrEditOperate",
  removeOperate: globalSetting.host + "/api/zero/operates/RemoveOperate",

  //menu
  roleMenus: globalSetting.host + "/api/zero/menus/GetMenusByRole",
  pageSearchMenu: globalSetting.host + "/api/zero/menus/PageSerachMenu",
  menuAddOrEdit:globalSetting.host + "/api/zero/menus/CreateOrEditMenu",
  removeMenu:globalSetting.host + "/api/zero/menus/RemoveMenu",
};

export default requestUrl;
