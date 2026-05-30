// Global variables
let userTableGridApi;
let dateBirthEdit;
let userNameList = [];
let selectedUserName = '';

document.addEventListener('DOMContentLoaded', async function () {
    var myGrid = document.querySelector("#myGrid");
    await getUserPermissions();

    const dateBirthElem = document.querySelector('#dateBirthEdit');
    dateBirthEdit = new Datepicker(dateBirthElem, DatePickerOptions());

    const myTheme = agGrid.themeQuartz.withParams({
        spacing: 10,
        accentColor: 'blue',
        fontFamily: 'Nunito Sans',
        selectedRowBackgroundColor: 'rgba(0, 255, 0, 0.1)',
    });

    const columnDefs = [
        {
            headerName: 'Name',
            cellRenderer: UserCellRenderer,
            headerClass: "font-bold text-neutral",
            minWidth: 250,
            getQuickFilterText: (params) => {
                const fullName = params.data.fullName || params.data.name || '';
                const email = params.data.email || '';
                return `${fullName} ${email}`;
            },
        },
        {
            headerName: 'Role',
            field: "roleName",
            headerClass: "font-bold text-neutral",
        },
        {
            headerName: 'Sex',
            field: "biologicalSexStr",
            maxWidth: 120,
            headerClass: "font-bold text-neutral",
        },
        {
            headerName: 'Date of Birth',
            field: "birthDate",
            cellDataType: 'date',
            headerClass: "font-bold text-neutral",
            valueGetter: (params) => {
                return params.data.birthDate ? new Date(params.data.birthDate) : null;
            },
            valueFormatter: (params) => {
                if (!params.value) return '';
                const date = params.value;
                const month = String(date.getMonth() + 1).padStart(2, '0');
                const day = String(date.getDate()).padStart(2, '0');
                const year = date.getFullYear();
                return `${month}/${day}/${year}`;
            },
            maxWidth: 160,
            filter: true,
        },
        {
            headerName: 'Status',
            cellRenderer: StatusBadge,
            headerClass: "font-bold text-neutral",
            maxWidth: 160,
        },
        {
            headerName: 'Address',
            field: "address",
            headerClass: "font-bold text-neutral",
            filter: true,
        },  

    ];   

    if (hasPermission("UAC_ADD_USER")) {
        columnDefs.push({
            colId: "actions",
            headerName: "Actions",
            headerClass: "font-bold text-neutral",
            cellRenderer: ButtonEdit,
            maxWidth: 150,
        },)
    }

    function ButtonEdit(params) {
        const eButton = document.createElement('button');
        eButton.className = 'btn btn-sm btn-primary btn-outline';

        const svgNS = "http://www.w3.org/2000/svg";
        const svg = document.createElementNS(svgNS, "svg");
        svg.setAttribute("viewBox", "0 0 24 24");
        svg.setAttribute("fill", "none");
        svg.setAttribute("stroke", "currentColor");
        svg.setAttribute("stroke-width", "1.5");
        svg.setAttribute("class", "size-5");

        const path = document.createElementNS(svgNS, "path");
        path.setAttribute("stroke-linecap", "round");
        path.setAttribute("stroke-linejoin", "round");
        path.setAttribute("d", "m16.862 4.487 1.687-1.688a1.875 1.875 0 1 1 2.652 2.652L10.582 16.07a4.5 4.5 0 0 1-1.897 1.13L6 18l.8-2.685a4.5 4.5 0 0 1 1.13-1.897l8.932-8.931Zm0 0L19.5 7.125M18 14v4.75A2.25 2.25 0 0 1 15.75 21H5.25A2.25 2.25 0 0 1 3 18.75V8.25A2.25 2.25 0 0 1 5.25 6H10");

        svg.appendChild(path);

        eButton.textContent = "Manage"
        eButton.appendChild(svg)
        this.eventListener = () => {
            if (params && params.data) {
                var rowData = params.data;
                populateUserModal(rowData)
            } else {
                console.error("Failed to load user data");
            }
        };
        eButton.addEventListener('click', this.eventListener);

        return eButton;
    }

    function UserCellRenderer(params) {
        const userData = params.value || params.data;
        const fullName = userData.fullName || userData.name || 'N/A';
        const email = userData.email || '';
        const avatarId = userData.profilePictureFileRecordId; 
        const avatarUrl = avatarId != null ? `/Setting/GetProfilePicture/${avatarId}` : null;

        return `
            <div class="flex items-center gap-3 h-full">
            ${avatarId ?
                `<div class="avatar">
                    <div class="w-10 rounded-full">
                        <img src="${avatarUrl}" alt="${fullName}" />
                     </div>
                  </div>` 
                : `<div class="avatar placeholder">
                      <div class="bg-neutral text-neutral-content w-10 rounded-full">
                         <span class="text-lg">${fullName.charAt(0).toUpperCase()}</span>
                      </div>
                    </div>`
            }
              <div class="flex flex-col">
                <span class="font-semibold text-sm">${fullName}</span>
                <span class="text-xs text-gray-500">${email}</span>
              </div>
            </div>
          `;
    }

    function StatusBadge(params) {
        const statusData = params.value || params.data;
        const statusBadge = userStatusColorMap[statusData.statusColor] || userStatusColorMap['gray'];
        return `
            <div class="badge ${statusBadge.text} ${statusBadge.bg} border-none font-semibold">
            ${statusData.status}
            </div>
            `
    }

    const gridOptions = {
        defaultColDef: {
            flex: 1,
            minWidth: 100,
            editable: false,
            resizable: true,
        },
        columnDefs,
        // pagination: true,
        theme: myTheme,
        rowHeight: 70,
        rowClass: "flex items-center",
        //rowSelection: {
        //    mode: 'multiRow',
        //},

    };

    const gridDiv = document.querySelector("#myGrid");
    userTableGridApi = agGrid.createGrid(gridDiv, gridOptions);

    loadUserData();

    document.getElementById('userSearch').addEventListener('input', (e) => {
        userTableGridApi.setGridOption('quickFilterText', e.target.value);
    });
});

function loadUserData() {
    axios.get("/Setting/Users")
        .then((res) => {
            let userList = res.data || [];
            if (res.data) {
                userNameList = userList.map(u => u.userName.toLowerCase());
            }
            return userTableGridApi.setGridOption("rowData", res.data)
        })
        .catch(function (error) {
            console.error(error);
        });
}
function populateUserModal(serverData) {
    document.getElementById('modalManageUser').showModal();
    const statusBadgeClass = userStatusColorMap[serverData.statusColor] || userStatusColorMap['gray'];

    try {
        const userDataElement = document.querySelector('[x-data="userForm"]');
        const userAlpinedata = Alpine.$data(userDataElement);
        const statusPill = document.querySelector('#statusPill');
        statusPill.className = `badge badge-lg ${statusBadgeClass.text} ${statusBadgeClass.bg} border-none font-semibold mb-1`;
        statusPill.textContent = serverData.status;

        const profileImg = document.querySelector('#profilePicImg');
        if (serverData.profilePictureFileRecordId != null) {
            profileImg.src = `/Setting/GetProfilePicture/${serverData.profilePictureFileRecordId}`;
        } else {
            profileImg.src = `${pathBase}/placeholders/avatar_placeholder.png`;
        }     

        if (serverData && userAlpinedata) {
            selectedUserName = serverData.userName;
            userAlpinedata.userId = serverData.userId;
            userAlpinedata.email = serverData.email;
            userAlpinedata.userName = serverData.userName;
            userAlpinedata.fullName = serverData.fullName;
            userAlpinedata.address = serverData.address;
            userAlpinedata.firstName = serverData.firstName;
            userAlpinedata.middleName = serverData.middleName;
            userAlpinedata.lastName = serverData.lastName;
            userAlpinedata.selectSex = serverData.biologicalSex;
            userAlpinedata.mobileNumber = serverData.mobileNumber; 
            userAlpinedata.selectRole = serverData.roleId; 
            userAlpinedata.selectStatus = serverData.userStatusId; 
            dateBirthEdit.setDate(serverData.birthDateInput);
            userAlpinedata.birthdateEdit = dateBirthEdit; 
            userAlpinedata.profilePicId = serverData.profilePictureFileRecordId;
        }

    } catch (ex) {
        console.error("selected user modal failed to load", ex);
    }
}

function closeManageUserModal() {
    try {
        const userDataElement = document.querySelector('[x-data="userForm"]');
        const userAlpinedata = Alpine.$data(userDataElement);
        userAlpinedata.userId = 0;
        userAlpinedata.email = '';
        userAlpinedata.userName = '';
        userAlpinedata.fullName = '';
        userAlpinedata.address = '';
        userAlpinedata.firstName = '';
        userAlpinedata.middleName = '';
        userAlpinedata.lastName = '';
        userAlpinedata.selectSex = '';
        userAlpinedata.mobileNumber = '';
        userAlpinedata.selectRole = '';
        userAlpinedata.selectStatus = ''; 
        userAlpinedata.birthdateEdit = null;
        dateBirthEdit.setDate(null);
        userAlpinedata.userNameError = ''; 
        userAlpinedata.firstNameError = ''; 
        userAlpinedata.lastNameError = ''; 
        userAlpinedata.selectSexError = ''; 
        userAlpinedata.selectRoleError = ''; 
        userAlpinedata.selectStatusError = ''; 

    } catch (ex) {
        console.error("manage user modal failed to close", ex);
    } finally {
        document.getElementById('modalManageUser').close();
    }
}

document.addEventListener('alpine:init', () => {

    Alpine.data('userForm', () => ({
        userId: 0,
        email: '',
        userName: '',
        firstName: '',
        middleName: '',
        lastName: '',
        selectSex: '',
        birthdate: '' || null,
        birthdateEdit: '' || null,
        mobileNumber: '',
        address: '',
        selectRole: '',
        selectStatus: '',
        fullName: '',
        profilePicId: null,
        emailError: '',
        userNameError: '',
        firstNameError: '',
        lastNameError: '',
        selectSexError: '',
        selectRoleError: '',
        selectStatusError: '',
        loading: false,

        async init() {
            const dateBirthElem = document.querySelector('#dateBirthNew');
            const dateBirth = new Datepicker(dateBirthElem, DatePickerOptions());
            this.birthdate = dateBirth;
        },

        validateNewUser(input) {

            switch (input) {
                case "email":
                    this.emailError = '';
                    if (!this.email.trim()) {
                        this.emailError = 'Email is required';
                        return false;
                    };
                    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.email)) {
                        this.emailError = 'Invalid email format';
                        return false;
                    };
                    return true;

                case "firstName":
                    this.firstNameError = '';
                    if (!this.firstName.trim()) {
                        this.firstNameError = 'First Name is required';
                        return false;
                    };
                    return true;

                case "lastName":
                    this.lastNameError = '';
                    if (!this.lastName.trim()) {
                        this.lastNameError = 'Last Name is required';
                        return false;
                    };
                    return true;

                case "role":
                    this.selectRoleError = '';
                    if (!this.selectRole) {
                        this.selectRoleError = 'Role is required';
                        return false;
                    };
                    return true;

                case "status":
                    this.selectStatusError = '';
                    if (!this.selectStatus) {
                        this.selectStatusError = 'Status is required';
                        return false;
                    };
                    return true;

                case "sex":
                    this.selectSexError = '';
                    if (!this.selectSex) {
                        this.selectSexError = 'Biological Sex is required';
                        return false;
                    };
                    return true;

                default:
                    return true;
            }

        },

        openRegisterModal() {
            this.resetForm();
            document.getElementById('modalAddUser').showModal();
        },

        validateAll(inputFields) {
           
            let invalidCount = 0;
            for (const input of inputFields) {
                let isValid = this.validateNewUser(input);
                if (!isValid) {
                    invalidCount++;
                }
            }
            return invalidCount == 0 ? true : false;
        },

        resetForm() {
            userId = 0,
            email = '',
                userName = '',
                firstName = '',
                lastName = '',
                selectSex = '',
                birthDate = null,
                address = '',
                selectRole = '',
                selectStatus = '',
                emailError = '',
                userNameError = '',
                firstNameError = '',
                lastNameError = '',
                selectSexError = '',
                selectRoleError = '',
                selectStatusError = '',
                this.loading = false;
            document.getElementById('modalAddUser').close();
        },

        async submitUser() {
            const inputFields = ['email', 'firstName', 'lastName', 'role', 'status', 'sex']
            if (!this.validateAll(inputFields)) {
                return
            }

            const newUser = {
                Email: this.email,
                UserName: this.userName,
                FirstName: this.firstName,
                LastName: this.lastName,
                RoleId: parseInt(this.selectRole),
                UserStatusId: parseInt(this.selectStatus),
                BiologicalSex: parseInt(this.selectSex),
                BirthDate: this.birthdate.getDate("yyyy-mm-dd"),
                Address: this.address,
            }

            this.loading = true;
            const result = await registerUser(newUser);

            if (result.success) {
                this.resetForm();
                loadUserData();
                showToastify('success', result.message.general)

            } else {
                this.loading = false;
                if (result.message.general) {
                    this.resetForm();
                    showToastify('error', result.message.general)
                } if (result.message.email) {
                    this.emailError = result.message.email;
                } if (result.message.username) {
                    this.userNameError = result.message.username;
                }
            }
        },
        async updateUser() {
            const inputFields = ['firstName', 'lastName', 'role', 'status', 'sex']
            if (!this.validateAll(inputFields)) {
                return
            }

            if (userNameList.includes(this.userName.toLowerCase())
                && this.userName.toLowerCase() != selectedUserName.toLowerCase()) {
                this.userNameError = 'Username name already exists';
                return;
            }

            const editUser = {
                UserId: this.userId,
                Email: this.email,
                UserName: this.userName,
                FirstName: this.firstName,
                MiddleName: this.middleName,
                LastName: this.lastName,
                RoleId: parseInt(this.selectRole),
                UserStatusId: parseInt(this.selectStatus),
                BiologicalSex: parseInt(this.selectSex),
                BirthDate: this.birthdateEdit.getDate("yyyy-mm-dd"),
                MobileNumber: this.mobileNumber,
                Address: this.address,
            }

            this.loading = true;
            var res = await updateUser(editUser);
            if (res.success) {
                loadUserData();
                showToastify('success', res.message.general);
                document.getElementById('modalConfirmManageUser').close();
                document.getElementById('modalManageUser').close();

            } else {

                if (res.message.general) {
                    showToastify('error', res.message.general)
                } else if (res.message.username) {
                    this.userNameError = res.message.username;
                    showToastify('error', res.message.username)
                } else {
                    showToastify('error', "Update Failed. Request cannot be processed this time")
                }
            }

            this.loading = false;
        },
        
    }));

});

async function registerUser(newUser) {
    const antiForgToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
    try {
        const res = await axios.post('/Setting/AddUser', newUser,
            {
                headers: {
                    'Content-Type': 'application/json',
                    "RequestVerificationToken": antiForgToken
                }
            }
        )

        return {
            success: res.data.isUserAdded,
            message: res.data.message
        }

    } catch (error) {
        let errMsg;

        if (error.response?.data &&
            (error.response?.status == 400 || error.response?.status == 200)) {
            var err = error.response.data
            errMsg = err.message
        }
        if (error.response?.status == 403) {
            errMsg = { general: "You are not allowed to Add User" }
        }
           
        return {
            success: false,
            message: errMsg
        };
    }
}

async function updateUser(editUser) {
    const antiForgToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
    try {
        const res = await axios.post('/Setting/UpdateUserfromAdmin', editUser,
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
        let errMsg = 'An error occurred while updating the user. Please try again later.';
        if (error.response?.data?.message)
            errMsg = error.response.data.message;
        return {
            success: false,
            message: errMsg
        };
    }
}

