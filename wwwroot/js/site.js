document.addEventListener("DOMContentLoaded", () => {
  const darkModeToggle = document.getElementById("darkModeToggle");
  const darkModeIcon = document.getElementById("darkModeIcon");
  const htmlElement = document.documentElement;

  if (localStorage.getItem("theme") === "dark") {
    htmlElement.classList.add("dark");
    darkModeIcon.classList.remove("fa-moon");
    darkModeIcon.classList.add("fa-sun");
  }
  darkModeToggle.addEventListener("click", () => {
    if (htmlElement.classList.contains("dark")) {
      htmlElement.classList.remove("dark");
      darkModeIcon.classList.remove("fa-sun");
      darkModeIcon.classList.add("fa-moon");
      localStorage.setItem("theme", "light");
    } else {
      htmlElement.classList.add("dark");
      darkModeIcon.classList.remove("fa-moon");
      darkModeIcon.classList.add("fa-sun");
      localStorage.setItem("theme", "dark");
    }
  });
});
