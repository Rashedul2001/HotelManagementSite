class FuturisticHeader {
  constructor() {
    this.header = document.querySelector("header");
    this.mobileMenuBtn = document.getElementById("mobileMenuBtn");
    this.mobileMenu = document.getElementById("mobileMenu");
    this.darkModeToggle = document.getElementById("darkModeToggle");
    this.hamburger = document.querySelector(".hamburger");
    this.lastScrollY = window.scrollY;

    this.init();
  }

  init() {
    this.setupMobileMenu();
    this.setupScrollEffects();
    this.setupThemeToggle();
    this.setupSearchFunctionality();
    this.setupNotifications();
  }

  setupMobileMenu() {
    this.mobileMenuBtn.addEventListener("click", () => {
      const isOpen = this.mobileMenu.classList.contains("open");

      if (isOpen) {
        this.mobileMenu.classList.remove("open");
        this.hamburger.classList.remove("active");
        document.body.style.overflow = "auto";
      } else {
        this.mobileMenu.classList.add("open");
        this.hamburger.classList.add("active");
        document.body.style.overflow = "hidden";
      }
    });

    // Close mobile menu when clicking outside
    document.addEventListener("click", (e) => {
      if (
        !this.mobileMenu.contains(e.target) &&
        !this.mobileMenuBtn.contains(e.target)
      ) {
        this.mobileMenu.classList.remove("open");
        this.hamburger.classList.remove("active");
        document.body.style.overflow = "auto";
      }
    });

    // Close mobile menu on window resize
    window.addEventListener("resize", () => {
      if (window.innerWidth >= 1024) {
        this.mobileMenu.classList.remove("open");
        this.hamburger.classList.remove("active");
        document.body.style.overflow = "auto";
      }
    });
  }

  setupScrollEffects() {
    let ticking = false;

    const updateHeader = () => {
      const currentScrollY = window.scrollY;

      // Header background opacity based on scroll
      const opacity = Math.min(currentScrollY / 100, 1);
      this.header.style.setProperty("--bg-opacity", opacity);

      // Hide/show header on scroll
      if (currentScrollY > 100) {
        if (currentScrollY > this.lastScrollY) {
          // Scrolling down
          this.header.style.transform = "translateY(-100%)";
        } else {
          // Scrolling up
          this.header.style.transform = "translateY(0)";
        }
      } else {
        this.header.style.transform = "translateY(0)";
      }

      this.lastScrollY = currentScrollY;
      ticking = false;
    };

    window.addEventListener("scroll", () => {
      if (!ticking) {
        requestAnimationFrame(updateHeader);
        ticking = true;
      }
    });
  }

  setupThemeToggle() {
    this.darkModeToggle.addEventListener("click", () => {
      const isDark = document.documentElement.classList.contains("dark");

      if (isDark) {
        document.documentElement.classList.remove("dark");
        localStorage.setItem("theme", "light");
      } else {
        document.documentElement.classList.add("dark");
        localStorage.setItem("theme", "dark");
      }

      // Add click animation
      this.darkModeToggle.style.transform = "scale(0.95)";
      setTimeout(() => {
        this.darkModeToggle.style.transform = "scale(1)";
      }, 150);
    });
  }

  setupSearchFunctionality() {
    const searchButtons = document.querySelectorAll('button[class*="search"]');

    searchButtons.forEach((btn) => {
      btn.addEventListener("click", () => {
        // Add ripple effect
        const ripple = document.createElement("div");
        ripple.className =
          "absolute inset-0 bg-blue-500 bg-opacity-20 rounded-full scale-0 animate-ping";
        btn.appendChild(ripple);

        setTimeout(() => {
          ripple.remove();
        }, 600);

        // Here you can add search modal or redirect logic
        console.log("Search clicked");
      });
    });
  }

  setupNotifications() {
    const notificationBtn = document.querySelector(
      'button[class*="notification"]'
    );

    if (notificationBtn) {
      notificationBtn.addEventListener("click", () => {
        // Animate notification badge
        const badge = notificationBtn.querySelector(".notification-badge");
        if (badge) {
          badge.style.animation = "none";
          setTimeout(() => {
            badge.style.animation = "pulse-notification 2s infinite";
          }, 100);
        }

        // Here you can add notification dropdown logic
        console.log("Notifications clicked");
      });
    }
  }
}

// Initialize header when DOM is loaded
document.addEventListener("DOMContentLoaded", () => {
  new FuturisticHeader();
});

// Add smooth scroll behavior for navigation links
document.querySelectorAll('a[href^="#"]').forEach((anchor) => {
  anchor.addEventListener("click", function (e) {
    e.preventDefault();
    const target = document.querySelector(this.getAttribute("href"));
    if (target) {
      target.scrollIntoView({
        behavior: "smooth",
        block: "start",
      });
    }
  });
});
