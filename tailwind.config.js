/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: "class",
  content: [
    "./Pages/**/*.cshtml",
    "./Views/**/*.cshtml",
    "./Views/Shared/**/*.cshtml",
  ],
  theme: {
    container: {
      center: true,
      padding: "2rem",
      screens: {
        "2xl": "1400px",
      },
    },
    extend: {
      colors: {
        iconColor:{
          light:"#00175c",
          dark:"#7e71f4",
        },
        tertiary:{
          light:"#da5e38dd",
          dark:"#f18767dd",
        }
      },
    },
    },

};
