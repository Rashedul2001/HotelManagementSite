document.addEventListener("DOMContentLoaded", () => {
  let isMobileOpen = false;

  // Icon HTML templates
  const HAMBURGER_ICON = `
    <svg class="h-4 w-4 text-gray-700" viewBox="0 0 14 14" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
      <path fill-rule="evenodd" clip-rule="evenodd" d="M1.5 3.5C0.67157 3.5 0 2.82843 0 2C0 1.17157 0.67157 0.5 1.5 0.5H12.5C13.3284 0.5 14 1.17157 14 2C14 2.82843 13.3284 3.5 12.5 3.5H1.5zM1.5 8.5C0.67157 8.5 0 7.8284 0 7C0 6.1716 0.67157 5.5 1.5 5.5H12.5C13.3284 5.5 14 6.1716 14 7C14 7.8284 13.3284 8.5 12.5 8.5H1.5zM1.5 13.5C0.67157 13.5 0 12.8284 0 12C0 11.1716 0.67157 10.5 1.5 10.5H12.5C13.3284 10.5 14 11.1716 14 12C14 12.8284 13.3284 13.5 12.5 13.5H1.5z"/>
    </svg>
  `;

  const CLOSE_ICON = `
    <svg class="h-6 w-6 text-gray-700" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
      <g id="SVGRepo_bgCarrier" stroke-width="0"></g>
      <g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g>
      <g id="SVGRepo_iconCarrier">
        <path opacity="0.5" d="M22 12C22 17.5228 17.5228 22 12 22C6.47715 22 2 17.5228 2 12C2 6.47715 6.47715 2 12 2C17.5228 2 22 6.47715 22 12Z" fill="currentColor"></path>
        <path d="M8.96967 8.96967C9.26256 8.67678 9.73744 8.67678 10.0303 8.96967L12 10.9394L13.9697 8.96969C14.2626 8.6768 14.7374 8.6768 15.0303 8.96969C15.3232 9.26258 15.3232 9.73746 15.0303 10.0304L13.0607 12L15.0303 13.9696C15.3232 14.2625 15.3232 14.7374 15.0303 15.0303C14.7374 15.3232 14.2625 15.3232 13.9696 15.0303L12 13.0607L10.0304 15.0303C9.73746 15.3232 9.26258 15.3232 8.96969 15.0303C8.6768 14.7374 8.6768 14.2626 8.96969 13.9697L10.9394 12L8.96967 10.0303C8.67678 9.73744 8.67678 9.26256 8.96967 8.96967Z" fill="currentColor"></path>
      </g>
    </svg>
  `;

  // Desktop sidebar hover effect
  const sidebar = document.querySelector(".sidebar");
  const mainContent = document.querySelector(".main-content");
  
  if (sidebar && mainContent) {
    sidebar.addEventListener("mouseenter", () => {
      mainContent.classList.add("expanded");
    });
    
    sidebar.addEventListener("mouseleave", () => {
      mainContent.classList.remove("expanded");
    });
  }

  // Mobile menu toggle
  const mobileMenuBtn = document.getElementById("mobile-menu-btn");
  const sidebarMobile = document.querySelector(".sidebar-mobile");
  const mobileOverlay = document.getElementById("mobile-overlay");
  
  if (mobileMenuBtn) {
    mobileMenuBtn.addEventListener("click", function () {
      isMobileOpen = !isMobileOpen;
      
      if (isMobileOpen) {
        if (sidebarMobile) sidebarMobile.classList.add("open");
        if (mobileOverlay) mobileOverlay.classList.remove("hidden");
        
        // Move button to right side of sidebar and change to close icon
        this.classList.remove("left-4");
        this.classList.add("left-[13rem]"); // Position it clearly to the right of the 12rem sidebar
        this.innerHTML = CLOSE_ICON;
      } else {
        if (sidebarMobile) sidebarMobile.classList.remove("open");
        if (mobileOverlay) mobileOverlay.classList.add("hidden");
        
        // Move button back to original position and change to hamburger icon
        this.classList.remove("left-[13rem]");
        this.classList.add("left-4");
        this.innerHTML = HAMBURGER_ICON;
      }
    });
  }

  // Close mobile menu when clicking overlay
  if (mobileOverlay) {
    mobileOverlay.addEventListener("click", function () {
      isMobileOpen = false;
      if (sidebarMobile) sidebarMobile.classList.remove("open");
      this.classList.add("hidden");
      
      // Reset button position and icon
      if (mobileMenuBtn) {
        mobileMenuBtn.classList.remove("left-[13rem]");
        mobileMenuBtn.classList.add("left-4");
        mobileMenuBtn.innerHTML = HAMBURGER_ICON;
      }
    });
  }

  // Navigation
  const navItems = document.querySelectorAll(".nav-item");
  navItems.forEach(item => {
    item.addEventListener("click", function (e) {
      // Only prevent default for buttons with data-target, not anchor tags
      if (this.tagName === 'BUTTON') {
        e.preventDefault();
        const target = this.getAttribute("data-target");

        // Update active state
        navItems.forEach(nav => nav.classList.remove("active"));
        this.classList.add("active");

        // Close mobile menu
        if (isMobileOpen) {
          if (sidebarMobile) sidebarMobile.classList.remove("open");
          if (mobileOverlay) mobileOverlay.classList.add("hidden");
          
          // Reset button position and icon
          if (mobileMenuBtn) {
            mobileMenuBtn.classList.remove("left-[13rem]");
            mobileMenuBtn.classList.add("left-4");
            mobileMenuBtn.innerHTML = HAMBURGER_ICON;
          }
          isMobileOpen = false;
        }

        // Navigate to page
        if (target) {
          window.location.href = target;
        }
      } else {
        // For anchor tags, just close mobile menu and let normal navigation happen
        if (isMobileOpen) {
          if (sidebarMobile) sidebarMobile.classList.remove("open");
          if (mobileOverlay) mobileOverlay.classList.add("hidden");
          
          // Reset button position and icon
          if (mobileMenuBtn) {
            mobileMenuBtn.classList.remove("left-[13rem]");
            mobileMenuBtn.classList.add("left-4");
            mobileMenuBtn.innerHTML = HAMBURGER_ICON;
          }
          isMobileOpen = false;
        }
      }
    });
  });

  // Modal functions
  window.openModal = (modalId) => {
    const modal = document.getElementById(modalId);
    if (!modal) return;
    
    modal.classList.remove("hidden");
    modal.classList.add("flex");
    
    const modalContent = modal.querySelector(".modal-content");
    if (modalContent) {
      modalContent.classList.add("animate-modal-slide-in");
    }
    
    document.body.classList.add("overflow-hidden");
  };

  window.closeModal = (modalId) => {
    const modal = document.getElementById(modalId);
    if (!modal) return;
    
    modal.classList.add("hidden");
    modal.classList.remove("flex");
    
    const modalContent = modal.querySelector(".modal-content");
    if (modalContent) {
      modalContent.classList.remove("animate-modal-slide-in");
    }
    
    document.body.classList.remove("overflow-hidden");

    // Clear form
    const form = modal.querySelector("form");
    if (form) {
      form.reset();
    }
    
    const errorMessage = modal.querySelector(".error-message");
    if (errorMessage) {
      errorMessage.classList.add("hidden");
    }
  };

  // Close modal when clicking backdrop
  const modalBackdrops = document.querySelectorAll(".modal-backdrop");
  modalBackdrops.forEach(backdrop => {
    backdrop.addEventListener("click", function (e) {
      if (e.target === this) {
        const modalId = this.getAttribute("id");
        window.closeModal(modalId);
      }
    });
  });

  // Form submission
  window.submitForm = (formId, endpoint, modalId) => {
    const form = document.getElementById(formId);
    if (!form) return;
    
    const formData = {};

    // Collect form data
    const inputs = form.querySelectorAll("input, select, textarea");
    inputs.forEach(field => {
      const name = field.getAttribute("name");
      const value = field.value;

      if (name && value) {
        formData[name] = value;
      }
    });

    // Show loading state
    const submitBtn = form.querySelector('button[type="submit"]');
    if (submitBtn) {
      const originalText = submitBtn.innerHTML;
      submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin mr-2"></i>Creating...';
      submitBtn.disabled = true;

      // Clear previous errors
      const errorMessages = form.querySelectorAll(".error-message");
      errorMessages.forEach(msg => msg.classList.add("hidden"));

      // Submit form
      fetch(endpoint, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(formData),
      })
      .then(response => response.json())
      .then(data => {
        if (data.success) {
          window.closeModal(modalId);
          window.showNotification(data.message, "success");
          // Reload page to show new data
          setTimeout(() => {
            window.location.reload();
          }, 1000);
        } else {
          showErrors(form, data.errors);
        }
      })
      .catch(() => {
        window.showNotification("An error occurred. Please try again.", "error");
      })
      .finally(() => {
        submitBtn.innerHTML = originalText;
        submitBtn.disabled = false;
      });
    }
  };

  // Show errors
  function showErrors(form, errors) {
    if (errors && errors.length > 0) {
      const errorContainer = form.querySelector(".error-message");
      if (errorContainer) {
        errorContainer.innerHTML = '<ul class="list-disc list-inside">' + 
          errors.map(error => "<li>" + error + "</li>").join("") + 
          '</ul>';
        errorContainer.classList.remove("hidden");
      }
    }
  }

  // Show notification
  window.showNotification = (message, type) => {
    const bgColor = type === "success" ? "bg-green-500" : "bg-red-500";
    const iconClass = type === "success" ? "check" : "exclamation-triangle";
    
    const notification = document.createElement("div");
    notification.className = `fixed top-4 right-4 ${bgColor} text-white px-6 py-3 rounded-lg shadow-lg z-50 animate-fade-in`;
    notification.innerHTML = `
      <div class="flex items-center">
        <i class="fas fa-${iconClass} mr-2"></i>
        ${message}
      </div>
    `;

    document.body.appendChild(notification);

    setTimeout(() => {
      notification.style.opacity = "0";
      setTimeout(() => {
        if (notification.parentNode) {
          notification.parentNode.removeChild(notification);
        }
      }, 300);
    }, 3000);
  };
});
