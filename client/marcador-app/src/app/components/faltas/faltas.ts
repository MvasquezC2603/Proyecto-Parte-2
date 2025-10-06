import { Component } from '@angular/core';
import { PartidoService } from '../../services/partido';

@Component({
  selector: 'app-faltas',
  standalone: true,
  templateUrl: './faltas.html'
})
export class FaltasComponent {
  constructor(public partido: PartidoService) {}
}
