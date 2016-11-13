"use strict";
var router_1 = require('@angular/router');
var login_form_component_1 = require('../public/login-form/login-form.component');
var user_list_component_1 = require('../public/user-list/user-list.component');
exports.routes = [
    {
        path: '',
        redirectTo: 'Users',
        pathMatch: 'full'
    },
    //{ path: '', component: UserListComponent },
    { path: 'Users', component: user_list_component_1.UserListComponent },
    { path: 'Login', component: login_form_component_1.LoginFormComponent },
    { path: 'Users/:page', component: user_list_component_1.UserListComponent }
];
exports.UsersRouterModule = router_1.RouterModule.forRoot(exports.routes, { useHash: true });
//# sourceMappingURL=app.routes.js.map