System.config({
  map: {
    'angular2-recaptcha': 'node_modules/angular2-recaptcha'
  },
  packages: {
    app: {
      format: 'register',
      defaultExtension: 'js'
    },
    'angular2-recaptcha': { defaultExtension: 'js', main: 'index' }
  }
});
