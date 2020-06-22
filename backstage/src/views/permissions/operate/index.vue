<template>
  <d2-container>
    <div>
      <d2-crud :columns="columns" :data="data" :loading="loading" selection-row
        @selection-change="handleSelectionChange" :pagination="pagination"
        @pagination-current-change="paginationCurrentChange">
        <zero-permission slot="header" style="margin-bottom: 5px" @zero-addEdit="addOrEditRow" @zero-search="search">
        </zero-permission>
      </d2-crud>
    </div>
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
        data: [],
        columns: [{
            title: "按钮名称",
            key: "name"
          },
          {
            title: "标识",
            key: "unique"
          },
          {
            title: "备注",
            key: "remark"
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
        multipleSelection: [],
      }
    },
    mounted() {
      let vm = this;
      vm.pageSearch(vm.pagination.currentPage);
    },
    methods: {
      search() {
        let vm = this;
        vm.pagination.currentPage = 1;
        vm.pageSearch(vm.pagination.currentPage);
      },
      addOrEditRow(formName) {
      },
      pageSearch(pageCurrent) {
        let vm = this;
        var params = {
          pageIndex: pageCurrent,
          pageMax: vm.pagination.pageSize
        };
        util.http.post(
          util.requestUrl.pageSearchOperate,
          params,
          vm,
          function (response) {
            console.log(response)
            vm.data = response.datas;
            vm.pagination.total = response.total;
            vm.loading = false;
          }
        );
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
  }

</script>
