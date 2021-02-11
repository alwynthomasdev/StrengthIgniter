"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.AppRoutes = void 0;
exports.AppRoutes = [
    { path: '', loadChildren: './main/main.module#MainModule' },
    { path: 'auth', loadChildren: './auth/auth.module#AuthModule' }
];
//# sourceMappingURL=app.routes.js.map