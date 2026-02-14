
document.addEventListener('DOMContentLoaded', () => {
    const expWarningAlert = document.getElementById('alertWarningSessionExp');
    const expWarningTxt = document.getElementById('txtWarningSessionExp');
    const logoutBtn = document.getElementById('btnLogout');
    let inactivityTime = 0;
    let maxInactiveTime = parseInt(document.getElementById('maxInactiveTime').value);
    let forcedLogoutTime = parseInt(document.getElementById('forcedLogoutTime').value);

    document.getElementById('reloginBtn').addEventListener('click', () => {
        window.location.href = '/Auth/Login';
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


    //document.getElementById("btnTest").addEventListener("click", function () {
    //    axios.get('/ping')
    //        .then(function (res) {
    //            console.log(res.data);
    //            document.getElementById("result").innerText = res.data.message;
    //        })
    //        .catch(function (err) {
    //            console.error(err);
    //            document.getElementById("result").innerText = "Error!";
    //        });
    //});
});


async function autoLogout() {
    const res = await fetch("/Auth/AutoLogout", {
        method: "POST",
    })

    if (res.ok) {
        modalSessionExpired.showModal();
    } else {
        alert("Logout failed. Please try again.");
    } 
}


async function logout() {
    const antiForgToken = document.querySelector('input[name="__RequestVerificationToken"]').value;

    try {
        const res = await fetch("/Auth/Logout", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": antiForgToken
            },
        })

        if (res.ok) {
            window.location.href = "/Auth/Login";
        } else {
            alert("Logout failed. Please try again.");
        }
    } catch (err) {
        console.error(err)
    } 
}
