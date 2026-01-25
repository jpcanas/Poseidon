document.addEventListener('alpine:init', () => {
    Alpine.data('passwordComponent', () => ({
        password: '',
        confirmPassword: '',
        submitError: '',
        notMatchError: '',
        loading: false,
        redirectCountdown: 10,
        countdownInterval: null,

        // Validation Rules
        get hasMinLength() {
            return this.password.length >= 8;
        },

        get hasNumber() {
            return /\d/.test(this.password);
        },

        get hasSpecialChar() {
            return /[!@#$%^&*(),.?":{}|<>]/.test(this.password);
        },

        get hasUpperAndLower() {
            return /[a-z]/.test(this.password) && /[A-Z]/.test(this.password);
        },

        get isValid() {
            return this.hasMinLength &&
                this.hasNumber &&
                this.hasSpecialChar &&
                this.hasUpperAndLower;
        },

        get passwordsMatch() {
            this.notMatchError = '';
            if (this.password === this.confirmPassword && this.confirmPassword.length > 0) {
                return true;
            } else {
                this.notMatchError = "Passwords do not match";
                return false;
            }
            
        },

        // Methods
        async handleSubmit() {
            this.submitError = '';
            this.loading = true;
            if (!this.isValid) {
                this.loading = false;
                this.submitError = 'Password requirements not met'
                return;
            }

            if (!this.passwordsMatch) {
                this.loading = false;
                this.submitError = 'Password do not match'
                return;
            }

            var passwordData = {
                UserGuid: document.querySelector('#userGuid').value,
                ResetToken: document.querySelector('#resetToken').value,
                NewPassword: this.password,
                ConfirmPassword: this.confirmPassword,
            }

            var result = await updatePassword(passwordData);

            if (result.status == 200) {
                setTimeout(() => {
                    this.loading = false;
                    const passwordModal = document.querySelector('#passwordChangeModal');
                    passwordModal.showModal();
                    this.startCountdown();
                }, 2000)
            } else {
                this.loading = false;
                this.submitError = result.msg
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
            window.location.href = '/Auth/Login';
        },

        resetForm() {
            this.password = '';
            this.confirmPassword = '';
            if (this.countdownInterval) {
                clearInterval(this.countdownInterval);
            }
        }

    }));
});

async function updatePassword(passwordData) {
    const antiForgToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
    try {
        const res = await axios.post('/Auth/UpdatePassword', passwordData,
            {
                headers: {
                    'Content-Type': 'application/json',
                    "RequestVerificationToken": antiForgToken
                }
            }
        )

        return {
            status: res.status,
            msg: res.data,
        }

    } catch (error) {
        let message = "Something went wrong. Please contact admin"
        if (error.response?.data) {
            message = error.response.data;
        }

        return {
            status: error.status,
            msg: message,
        }
    }
}
