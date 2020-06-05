import { MemberEditComponent } from './member-edit/member-edit.component';
import { MembersResolver } from './resolvers/members.resolver';
import { MemberDetailResolver } from './resolvers/member-detail.resolver';
import { MemberDetailComponent } from './member-detail/member-detail.component';
import { AuthGuard } from './guards/auth.guard';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { MembersComponent } from './members/members.component';
import { HomeComponent } from './home/home.component';
import { Routes } from '@angular/router';
import { MemberEditResolver } from './resolvers/member-edit.resolver';
import { PreventUnsavedChanges } from './guards/prevent-unsaved-changes-guard';

export const appRoutes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'home', component: HomeComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      { path: 'members', component: MembersComponent, resolve: {users: MembersResolver} },
      { path: 'members/:id', component: MemberDetailComponent, resolve: {user: MemberDetailResolver} },
      { path: 'member/edit', component: MemberEditComponent, resolve: {user: MemberEditResolver}, canDeactivate: [PreventUnsavedChanges] },
      { path: 'messages', component: MessagesComponent },
      { path: 'lists', component: ListsComponent }
    ]
  },
  { path: '**', redirectTo: 'home', pathMatch: 'full' },
];
