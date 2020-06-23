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
import $ from "jquery";
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
          title: "名称",
          key: "name"
        },
        {
          title: "图标",
          key: "icon"
        },
        {
          title: "级别",
          key: "level",
          formatter(row, column, cellValue, index) {
            if (cellValue == 1) {
              return "顶级";
            } else if (cellValue == 2) {
              return "一级";
            }
          }
        },
        {
          title: "路由",
          key: "url"
        },
        {
          title: "功能",
          key: "operateModels",
          formatter(row, column, cellValue, index) {
            var displayStr = "";
            $.each(cellValue, function(index, element) {
              displayStr += element.name + ",";
            });
            return displayStr;
          }
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
    search() {},
    addOrEditRow(type) {},
    remove() {},
    pageSearch(pageCurrent) {
      let vm = this;
      vm.params.pageIndex = pageCurrent;
      vm.params.pageMax = vm.pagination.pageSize;

      util.http.post(util.requestUrl.pageSearchMenu, vm.params, vm, function(
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