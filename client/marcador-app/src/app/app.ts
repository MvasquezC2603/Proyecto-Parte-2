import { Component, OnInit } from '@angular/core';
import { ApiService } from './services/api';
import { PartidoService } from './services/partido';
import { MarcadorComponent } from './components/marcador/marcador';

// OJO: No importes Marcador, Faltas, Tiempo aquí

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [MarcadorComponent],
  templateUrl: './app.html',
  styleUrls: ['./app.scss'],
})
export class AppComponent implements OnInit {
  constructor(private api: ApiService, public partido: PartidoService) {}

  ngOnInit(): void {
    this.api.getInProgress().subscribe(m => {
      if (!m) return;
      this.partido.homeTeam.set(m.homeTeam || 'Local');
      this.partido.awayTeam.set(m.awayTeam || 'Visita');
      this.partido.scoreHome.set(m.scoreHome);
      this.partido.scoreAway.set(m.scoreAway);
      this.partido.foulsHome.set(m.foulsHome);
      this.partido.foulsAway.set(m.foulsAway);
      this.partido.quarter.set(m.quarter);
      this.partido.currentMatchId.set(m.id ?? null);
    });
  }
}
