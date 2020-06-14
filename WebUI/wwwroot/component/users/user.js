/// <reference path="../../lib/vue/vue.js" />
var app = new Vue({
    el: '#app',
    data: {
        selectedRoleState: 'watcher',
        roles: []
    },
    methods: {
        getSelectedRole: async function (id) {
            await axios.get('/api/getrolebyuserid?id=' + id).then(response => this.roles = response.data)
            if (this.roles.includes('manager')) {
                this.selectedRoleState = 'manager'
            }
            else {
                this.selectedRoleState = 'watcher'
            }
        }
    }
})