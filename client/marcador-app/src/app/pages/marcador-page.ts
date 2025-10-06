import { Component } from '@angular/core';
import { ControlesComponent } from '../components/controles/controles';
import { MarcadorComponent } from '../components/marcador/marcador';
import { NuevoPartidoComponent } from '../components/nuevo-partido';
import { HistorialComponent } from '../components/historial';

@Component({
  selector: 'app-marcador-page',
  standalone: true,
  imports: [ControlesComponent, MarcadorComponent, NuevoPartidoComponent, HistorialComponent],
  template: `
    <h1 style="margin-bottom:16px">Marcador de Basket</h1>
    <app-controles></app-controles>
    <app-marcador></app-marcador>
    <app-nuevo-partido></app-nuevo-partido>
    <app-historial></app-historial>
  `
})
export class MarcadorPageComponent {}
