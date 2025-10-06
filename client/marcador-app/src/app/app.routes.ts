import { Routes } from '@angular/router';
import { MarcadorPageComponent } from './pages/marcador-page';

export const routes: Routes = [
  { path: '', component: MarcadorPageComponent },
  { path: '**', redirectTo: '' }
];
