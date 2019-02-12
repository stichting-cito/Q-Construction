import { Component } from '@angular/core';
import { UserService } from '../../shared/services/user.service';

@Component({
    moduleId: module.id,
    templateUrl: 'login.page.component.html'
})
export class LoginComponent {
    login: string;

    constructor(private userService: UserService) {
        this.userService.logout();
    }
    userLogin = () => this.userService.login();
}
