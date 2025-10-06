import { Component } from '@angular/core';
import { PartidoService } from '../../services/partido';

@Component({
  selector: 'app-tiempo',
  standalone: true,
  templateUrl: './tiempo.html'
})
export class TiempoComponent {
  constructor(public partido: PartidoService) {}
}
