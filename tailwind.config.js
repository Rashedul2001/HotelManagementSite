/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: "class",
  content: [
    "./Pages/**/*.cshtml",
    "./Views/**/*.cshtml",
    "./Views/Shared/**/*.cshtml",
  ],
  theme: {
    extend: {
      colors: {
        primary: "#038C7F",
        secondary: "#F2C641",
        tertiary: {
          dark: "#F27405",
          light: "#F2C641",
        },
      },
      fontFamily:{
      }
    },
  },
  plugins: [],
};
