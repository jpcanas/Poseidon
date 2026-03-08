
document.addEventListener('alpine:init', function () {

    Alpine.data('profileData', () => ({
        emailDisplay: '',
        usernameDisplay: '',
        rolenameDisplay: '',
        firstnameDisplay: '',
        lastnameDisplay: '',
        middlenameDisplay: '',
        sexDisplay: '',
        birthdateDisplay: '',
        mobilenumDisplay: '',
        addressDisplay: '',
        statusDisplay: '',
        useridentifier: '',

        username: '',
        firstname: '',
        lastname: '',
        middlename: '',
        birthdate: '' || null,
        mobile: '',
        address: '',
        currentPassword: '',
        newPassword: '',
        confirmPassword: '',
        userNameError: '',
        firstNameError: '',
        lastNameError: '',
        selectSexError: '',
        currentPasswordError: '',
        newPasswordError: '',
        notMatchError: '',
        loading: false,
        passwordLoading: false,
        forgotpasswordLoading: false,
        redirectCountdown: 10,
        countdownInterval: null,

        async init() {
            const dateBirthElem = document.querySelector('#dateBirth');
            const dateBirth = new Datepicker(dateBirthElem, DatePickerOptions());
            this.birthdate = dateBirth;

            const result = await loadCurrentUserData();
            if (result.success) {
                this.initializedForm(result)
                dateBirth.setDate(result.user.birthDateInput)
            }
        },

        validateUserInput(input) {

            switch (input) {

                case "userName":
                    this.userNameError = '';
                    if (!this.username.trim()) {
                        this.userNameError = 'Username is required';
                        return false;
                    };
                    return true;

                case "firstName":
                    this.firstNameError = '';
                    if (!this.firstname.trim()) {
                        this.firstNameError = 'First Name is required';
                        return false;
                    };
                    return true;

                case "lastName":
                    this.lastNameError = '';
                    if (!this.lastname.trim()) {
                        this.lastNameError = 'Last Name is required';
                        return false;
                    };
                    return true;

                case "sex":
                    this.selectSexError = '';
                    if (!document.querySelector('#selectSex').value) {
                        this.selectSexError = 'Biological Sex is required';
                        return false;
                    };
                    return true;

                case "currentPassword":
                    this.currentPasswordError = '';
                    if (!this.currentPassword.trim()) {
                        this.currentPasswordError = 'Current password is required';
                        return false;
                    };
                    return true;

                case "newPassword":
                    this.newPasswordError = '';
                    if (!this.newPassword.trim()) {
                        this.newPasswordError = 'New password is required';
                        return false;
                    };
                    return true;

                default:
                    return true;
            }

        },

        validateAll() {
            const inputFields = ['firstName', 'lastName', 'sex', 'userName']
            let invalidCount = 0;
            for (const input of inputFields) {
                let isValid = this.validateUserInput(input);
                if (!isValid) {
                    invalidCount++;
                }
            }
            return invalidCount == 0 ? true : false;
        },

        initializedForm(result) {
            this.statusDisplay = result.user.statusName;
            this.emailDisplay = result.user.email;
            this.rolenameDisplay = result.user.roleName;
            this.usernameDisplay = result.user.userName;
            this.firstnameDisplay = result.user.firstName;
            this.lastnameDisplay = result.user.lastName;
            this.middlenameDisplay = result.user.middleName;
            this.sexDisplay = result.user.biologicalSexStr;
            this.birthdateDisplay = result.user.birthDateString;
            this.mobilenumDisplay = result.user.mobileNumber;
            this.addressDisplay = result.user.address;
            this.userid = result.user.userId;

            this.username = result.user.userName;
            this.firstname = result.user.firstName;
            this.lastname = result.user.lastName;
            this.middlename = result.user.middleName;
            document.querySelector('#selectSex').value = result.user.biologicalSex;
            this.mobile = result.user.mobileNumber;
            this.address = result.user.address;
        },

        // Validation Rules
        get hasMinLength() {
            return this.newPassword.length >= 8;
        },

        get hasNumber() {
            return /\d/.test(this.newPassword);
        },

        get hasSpecialChar() {
            return /[!@#$%^&*(),.?":{}|<>]/.test(this.newPassword);
        },

        get hasUpperAndLower() {
            return /[a-z]/.test(this.newPassword) && /[A-Z]/.test(this.newPassword);
        },

        get isValid() {
            return this.hasMinLength &&
                this.hasNumber &&
                this.hasSpecialChar &&
                this.hasUpperAndLower;
        },

        get passwordsMatch() {
            this.notMatchError = '';
            if (this.newPassword === this.confirmPassword && this.confirmPassword.length > 0) {
                return true;
            } else {
                this.notMatchError = "Passwords do not match";
                return false;
            }
        },

        startCountdown() {
            this.redirectCountdown = 10;
            this.countdownInterval = setInterval(() => {
                this.redirectCountdown--;
                if (this.redirectCountdown <= 0) {
                    this.redirectToLogin();
                }
            }, 1000);
        },

        redirectToLogin() {
            clearInterval(this.countdownInterval);
            logout(); //from layout.js
        },

        async submitUpdateUser() {

            if (!this.validateAll()) {
                return
            }
            this.loading = true;
            var userUpdate = {
                UserId: this.userid,
                UserName: this.username,
                FirstName: this.firstname,
                LastName: this.lastname,
                MiddleName: this.middlename,
                BiologicalSex: parseInt(document.querySelector('#selectSex').value),
                MobileNumber: this.mobile,
                Address: this.address,
                BirthDate: this.birthdate.getDate("yyyy-mm-dd"),
            };

            var res = await updateUserProfile(userUpdate)
            if (res.success && res.user != null) {
                this.initializedForm(res);

                showToastify('success', res.message.general)

                document.getElementById('profileEditModal').close();

            } else {

                if (res.message.general) {
                    showToastify('error', res.message.general)
                }
                if (res.message.username) {
                    this.userNameError = res.message.username;
                }
            }

            this.loading = false;
        },

        async handlePasswordSubmit() {
            this.passwordLoading = true;

            if (!this.validateUserInput("currentPassword") && !this.validateUserInput("newPassword")) {
                this.passwordLoading = false;
                return
            }

            if (!this.isValid) {
                this.passwordLoading = false;
                this.newPasswordError = 'Password requirements not met'
                return;
            }

            if (!this.passwordsMatch) {
                this.passwordLoading = false;
                this.notMatchError = 'Password do not match'
                return;
            }

            var passwordData = {
                UserId: this.userid,
                CurrentPassword: this.currentPassword,
                NewPassword: this.newPassword,
                ConfirmPassword: this.confirmPassword,
            }

            var result = await UpdateUserPassword(passwordData);
            if (result.success) {
                this.passwordLoading = false;
                const passwordModal = document.querySelector('#modalRedirectLogin');
                document.querySelector('#txtHeaderPasswordChanged').textContent = "Password Changed Successfully!";
                document.querySelector('#txtDetailsPasswordChanged').textContent = "Your password has been changed successfully. For security reasons, please sign in again using your new password.You will be redirected to the login page to continue.";
                passwordModal.showModal();
                this.startCountdown();
            } else {
                if (result.errors.General) {
                    showToastify('error', result.errors.General[0])
                }
                if (result.errors.currentPassword) {
                    this.currentPasswordError = result.errors.currentPassword[0]
                }
                if (result.errors.newPassword) {
                    this.newPasswordError = result.errors.newPassword[0]
                }
                if (result.errors.confirmPassword) {
                    this.notMatchError = result.errors.confirmPassword[0]
                }

            }

            this.passwordLoading = false;
        },

        async forgotMyPassword() {
            this.forgotpasswordLoading = true;
            var result = await sendResetPasswordEmail(this.emailDisplay)
            if (result.status == 200) {
                this.forgotpasswordLoading = false;
                const passwordModal = document.querySelector('#modalRedirectLogin');
                document.querySelector('#txtHeaderPasswordChanged').textContent = "Password reset instruction sent";
                document.querySelector('#txtDetailsPasswordChanged').textContent = result.msg;
                passwordModal.showModal();
                this.startCountdown();
            } else {
                showToastify('error', result.msg)
            }
        },


    }));
});

async function loadCurrentUserData() {
    try {
        const res = await axios.get('/Setting/GetCurrentUser')

        return {
            success: true,
            user: res.data
        }

    } catch (error) {
        let err;

        if (error.response?.data)
            err = error.response.data

        return {
            success: false,
            user: null
        };
    }

}

async function updateUserProfile(userUpdate) {
    const antiForgToken = document.querySelector('input[name="__RequestVerificationToken"]').value;

    try {
        const res = await axios.post('/Setting/UpdateUser', userUpdate,
            {
                headers: {
                    'Content-Type': 'application/json',
                    "RequestVerificationToken": antiForgToken
                }
            }
        )
        return {
            success: true,
            user: res.data.user,
            message: res.data.message
        }
    } catch (error) {
        let errMsg = 'An error occurred while updating the profile. Please try again later.';
        if (error.response?.data?.message)
            errMsg = error.response.data.message;
        return {
            success: false,
            user: null,
            message: errMsg
        };
    }
}

async function UpdateUserPassword(passwordData) {
    const antiForgToken = document.querySelector('input[name="__RequestVerificationToken"]').value;

    try {
        const res = await axios.post('/Setting/UpdateUserPassword', passwordData,
            {
                headers: {
                    'Content-Type': 'application/json',
                    "RequestVerificationToken": antiForgToken
                }
            }
        )
        return {
            success: res.data.success,
            errors: ""
        }
    } catch (error) {
        return {
            success: false,
            errors: error.response?.data?.errors || {}
        };
    }
}

async function sendResetPasswordEmail(emailInput) {
    const antiForgToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
    try {
        const res = await axios.post('/Auth/ResetPassword', { Email: emailInput },
            {
                headers: {
                    'Content-Type': 'application/json',
                    "RequestVerificationToken": antiForgToken
                }
            }
        )

        return {
            status: res.status,
            msg: "Password reset instructions have been sent to your email. For security purposes, you will be logged out of the system. Please check your email to reset your password.",
        }

    } catch (error) {
        let message = "Something went wrong. Please contact admin"
        if (error.response?.data) {
            const { Email } = error.response.data;
            message = Email[0]
        }

        return {
            status: error.status,
            msg: message,
        }
    }
}