import { Routes } from '@angular/router';

const Routing: Routes = [
  {
    path: 'dashboard',
    loadChildren: () => import('./dashboard/dashboard.module').then((m) => m.DashboardModule),
  },
  {
    path: 'builder',
    loadChildren: () => import('./builder/builder.module').then((m) => m.BuilderModule),
  },
  {
    path: 'crafted/account',
    loadChildren: () => import('../modules/account/account.module').then((m) => m.AccountModule),
  },  
  {
    path: 'apps/users',
    loadChildren: () => import('./user/user.module').then((m) => m.UserModule),
  },
  {
    path: 'apps/roles',
    loadChildren: () => import('./role/role.module').then((m) => m.RoleModule),
  },
  {
    path: 'apps/companies',
    loadChildren: () => import('./company/company.module').then((m) => m.CompanyModule),
  },
  {
    path: 'apps/hotels',
    loadChildren: () => import('./hotel/hotel.module').then((m) => m.HotelModule),
  },
  {
    path: 'apps/buildings',
    loadChildren: () => import('./building/building.module').then((m) => m.BuildingModule),
  },
  {
    path: 'apps/room-types',
    loadChildren: () => import('./room-type/room-type.module').then((m) => m.RoomTypeModule),
  },
  {
    path: 'apps/boards',
    loadChildren: () => import('./board/board.module').then((m) => m.BoardModule),
  },  
  {
    path: 'apps/templates',
    loadChildren: () => import('./template/template.module').then((m) => m.TemplateModule),
  },  
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full',
  },
  {
    path: '**',
    redirectTo: 'error/404',
  },
];

export { Routing };
