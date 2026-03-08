// Global variables
let userTableGridApi;

document.addEventListener('DOMContentLoaded', async function () {
    var myGrid = document.querySelector("#myGrid");
    await getUserPermissions();

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
        },
        {
            headerName: 'Role',
            field: "roleName",
            headerClass: "font-bold text-neutral",
        },
        {
            headerName: 'Sex',
            field: "biologicalSex",
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
            field: "status",
            headerClass: "font-bold text-neutral",
            maxWidth: 160,
            filter: true,
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
        this.eventListener = () => console.log('params', params);
        eButton.addEventListener('click', this.eventListener);

        return eButton;
    }

    function UserCellRenderer(params) {
        const userData = params.value || params.data;
        const fullName = userData.fullName || userData.name || 'N/A';
        const email = userData.email || '';
        const avatarUrl = userData.avatarUrl || userData.avatar || ''; 

        return `
            <div class="flex items-center gap-3 h-full">
            ${avatarUrl ?
                `<div class="avatar">
                    <div class="w-10">
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

});

function loadUserData() {
    axios.get("/Setting/Users")
        .then((res) => {
            return userTableGridApi.setGridOption("rowData", res.data)
        })
        .catch(function (error) {
            console.error(error);
        });
}


document.addEventListener('alpine:init', () => {

    Alpine.data('userForm', () => ({
        email: '',
        userName: '',
        firstName: '',
        lastName: '',
        selectSex: '',
        birthDate: null,
        address: '',
        selectRole: '',
        selectStatus: '',
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
                    if (!this.selectRole.trim()) {
                        this.selectRoleError = 'Role is required';
                        return false;
                    };
                    return true;

                case "status":
                    this.selectStatusError = '';
                    if (!this.selectStatus.trim()) {
                        this.selectStatusError = 'Status is required';
                        return false;
                    };
                    return true;

                case "sex":
                    this.selectSexError = '';
                    if (!this.selectSex.trim()) {
                        this.selectSexError = 'Biological Sex is required';
                        return false;
                    };
                    return true;

                default:
                    return true;
            }

        },

        validateAll() {
            const inputFields = ['email', 'firstName', 'lastName', 'role', 'status', 'sex']
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

            if (!this.validateAll()) {
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

        }

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

