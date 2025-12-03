
module.exports = {
    content: [
        "./Views/**/*.cshtml",
        "./Areas/**/Views/**/*.cshtml",
        "./wwwroot/js/**/*.js",
        "./wwwroot/Scripts/**/*.js" 
    ],
    theme: {
        extend: {
            fontFamily: {
                sans: ['Nunito Sans', 'ui-sans-serif', 'system-ui']
            },

        },
    },
    plugins: [
        require('daisyui')
    ],
    daisyui: {
        themes: ["light", "dark", {
            mytheme: {
                "primary": "#1C2C64",
                "secondary": "#FDF0D5",
                "accent": "#00ffff",
                "neutral": "#232325",
                "base-100": "#ffffff",
                "info": "#0000ff",
                "success": "#00ff00",
                "warning": "#00ff00",
                "error": "#FD5050",
            },
        },],
    },
}