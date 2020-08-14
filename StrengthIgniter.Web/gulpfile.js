const gulp = require('gulp');

const js = () => {
    return gulp.src([
        './Assets/Js/site.js'
    ]).pipe(gulp.dest('./wwwroot/js/'));
};
const css = () => {
    return gulp.src([
        './Assets/Css/site.css'
    ]).pipe(gulp.dest('./wwwroot/css/'));
};
const icon = () => {
    return gulp.src([
        './Assets/favicon.ico'
    ]).pipe(gulp.dest('./wwwroot/'));
};

const thirdPartyJs = () => {
    return gulp
        .src([
            './node_modules/jquery/dist/jquery.min.js',
            './node_modules/jquery/dist/jquery.min.map',
            './node_modules/jquery-validation/dist/jquery.validate.min.js',
            './node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.min.js',
            './node_modules/bootstrap/dist/js/bootstrap.min.js',
            './node_modules/bootstrap/dist/js/bootstrap.min.js.map',
            './node_modules/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js',
            './node_modules/moment/dist/moment.js',
            './node_modules/chart.js/dist/chart.min.js',
        ]).pipe(gulp.dest('./wwwroot/js/'));
};

const thirdPartyCss = () => {
    return gulp
        .src([
            './node_modules/bootstrap/dist/css/bootstrap.min.css',
            './node_modules/bootstrap/dist/css/bootstrap.min.css.map',
            './node_modules/font-awesome/css/font-awesome.min.css',
            './node_modules/font-awesome/css/font-awesome.css.map',
            './node_modules/bootstrap-datepicker/dist/css/bootstrap-datepicker3.min.css',
            './node_modules/bootstrap-datepicker/dist/css/bootstrap-datepicker3.css.map',
            './node_modules/chart.js/dist/chart.min.css',
        ]).pipe(gulp.dest('./wwwroot/css/'));
};

const thirdPartyFonts = () => {
    return gulp
        .src([
            './node_modules/font-awesome/fonts/*'
        ]).pipe(gulp.dest('./wwwroot/fonts/'));
};

gulp.task('default', gulp.parallel(js, css, icon, thirdPartyJs, thirdPartyCss, thirdPartyFonts));