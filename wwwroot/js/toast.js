document.addEventListener("DOMContentLoaded", function () {
    const toast = document.getElementById("toast");
    const closeBtn = document.getElementById("toastClose");
    const toastContent = document.getElementById("toastContent");

    if (!toast || !toastContent) return;

    // Show the toast (slide in from right)
    toast.classList.remove("hidden");
    setTimeout(() => {
        toastContent.classList.remove("translate-x-20", "opacity-0");
        toastContent.classList.add("translate-x-0", "opacity-100");
    }, 10);

    // Auto hide after 4 seconds (slide out to right)
    setTimeout(() => {
        toastContent.classList.remove("translate-x-0", "opacity-100");
        toastContent.classList.add("translate-x-20", "opacity-0");
        setTimeout(() => {
            toast.classList.add("hidden");
        }, 500);
    }, 3000);

    // Manual close (slide out to right)
    closeBtn.addEventListener("click", () => {
        toastContent.classList.remove("translate-x-0", "opacity-100");
        toastContent.classList.add("translate-x-20", "opacity-0");
        setTimeout(() => {
            toast.classList.add("hidden");
        }, 500);
    });
});