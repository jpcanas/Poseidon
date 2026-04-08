let roleTableGridApi;
var rolePermissionAlpine;
let rolePermissionNames = [];
let selectedRoleName = '';

document.addEventListener('DOMContentLoaded', async function () {
    const myTheme = agGrid.themeQuartz.withParams({
        spacing: 10,
        accentColor: 'blue',
        fontFamily: 'Nunito Sans',
        selectedRowBackgroundColor: 'rgba(0, 255, 0, 0.1)',
    });

    await getUserPermissions();

    const columnDefs = [
        {
            headerName: 'Role Name',
            field: "roleName",
            maxWidth: 450,
            filter: true,
        },
        {
            headerName: 'Description',
            field: "description",
        },
       
    ];

    if (hasPermission("UAC_ASSIGN_ROLES")) {
        columnDefs.push({
            colId: "actions",
            headerName: "Actions",
            cellRenderer: ViewPermissionBtn,
            maxWidth: 180,
        },)
    }

    function ViewPermissionBtn(params) {
        const eButton = document.createElement('button');
        eButton.className = 'btn btn-sm btn-primary btn-outline';
        eButton.textContent = "View Permission"

        this.eventListener = async () => {
            var rowData = params.data;
            const permission = await loadPermissions(rowData.roleId)
            if (permission && permission.data) {
                populatePermissionModal(permission.data);
            } else {
                console.error("Failed to load permissions for roleId", rowData.roleId);
            }
        };
        eButton.addEventListener('click', this.eventListener);

        return eButton;
    }

    const gridOptions = {
        defaultColDef: {
            flex: 1,
            minWidth: 100,
            editable: false,
            resizable: true,
            headerClass: "font-bold text-neutral",
        },
        columnDefs,
        // pagination: true,
        theme: myTheme,
        rowHeight: 50,
        rowClass: "flex items-center",
        //rowSelection: {
        //    mode: 'multiRow',
        //},

    };

    const gridDiv = document.querySelector("#roleTable");
    roleTableGridApi = agGrid.createGrid(gridDiv, gridOptions);

    loadRolesPermission();

    document.getElementById('roleSearch').addEventListener('input', (e) => {
        roleTableGridApi.setGridOption('quickFilterText', e.target.value);
    });

});

function loadRolesPermission() {
    axios.get("/Setting/GetRoles")
        .then((res) => {
            let rolePermissionList = res.data;
            if (res.data) {
                rolePermissionNames = rolePermissionList.map(role => role.roleName.toLowerCase());
            }
            return roleTableGridApi.setGridOption("rowData", res.data)
        })
        .catch(function (error) {
            console.log(error);
        });
}

// modal permission
document.addEventListener('alpine:init', () => {
    Alpine.data('permissionForm', () => ({

        form: {
            roleId: 0,
            roleName: '',
            description: '',
            isSystemRole: false,
        },

        permissions: [],

        roleNameError: '',
        isSubmitting: false,

        async init() {
            const { data, status } = await loadPermissions(0);

            if (status == 200) {
                this.form.roleId = data.roleId;
                this.form.roleName = data.roleName;
                this.form.description = data.description;
                this.form.isSystemRole = data.isSystemRole;

                if (data.permissions.length > 0) {
                    this.permissions = data.permissions
                }
            }
        },

        togglePermission(index) {
            const toggledPermission = this.permissions.find(m => m.moduleId == index);
            if (toggledPermission) {
                toggledPermission.enabled = !toggledPermission.enabled;
            }
            toggledPermission.subModules.forEach(sub => 
                sub.isAssigned = toggledPermission.enabled
            )
        },

        toggleSubModule(index) {
            const toggledPermission = this.permissions.find(m => m.moduleId == index);
            var hasCheckedSubModule = toggledPermission.subModules.some(s => s.isAssigned);
            if (!hasCheckedSubModule) {
                toggledPermission.enabled = false;
            }
        },

        checkRoleName() {
            this.roleNameError = '';
            if (this.form.roleId == 0) {
                selectedRoleName = ''
            }

            if (!this.form.roleName.trim()) {
                this.roleNameError = 'Role name is required';
                return false;
            }
            if (rolePermissionNames.includes(this.form.roleName.toLowerCase())
                && this.form.roleName.toLowerCase() != selectedRoleName.toLowerCase()) {
                this.roleNameError = 'Role name already exists';
                return false;
            }

            return true;
        },

        closeModal() {
            document.getElementById('modalAddRole').close()
        },

        openModal() {
            document.getElementById('btnSubmitRole').innerText = 'Create Role';
            document.getElementById('roleModalHeader').textContent = "Add Custom Role";
            this.resetForm();
            document.getElementById('modalAddRole').showModal();
        },
        confirmSubmit() {
            if (!this.checkRoleName()) {
                return
            }
            if (this.form.roleId == 0) {
                document.getElementById('txtConfirmTitle').textContent = "Create Role?";
                document.getElementById('txtConfirmMsg').textContent = `You're about to create ${this.form.roleName} with the selected permissions. Do you want to continue?`;
            } else {
                document.getElementById('txtConfirmTitle').textContent = "Save Changes?";

                var loggedUserRoleId = document.getElementById('loggedUserRoleId').value;
                if (loggedUserRoleId && parseInt(loggedUserRoleId) == this.form.roleId) {
                    document.getElementById('txtConfirmMsg').innerHTML = "You're about to update role details and permissions for this role and will be redirected to home page to reflect the changes. <br> Do you want to continue?";
                } else {
                    document.getElementById('txtConfirmMsg').textContent = "You're about to update role details and permissions for this role. Do you want to continue?";
                }
                
            }
            document.getElementById('modalConfirmSaveRole').showModal();
        },

        resetForm() {
            this.form.roleId = 0;
            this.form.roleName = '';
            this.form.description = '';
            this.form.isSystemRole = false;
            this.roleNameError = '';

            this.permissions.forEach(m => {
                m.enabled = false;
                m.subModules.forEach(sub => {
                    sub.isAssigned = false;
                });
            });
        },

        async submitForm() {
            this.isSubmitting = true;
            const checkedSubModuleIds = this.permissions
                .flatMap(m => m.subModules)  // Flatten all submodules into one array
                .filter(sub => sub.isAssigned)             
                .map(sub => sub.subModuleId);  

            const payload = {
                roleId: this.form.roleId,
                roleName: this.form.roleName,
                description: this.form.description,
                subModuleIds: checkedSubModuleIds
            };

            const { success, message } = await saveRolePermission(payload);

            if (success) {
                loadRolesPermission();
                showToastify('success', message)
            } else {
                showToastify('error', message)
            }

            this.isSubmitting = false;

            var loggedUserRoleId = document.getElementById('loggedUserRoleId').value;
            if (loggedUserRoleId && parseInt(loggedUserRoleId) == this.form.roleId) {
                window.location.replace("/Home/Index");
            } else {
                document.getElementById('modalConfirmSaveRole').close()
                this.closeModal()
            }

        }

    }))
})

async function loadPermissions(roleId) {

    try {
        const res = await axios.get(`/Setting/GetPermissions/${roleId}`)
        return res; //status //data

    } catch (error) {
        return null;
    }
}

function populatePermissionModal(serverData) {
    document.getElementById('btnSubmitRole').innerText = 'Update Role';
    document.getElementById('roleModalHeader').textContent = "Edit Role Permission";
    document.getElementById('modalAddRole').showModal();

    try {
        const permissionDataElement = document.querySelector('[x-data="permissionForm"]');
        const rolePermissionAlpinedata = Alpine.$data(permissionDataElement);

        if (serverData && rolePermissionAlpinedata) {
            selectedRoleName = serverData.roleName.toLowerCase();
            rolePermissionAlpinedata.roleNameError = '';
            rolePermissionAlpinedata.form.roleId = serverData.roleId;
            rolePermissionAlpinedata.form.roleName = serverData.roleName;
            rolePermissionAlpinedata.form.description = serverData.description;
            rolePermissionAlpinedata.form.isSystemRole = serverData.isSystemRole;

            rolePermissionAlpinedata.permissions = serverData.permissions;
        }

    } catch (ex) {
        console.error("populatePermissionModal failed", ex);
    }
}

async function saveRolePermission(roleRequest) {
    const antiForgToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
    try {
        const res = await axios.post('/Setting/SaveRolePermission', roleRequest,
            {
                headers: {
                    'Content-Type': 'application/json',
                    "RequestVerificationToken": antiForgToken
                }
            }
        )

        return {
            success: res.data.success,
            message: res.data.message
        }

    } catch (error) {
        let err;

        if (error.response?.data)
            err = error.response.data

        return {
            success: false,
            message: err.message
        };
    }
}
