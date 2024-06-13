export class User {
  companies: number[];
  createDate?: Date;
  email: string;
  emailConfirmed: boolean;
  firstname: string;
  id: string;
  lastLoginDate?: Date;
  lastname: string;
  roles: string[];  

  constructor(user?: User) {
    if (user) {
      this.companies = user.companies;
      this.createDate = user.createDate;
      this.email = user.email;
      this.emailConfirmed = user.emailConfirmed;
      this.firstname = user.firstname;
      this.id = user.id;
      this.lastLoginDate = user.lastLoginDate;
      this.lastname = user.lastname;
      this.roles = user.roles;
    }
  }

  isAtLeastOneRoleSelected() {
    if (this.roles) {
      for (let role in this.roles) {
        if (this.roles[role]) {
          return true;
        }
      }
      return false;
    }
  }

  public updateRole(role: string, isChecked: boolean) {
    if (role) {
      if (!this.roles) {
        this.roles = [];
      }
      if (!isChecked) {
        this.roles = this.roles.filter(function(id) {
          return id != role;
        });
      } else {
        let index = this.roles.findIndex((id) => id === role);
        if (index === -1) {
          this.roles.push(role);
        }
      }
    }
  }
}