<template>
  <d2-container>
    <el-row :gutter="5">
      <el-col :span="3">
        <el-input v-model="name" clearable placeholder="请输入名称" style="margin-bottom: 5px"></el-input>
      </el-col>
    </el-row>
    <d2-crud :columns="columns" :data="data" :loading="loading" selection-row @selection-change="handleSelectionChange" :pagination="pagination"
      @pagination-current-change="paginationCurrentChange">
      <zero-permission slot="header" style="margin-bottom: 5px" @zero-addEdit="addOrEditRow" @zero-search="search" @zero-remove="remove">
      </zero-permission>
    </d2-crud>
  </d2-container>
</template>
<script>
import zeroComponent from "@/views/permissions/zero.component/index.vue";
import util from "@/libs/util.js";
export default {
  components: {
    "zero-permission": zeroComponent
  },
  data() {
    return {
      //查询条件
      name: "",
      params: {},

      //table
      data: [],
      columns: [
        {
          title: "用户",
          key: "userName"
        },
        {
          title: "角色",
          key: "roleName"
        }
      ],
      loading: true,

      //分页
      pagination: {
        currentPage: 1,
        pageSize: 20,
        total: 0
      },
      // checkbox选择
      multipleSelection: []
    };
  },
  mounted() {
    let vm = this;
    vm.pageSearch(vm.pagination.currentPage);
  },
  methods: {
    remove() {
      let vm = this;
      if (vm.multipleSelection == null || vm.multipleSelection.length != 1) {
        this.$notify.error({
          title: util.globalSetting.operateErrorMsg,
          message: "请选取一行数据操作"
        });
      } else {
        this.$confirm("此操作将永久删除该记录, 是否继续?", "提示", {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning",
          center: true
        })
          .then(() => {
            let row = vm.multipleSelection[0];
            util.http.post(
              util.requestUrl.removeOperate,
              { id: row.id },
              vm,
              function(response) {
                vm.$notify({
                  title: "成功",
                  duration: 3000,
                  message: util.globalSetting.operateSuccessMsg,
                  type: "success"
                });
                vm.dialogFormVisible = false;
                vm.pageSearch(vm.pagination.currentPage);
              }
            );
          })
          .catch(() => {
            this.$message({
              type: "info",
              message: "已取消删除"
            });
          });
      }
    },
    search() {
      let vm = this;
      vm.pagination.currentPage = 1;
      if (vm.name != "" || vm.name != null) {
        vm.params.name = vm.name;
      }
      vm.pageSearch(vm.pagination.currentPage);
    },
    addOrEditRow(type) {},
    pageSearch(pageCurrent) {
      let vm = this;
      vm.params.pageIndex = pageCurrent;
      vm.params.pageMax = vm.pagination.pageSize;

      util.http.post(util.requestUrl.pageSerachSysUser, vm.params, vm, function(
        response
      ) {
        vm.data = response.datas;
        vm.pagination.total = response.total;
        vm.loading = false;
      });
    },
    paginationCurrentChange(currentPage) {
      let vm = this;
      vm.pagination.currentPage = currentPage;
      vm.pageSearch(currentPage);
    },
    handleSelectionChange(selection) {
      this.multipleSelection = selection;
    }
  }
};
</script>