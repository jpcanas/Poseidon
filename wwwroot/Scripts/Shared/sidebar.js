const sidebar = document.getElementById('sidebar');
const toggleBtn = document.getElementById('toggleSidebar');
const overlay = document.getElementById('overlay');

function toggleSidebarFn() {
    sidebar.classList.toggle('-translate-x-full');
    overlay.classList.toggle('hidden');
    document.body.classList.toggle('overflow-hidden');
}

toggleBtn.addEventListener('click', toggleSidebarFn);
overlay.addEventListener('click', toggleSidebarFn);

const sidebarLinks = sidebar?.querySelectorAll('a');
sidebarLinks?.forEach(link => {
    link.addEventListener('click', function () {
        // Only auto-close on mobile devices
        if (window.innerWidth < 768 && !sidebar.classList.contains('-translate-x-full')) {
            toggleSidebarFn();
        }
    });
});

// Handle window resize - close sidebar and overlay when resizing to desktop
//let resizeTimer;
//window.addEventListener('resize', function () {
//    clearTimeout(resizeTimer);
//    resizeTimer = setTimeout(function () {
//        if (window.innerWidth >= 768) {
//            sidebar.classList.add('-translate-x-full');
//            sidebar.classList.remove('-translate-x-full'); // Reset to default desktop state
//            overlay.classList.add('hidden');
//            document.body.classList.remove('overflow-hidden');
//        }
//    }, 250);
//});