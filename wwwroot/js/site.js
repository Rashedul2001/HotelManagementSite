document.addEventListener("DOMContentLoaded", () => {

  // // ======================
  // // Dark Mode Toggle Start
  // // ======================
  // const darkModeToggle = document.getElementById("darkModeToggle");
  // const darkModeIcon = document.getElementById("darkModeIcon");
  // const htmlElement = document.documentElement;
  // const sunIconPath = `<path d="M361.5 1.2c5 2.1 8.6 6.6 9.6 11.9L391 121l107.9 19.8c5.3 1 9.8 4.6 11.9 9.6s1.5 10.7-1.6 15.2L446.9 256l62.3 90.3c3.1 4.5 3.7 10.2 1.6 15.2s-6.6 8.6-11.9 9.6L391 391 371.1 498.9c-1 5.3-4.6 9.8-9.6 11.9s-10.7 1.5-15.2-1.6L256 446.9l-90.3 62.3c-4.5 3.1-10.2 3.7-15.2 1.6s-8.6-6.6-9.6-11.9L121 391 13.1 371.1c-5.3-1-9.8-4.6-11.9-9.6s-1.5-10.7 1.6-15.2L65.1 256 2.8 165.7c-3.1-4.5-3.7-10.2-1.6-15.2s6.6-8.6 11.9-9.6L121 121 140.9 13.1c1-5.3 4.6-9.8 9.6-11.9s10.7-1.5 15.2 1.6L256 65.1 346.3 2.8c4.5-3.1 10.2-3.7 15.2-1.6zM160 256a96 96 0 1 1 192 0 96 96 0 1 1 -192 0zm224 0a128 128 0 1 0 -256 0 128 128 0 1 0 256 0z"/>`;
  // const moonIconPath = `<path d="M223.5 32C100 32 0 132.3 0 256S100 480 223.5 480c60.6 0 115.5-24.2 155.8-63.4c5-4.9 6.3-12.5 3.1-18.7s-10.1-9.7-17-8.5c-9.8 1.7-19.8 2.6-30.1 2.6c-96.9 0-175.5-78.8-175.5-176c0-65.8 36-123.1 89.3-153.3c6.1-3.5 9.2-10.5 7.7-17.3s-7.3-11.9-14.3-12.5c-6.3-.5-12.6-.8-19-.8z"/>`;
  // const sunViewBox = "0 0 512 512";
  // const moonViewBox = "0 0 384 512";

  // if (localStorage.getItem("theme") === "dark") {
  //   htmlElement.classList.add("dark");
  //   if (darkModeIcon) {
  //     darkModeIcon.innerHTML = sunIconPath;
  //     darkModeIcon.setAttribute("viewBox", sunViewBox);
  //   }
  // }

  // if (darkModeToggle) {
  //   darkModeToggle.addEventListener("click", () => {
  //     const isDark = htmlElement.classList.toggle("dark");
  //     localStorage.setItem("theme", isDark ? "dark" : "light");
  //     if (darkModeIcon) {
  //       darkModeIcon.innerHTML = isDark ? sunIconPath : moonIconPath;
  //       darkModeIcon.setAttribute("viewBox", isDark ? sunViewBox : moonViewBox);
  //     }
  //   });
  // }

  // =====================
  // Modal Controllers
  // =====================


  window.showModal = function (modalId) {
    const modal = document.getElementById(modalId);
    const content = modal?.querySelector("div");
    if (!modal || !content) return;

    modal.classList.remove("hidden");
    modal.classList.add("flex");
    setTimeout(() => {
      content.classList.remove("-translate-y-10", "opacity-0", "scale-95");
      content.classList.add("translate-y-0", "opacity-100", "scale-100");
    }, 10);
  };

  window.hideModal = function (modalId) {
    const modal = document.getElementById(modalId);
    const content = modal?.querySelector("div");
    if (!modal || !content) return;

    content.classList.add("-translate-y-10", "opacity-0", "scale-95");
    content.classList.remove("translate-y-0", "opacity-100", "scale-100");
    setTimeout(() => {
      modal.classList.add("hidden");
      modal.classList.remove("flex");
    }, 300);
  };

  window.toggleModals = function (showId, hideId) {
    hideModal(hideId);
    setTimeout(() => showModal(showId), 300);
  };

  const modalTrigger = document.getElementById("modalTrigger")?.value;
  if (modalTrigger === "ShowLogInModal") {
    showModal("loginModal");
  } else if (modalTrigger === "ShowRegisterModal") {
    showModal("registerModal");
  }

  // =====================
  // Counter Animation
  // =====================
  //document.querySelectorAll(".count-up").forEach(function (el) {
  //  const span = el.querySelector("span");
  //  const target = parseInt(el.getAttribute("data-target"), 10);
  //  let count = 0;
  //  const duration = 2000;
  //  const stepTime = Math.max(Math.floor(duration / target), 30);

  //  function updateCount() {
  //    count++;
  //    span.textContent = count;
  //    if (count < target) {
  //      setTimeout(updateCount, stepTime);
  //    }
  //  }
  //  updateCount();
  //});

  // =====================
  // Dropdown Logic
  // =====================
  const dropdown = document.getElementById("dropdown");
  const dropdownBtn = document.getElementById("dropdownBtn");
  const dropdownList = document.getElementById("dropdownList");
  const selectedText = document.getElementById("selectedText");
  const arrow = dropdownBtn?.querySelector(".arrow");

  if (dropdown && dropdownBtn && dropdownList && selectedText) {
    dropdownBtn.addEventListener("click", function (e) {
      e.stopPropagation();
      dropdownList.classList.toggle("hidden");
      arrow?.classList.toggle("rotate-180");
    });

    dropdownList.querySelectorAll("li").forEach((item) => {
      item.addEventListener("click", function () {
        selectedText.textContent = this.textContent;
        dropdownList.classList.add("hidden");
        arrow?.classList.remove("rotate-180");
      });
    });

    document.addEventListener("click", function () {
      dropdownList.classList.add("hidden");
      arrow?.classList.remove("rotate-180");
    });
  }

  // =====================
  // Page Loader
  // =====================
    window.showLoader = function () {
        document.getElementById("localLoader").classList.remove("hidden");
    }

    window.hideLoader = function () {
        document.getElementById("localLoader").classList.add("hidden");
    }



  // // =====================
  // // Mobile Menu Toggle
  // // =====================
  // const mobileMenuBtn = document.getElementById("mobileMenuBtn");
  // const mobileMenu = document.getElementById("mobileMenu");
  // if (mobileMenuBtn && mobileMenu) {
  //   mobileMenuBtn.addEventListener("click", () => {
  //     const isHidden = mobileMenu.classList.contains("hidden");
  //     if (isHidden) {
  //       mobileMenu.classList.remove("hidden", "pointer-events-none");
  //     } else {
  //       mobileMenu.classList.add("hidden", "pointer-events-none");
  //     }
  //   });
  // }

});
