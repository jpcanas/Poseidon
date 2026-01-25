// Global variables
let userTableGridApi;

document.addEventListener('DOMContentLoaded', function () {
    var myGrid = document.querySelector("#myGrid");

    const myTheme = agGrid.themeQuartz.withParams({
        spacing: 10,
        accentColor: 'blue',
        fontFamily: 'Nunito Sans',
        selectedRowBackgroundColor: 'rgba(0, 255, 0, 0.1)',
    });

    const columnDefs = [
        {
            headerName: 'Name',
            field: "fullName",
            minWidth: 250,
            filter: true,
        },
        {
            headerName: 'Role',
            field: "roleName",
        },
        {
            headerName: 'Sex',
            field: "biologicalSex",
            maxWidth: 120,
        },
        {
            headerName: 'Date of Birth',
            field: "birthDate",
            cellDataType: 'date',
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
            headerName: 'Email Address',
            field: "email",
            filter: true,
        },
        {
            headerName: 'Status',
            field: "status",
            filter: true,
        },
        {
            headerName: 'Address',
            field: "address",
            filter: true,
        },
        {
            colId: "actions",
            headerName: "Actions",
            cellRenderer: ButtonEdit,
            maxWidth: 150,
        },
    ];

    function ButtonEdit(params) {
        const eButton = document.createElement('button');
        eButton.className = 'btn btn-sm';

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
            console.log(error);
        });
}


document.addEventListener('alpine:init', () => {

    Alpine.data('userForm', () => ({
        email: '',
        userName: '',
        firstName: '',
        lastName: '',
        selectSex: '',
        birthDate: '',
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
                birthDate = '',
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
                BirthDate: this.birthDate,
                Address: this.address,
            }

            this.loading = true;
            const result = await registerUser(newUser);

            if (result.success) {
                this.resetForm();
                loadUserData();

                Toastify({
                    text: result.message.general,
                    className: "text-white mx-5",
                    duration: 3000,
                    close: true,
                    gravity: "bottom",
                    position: "right",
                    stopOnFocus: true,
                    offset: { x: 30, y: 30 },
                    style: {
                        background: "#059669",
                        borderRadius: "0.5rem",
                        padding: "1rem",
                    },
                }).showToast();

            } else {
                this.loading = false;
                if (result.message.general) {
                    this.resetForm();
                    //show toast
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
        let err;

        if (error.response?.data)
            err = error.response.data

        return {
            success: false,
            message: err.message
        };
    }
}

//init() {
//    this.selectRole = new TomSelect(this.$refs.roleSelect, {})
//}