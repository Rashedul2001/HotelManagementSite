class FuturisticHeader {
  constructor() {
    this.header = document.querySelector("header");
    this.mobileMenuBtn = document.getElementById("mobileMenuBtn");
    this.mobileMenu = document.getElementById("mobileMenu");
    this.mobileMenuBackdrop = document.getElementById("mobileMenuBackdrop");
    this.darkModeToggle = document.getElementById("darkModeToggle");
    this.hamburger = document.querySelector(".hamburger");
    this.lastScrollY = window.scrollY;
    this.isMenuOpen = false;

    this.init();
  }

  init() {
    this.setupMobileMenu();
    this.setupScrollEffects();
    this.setupThemeToggle();
    this.setupSearchFunctionality();
    this.setupNotifications();
    this.loadTheme();
  }

  setupMobileMenu() {
    // Toggle button
    this.mobileMenuBtn.addEventListener("click", (e) => {
      e.stopPropagation();
      this.toggleMobileMenu();
    });

    // Close when clicking backdrop
    this.mobileMenuBackdrop.addEventListener("click", () => {
      this.closeMobileMenu();
    });

    // Close when clicking outside
    document.addEventListener("click", (e) => {
      if (
        this.isMenuOpen &&
        !this.mobileMenu.contains(e.target) &&
        !this.mobileMenuBtn.contains(e.target)
      ) {
        this.closeMobileMenu();
      }
    });

    // Close on resize (desktop breakpoint)
    window.addEventListener("resize", () => {
      if (window.innerWidth >= 1024 && this.isMenuOpen) {
        this.closeMobileMenu();
      }
    });

    // Close on nav link click
    this.mobileMenu.querySelectorAll("a").forEach((link) => {
      link.addEventListener("click", () => {
        this.closeMobileMenu();
      });
    });
  }
  toggleMobileMenu() {
    if (this.isMenuOpen) {
      this.closeMobileMenu();
    } else {
      this.openMobileMenu();
    }
  }

  openMobileMenu() {
    this.isMenuOpen = true;
    this.mobileMenu.classList.remove("hidden", "pointer-events-none"); // make visible
    this.mobileMenu.classList.add("open");
    this.mobileMenuBackdrop.classList.add("active");
    this.hamburger.classList.add("active");
    document.body.style.overflow = "hidden";
  }

  closeMobileMenu() {
    this.isMenuOpen = false;
    this.mobileMenu.classList.add("hidden", "pointer-events-none"); // hide again
    this.mobileMenu.classList.remove("open");
    this.mobileMenuBackdrop.classList.remove("active");
    this.hamburger.classList.remove("active");
    document.body.style.overflow = "auto";
  }

  setupScrollEffects() {
    let ticking = false;

    const updateHeader = () => {
      const currentScrollY = window.scrollY;

      // Header background opacity based on scroll
      const opacity = Math.min(currentScrollY / 100, 0.95);
      this.header.style.setProperty("--bg-opacity", opacity);

      // Hide/show header on scroll (only if menu is not open)
      if (!this.isMenuOpen && currentScrollY > 100) {
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
    this.darkModeToggle.addEventListener("click", (e) => {
      e.stopPropagation();
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

  loadTheme() {
    // Load theme from localStorage
    const savedTheme = localStorage.getItem("theme");
    if (
      savedTheme === "dark" ||
      (!savedTheme && window.matchMedia("(prefers-color-scheme: dark)").matches)
    ) {
      document.documentElement.classList.add("dark");
    }
  }

  setupSearchFunctionality() {
    const searchButtons = document.querySelectorAll(
      'button[class*="glass-morphism"]'
    );

    searchButtons.forEach((btn) => {
      if (btn.querySelector('svg path[d*="M21 21l-6-6"]')) {
        btn.addEventListener("click", (e) => {
          e.stopPropagation();
          // Add ripple effect
          const ripple = document.createElement("div");
          ripple.className =
            "absolute inset-0 bg-blue-500 bg-opacity-20 rounded-full scale-0 animate-ping";
          btn.style.position = "relative";
          btn.appendChild(ripple);

          setTimeout(() => {
            ripple.remove();
          }, 600);

          // Here you can add search modal or redirect logic
          console.log("Search clicked");
        });
      }
    });
  }

  setupNotifications() {
    const notificationBtn = document.querySelector(
      "button .notification-badge"
    );

    if (notificationBtn && notificationBtn.parentElement) {
      notificationBtn.parentElement.addEventListener("click", (e) => {
        e.stopPropagation();
        // Animate notification badge
        notificationBtn.style.animation = "none";
        setTimeout(() => {
          notificationBtn.style.animation = "pulse-notification 2s infinite";
        }, 100);

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
