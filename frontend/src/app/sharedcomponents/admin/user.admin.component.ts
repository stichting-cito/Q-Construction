import { Component, Input, Output, EventEmitter, DoCheck, IterableDiffers, ViewChild } from '@angular/core';
import { UserType, User, KeyValue } from './../../shared/model/model';
import { UserService } from './../../shared/services/user.service';
import { forkJoin } from 'rxjs';
import { ModalDirective } from 'ngx-bootstrap';
import { tap } from 'rxjs/operators';

@Component({
    moduleId: module.id,
    selector: 'app-admin-usersdialog',
    templateUrl: 'user.admin.component.html',
})

export class UserAdminDialogComponent implements DoCheck {
    @Input() visible: boolean;
    @Input() header: string;
    @Input() userType: UserType;
    @Input() usersNoPermission = new Array<User>();
    @Output() usersAdded = new EventEmitter<User[]>();
    public existingUser = false;
    differ: any;
    public selectedUsers = new Array<User>();
    public newUser = new User();
    public options: any;
    @ViewChild('userModal') userModal: ModalDirective;
    constructor(private userService: UserService, differs: IterableDiffers) {
        this.differ = differs.find([]).create(null);
    }

    show = (existingUser: boolean) => {
        this.existingUser = existingUser;
        this.userModal.show();
    }
    ngDoCheck() {
        const changes = this.differ.diff(this.usersNoPermission);
        if (changes) {
            this.selectedUsers = new Array<User>();
            this.options = [];
            this.usersNoPermission.forEach(u => this.options.push({ label: u.name, value: u }));
        }
    }

    toggleAddUser(e, user: User) {
        if (e.target.checked) {
            this.selectedUsers.push(user);
        } else {
            this.selectedUsers = this.selectedUsers.filter(u => u.id !== user.id);
        }
    }

    addUser() {
        forkJoin(this.selectedUsers.map(u => {
            if (!u.allowedWishlists) {
                u.allowedWishlists = new Array<KeyValue>();
            }
            u.allowedWishlists.push(this.userService.user.selectedWishlist);
            return this.userService.UpdatePermissions(u.id, u.allowedWishlists.map(aw => aw.id));
        })).subscribe(_ => {
            this.usersAdded.emit(this.selectedUsers);
            this.selectedUsers = new Array<User>();
            this.userModal.hide();
        });
    }
    onSubmit() {
        if (this.newUser.name && this.newUser.password && this.newUser.email) {
            this.newUser.userType = this.userType;
            this.userService.AddNew(this.newUser).subscribe(u => {
                if (!u.allowedWishlists) {
                    u.allowedWishlists = new Array<KeyValue>();
                }
                u.allowedWishlists.push(this.userService.user.selectedWishlist);
                this.userService.UpdatePermissions(u.id, u.allowedWishlists.map(aw => aw.id)).pipe(
                    tap(_ => {
                        this.selectedUsers.push(u);
                        this.usersAdded.emit([u]);
                        this.newUser = new User();
                        this.userModal.hide();
                    })
                ).subscribe();

            });
        }
    }
    getSelected = () => this.selectedUsers.map(u => u.name).join();

}
