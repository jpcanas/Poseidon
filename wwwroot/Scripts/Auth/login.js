
document.addEventListener('alpine:init', () => {

    Alpine.data('loginForm', () => ({
        email: '',
        password: '',
        rememberMe: false,
        loading: false,
        emailError: '',
        passwordError: '',
        generalError: '',

        validateEmail() {
            this.emailError = '';
            if (!this.email.trim()) {
                this.emailError = 'Email is required';
                return false;
            }
            if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.email)) {
                this.emailError = 'Invalid email format';
                return false;
            }
            return true;
        },

        validatePassword() {
            this.passwordError = '';
            if (!this.password) {
                this.passwordError = 'Password is required';
                return false;
            }
 
            if (this.password.length < 8 || this.password.length > 100) {
                this.passwordError = 'Password must be 8-100 characters';
                return false;
            }
            return true;
        },

        validateAll() {
            const isEmailValid = this.validateEmail();
            const isPasswordValid = this.validatePassword();
            return isEmailValid && isPasswordValid;
        },

        async submitLogin() {
            this.generalError = '';

            if (!this.validateAll()) {
                return;
            }

            this.loading = true;

            var result = await login(this.email, this.password, this.rememberMe);

            if (result.success) {
                this.loading = false
                window.location.href = result.redirectUrl; 
            } else {
                
                if (result.errors.Email) {
                    this.emailError = result.errors.Email[0]
                }
                if (result.errors.Password) {
                    this.passwordError = result.errors.Password[0]
                }
                if (result.errors.general) {
                    this.generalError = result.errors.general[0]
                }

                this.loading = false;
            }
        },

    }));

    Alpine.data('forgotPasswordForm', () => ({
        emailInput: '',
        loading: false,
        isValid: true,
        errorMessage: '',
        successMsg: '',

        async submitResetPassword() {
            this.isValid = true,
            this.errorMessage = "";
            this.successMsg = "";
            //frontend validation
            if (!this.emailInput || !this.emailInput.includes('@')) {
                this.errorMessage = 'Please enter a valid email address.';
                this.isValid = false;
                return;
            }

            this.loading = true;
            var result = await resetEmail(this.emailInput)
            this.loading = false;

            if (result.status == 200) {
                this.isValid = true,
                    this.errorMessage = "";
                this.successMsg = result.msg

                forgotPassConfirm.showModal()

                document.getElementById('forgotPassMessage').innerHTML = this.successMsg;

                //Temporary show reset token link for testing purposes
                document.getElementById('resePasswordLink').href = result.resetLink;

            } else {
                this.isValid = false;
                this.errorMessage = result.msg
            }

        }
    }))
});


async function resetEmail(emailInput) {
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
            msg: res.data.msg,
            resetLink: res.data.tempResetLink //temporary reset link for testing purposes
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
            resetLink: null
        }
    }
}

async function login(email, password, rememberMe) {
    const antiForgToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
    try {
        const res = await axios.post('/Auth/Login',
            {
                Email: email,
                Password: password,
                RememberMe: rememberMe
            },
            {
                headers: {
                    'Content-Type': 'application/json',
                    "RequestVerificationToken": antiForgToken
                }
            }
        )

        return {
            success: true,
            redirectUrl: res.data.redirectUrl
        }

    } catch (error) {

        return {
            success: false,
            errors: error.response?.data?.errors || {}
        };
    }
}