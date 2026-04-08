
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
                "primary": "#2872A1",
                "secondary": "#F9EDB2",
                "accent": "#00ffff",
                "neutral": "#232325",
                "base-100": "#ffffff",
                "info": "#0000ff",
                "success": "#2DCD68",
                "warning": "#FDD339",
                "error": "#FD5050",
            },
        },],
    },
}