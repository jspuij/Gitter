const { src, dest, parallel } = require('gulp'),
    sourcemaps = require("gulp-sourcemaps"),
    sass = require("gulp-sass"),
    rename = require("gulp-rename"),
    cleanCSS = require('gulp-clean-css');

function css() {
    return src('content/css/blazor.gitter.scss')
        .pipe(sourcemaps.init())
        .pipe(sass())
        .pipe(cleanCSS())
        .pipe(rename("blazored.gitter.min.css"))
        .pipe(sourcemaps.write('.'))
        .pipe(dest('../Blazor.Gitter.Client/wwwroot/css'))
        .pipe(dest('../Blazor.Gitter.Server/wwwroot/css'))
        .pipe(dest('../Blazor.Gitter.AndroidApp/wwwroot/css'))
        .pipe(dest('../Blazor.Gitter.IosApp/Resources/wwwroot/css'))
        .pipe(dest('../Blazor.Gitter.WindowsApp/wwwroot/css'));
}

function javascript() {
    return src('content/scripts/chat.js')
        .pipe(dest('../Blazor.Gitter.Client/wwwroot/scripts'))
        .pipe(dest('../Blazor.Gitter.Server/wwwroot/scripts'))
        .pipe(dest('../Blazor.Gitter.AndroidApp/wwwroot/scripts'))
        .pipe(dest('../Blazor.Gitter.IosApp/Resources/wwwroot/scripts'))
        .pipe(dest('../Blazor.Gitter.WindowsApp/wwwroot/scripts'));
}

exports.sass = parallel(css, javascript);