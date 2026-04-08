// Global functions and variales
let userPermissions = [];
const base = document.querySelector('meta[name="path-base"]')?.content ?? '';
axios.defaults.baseURL = base;
const pathBase = base ? `/${base}` : '';

function DatePickerOptions() {
    return {
        clearButton: true,
        todayButton: true,
        format: 'MM dd yyyy'
    }
}

function showToastify(type, message = "[no message to show]") {

    var toastColor = ""
    switch (type) {
        case 'success':
            toastColor = "#059669"
            break;
        case 'error':
            toastColor = "#EF5757"
            break;
        default:
            toastColor = "#059669"
    }

    Toastify({
        text: message,
        className: "text-white mx-5",
        duration: 3000,
        close: true,
        gravity: "bottom",
        position: "right",
        stopOnFocus: true,
        offset: { x: 30, y: 30 },
        style: {
            background: toastColor,
            borderRadius: "0.5rem",
            padding: "1rem",
        },
    }).showToast();

}

async function getUserPermissions() {
    userPermissions = [];
    try {
        const response = await axios.get('/Setting/GetMyPermissions');
        userPermissions = response.data.permissions;
    } catch (error) {
        console.error('Failed to load permissions:', error);
        userPermissions = [];
    }
}

function hasPermission(permissionCode) {
    return userPermissions.includes(permissionCode);
}

const userStatusColorMap = {
    green: { text: 'text-green-700', bg: 'bg-green-100' },
    red: { text: 'text-red-700', bg: 'bg-red-100' },
    yellow: { text: 'text-yellow-700', bg: 'bg-yellow-100' },
    gray: { text: 'text-gray-700', bg: 'bg-gray-100' },
}