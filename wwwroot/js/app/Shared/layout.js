
document.addEventListener('DOMContentLoaded', () => {
    const expWarningAlert = document.getElementById('alertWarningSessionExp');
    const expWarningTxt = document.getElementById('txtWarningSessionExp');
    const logoutBtn = document.getElementById('btnLogout');
    let inactivityTime = 0;
    let maxInactiveTime = parseInt(document.getElementById('maxInactiveTime').value);
    let forcedLogoutTime = parseInt(document.getElementById('forcedLogoutTime').value);

    document.getElementById('reloginBtn').addEventListener('click', () => {
        window.location.href = `${axios.defaults.baseURL}/Auth/Login`;
    });

    logoutBtn.addEventListener('click', async () => logout());

    const inactivityInterval = setInterval(() => {
        inactivityTime++;
        if (inactivityTime >= maxInactiveTime) {
            expWarningAlert.classList.remove('hidden');
            expWarningTxt.textContent = `Warning: You’ve been inactive for a while. For your security, you’ll be logged out in ${forcedLogoutTime}s if no activity is detected`
            forcedLogoutTime--;
            if (forcedLogoutTime == 0) {
                inactivityTime = 0;
                expWarningTxt.textContent = "";
                autoLogout();
                clearInterval(inactivityInterval);
            }
        }
    }, 1000);

    ["mousemove", "keydown", "click", "scroll", "touchstart"].forEach(event => {
        document.addEventListener(event, () => {
            forcedLogoutTime = 30;
            inactivityTime = 0;
            expWarningAlert.classList.add('hidden');
        }, false)
    })

    //const elem = document.querySelector('#vanillaPckr');
    //const datepicker = new Datepicker(elem, {
    //    clearButton: true,
    //    format: 'MM dd yyyy'
    //}); 

    //datepicker.setDate('03-23-2026')

});


async function autoLogout() {
    const antiForgToken = document.querySelector('input[name="__RequestVerificationToken"]').value;

    try {
        const res = await axios.post('/Auth/Logout', null,
            {
                headers: {
                    'Content-Type': 'application/json',
                    "RequestVerificationToken": antiForgToken
                }
            }
        )

        if (res.status == 200) {
            modalSessionExpired.showModal();
        } else {
            alert("Logout failed. Please try again.");
        }
    } catch (err) {
        console.error("err", err)
    } 

}


async function logout() {
    const antiForgToken = document.querySelector('input[name="__RequestVerificationToken"]').value;

    try {
        const res = await axios.post('/Auth/Logout', null,
            {
                headers: {
                    'Content-Type': 'application/json',
                    "RequestVerificationToken": antiForgToken
                }
            }
        )

        if (res.status == 200) {
            window.location.href = `${axios.defaults.baseURL}/Auth/Login`;
        } else {
            alert("Logout failed. Please try again.");
        }
    } catch (err) {
        console.error("err", err)
    } 
}
