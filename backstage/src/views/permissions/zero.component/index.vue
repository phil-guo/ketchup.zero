<template>
  <div>
    <el-button v-if="isSearch" style="margin-right:-5px" v-show="isSearch" size="small" @click="search">查询</el-button>
    <el-button v-if="isInsert" style="margin-right:-5px" v-show="isInsert" size="small" @click="insert">添加</el-button>
    <el-button v-if="isEdit" style="margin-right:-5px" v-show="isEdit" size="small" @click="edit">编辑</el-button>
    <el-button v-if="isDelete" style="margin-right:-5px" v-show="isDelete" size="small" @click="remove">删除</el-button>
  </div>
</template>
<script>
  import util from "@/libs/util.js";
  import $ from "jquery";
  export default {
    data() {
      return {
        permissions: [],
        isInsert: false,
        isEdit: false,
        isSearch: false,
        isDelete: false
      };
    },
    mounted() {},
    methods: {
      search() {
        this.$emit("zero-search");
      },
      insert() {
        this.$emit("zero-insert");
      },

      edit() {
        this.$emit("zero-edit");
      },

      remove() {
        this.$emit("zero-remove");
      },

      getMenuOperate() {
        let vm = this;
        var params = {
          model: {
            roleId: parseInt(util.cookies.get(util.globalSetting.roleId)),
            menuId: parseInt(vm.$route.query.id)
          }
        };

        util.http.post(util.requestUrl.getMenuOfOperate, params, vm, function (
          response
        ) {
          vm.permissions = response.data.result;
          vm.showPermission();
        });
      },

      showPermission() {
        let vm = this;
        $.each(vm.permissions, (key, item) => {
          if (item == "10001") {
            vm.isInsert = true;
          } else if (item == "10002") {
            vm.isEdit = true;
          } else if (item == "10003") {
            vm.isSearch = true;
          } else if (item == "10004") {
            vm.isDelete = true;
          }
        });
      }
    }
  };

</script>
