import { Component } from '@angular/core';
import { PartidoService } from '../../services/partido';

import { ControlesComponent } from '../controles/controles';
import { NuevoPartidoComponent } from '../nuevo-partido';
import { HistorialComponent } from '../historial';

@Component({
  selector: 'app-marcador',
  standalone: true,
  imports: [ControlesComponent, NuevoPartidoComponent, HistorialComponent],
  templateUrl: './marcador.html',
})
export class MarcadorComponent {
  constructor(public partido: PartidoService) {}
}
