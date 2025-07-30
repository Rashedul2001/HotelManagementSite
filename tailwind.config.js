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
				iconColor: {
					light: "#00175c",
					dark: "#7e71f4",
				},
				tertiary: {
					light: "#da5e38dd",
					dark: "#f18767dd",
				}
			},
			animation: {
				'slide-in': 'slideIn 0.3s ease-out',
				'fade-in': 'fadeIn 0.3s ease-out',
				'pulse-glow': 'pulseGlow 2s infinite',
				'float': 'float 3s ease-in-out infinite',
			},
			keyframes: {
				slideIn: {
					'0%': { transform: 'translateY(-100px)', opacity: '0' },
					'100%': { transform: 'translateY(0)', opacity: '1' }
				},
				fadeIn: {
					'0%': { opacity: '0' },
					'100%': { opacity: '1' }
				},
				pulseGlow: {
					'0%, 100%': { boxShadow: '0 0 20px rgba(59, 130, 246, 0.5)' },
					'50%': { boxShadow: '0 0 30px rgba(59, 130, 246, 0.8)' }
				},
				float: {
					'0%, 100%': { transform: 'translateY(0px)' },
					'50%': { transform: 'translateY(-10px)' }
				}
			}




		},

	},

};
