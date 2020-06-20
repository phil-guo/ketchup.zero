<template>
  <div>
    <el-button
      v-if="isSearch"
      style="margin-right:-5px"
      v-show="isSearch"
      size="small"
      @click="search"
    >查询</el-button>
  </div>
</template>
<script>
import util from "@/libs/util.js";
import $ from "jquery";
export default {
  data() {
    return {
      permissions: [],
      isSearch: false
    };
  },
  mounted() {},
  methods: {
    search() {
      this.$emit("zero-Search");
    },
    getMenuOperate() {
      let vm = this;
      var params = {
        model: {
          roleId: parseInt(util.cookies.get(util.globalSetting.roleId)),
          menuId: parseInt(vm.$route.query.id)
        }
      };

      util.http.post(util.requestUrl.getMenuOfOperate, params, vm, function(
        response
      ) {
        vm.permissions = response.data.result;
        vm.showButton();
      });
    }
  }
};
</script>