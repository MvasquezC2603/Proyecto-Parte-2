import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService, Match } from '../services/api';
import { PartidoService } from '../services/partido';

@Component({
  selector: 'app-nuevo-partido',
  standalone: true,
  imports: [FormsModule],
  template: `
  <h2>Nuevo Partido</h2>
  <form (ngSubmit)="crear()">
    <label>
      Equipo Local
      <input [(ngModel)]="home" name="home" required />
    </label>

    <label style="margin-left:12px;">
      Equipo Visita
      <input [(ngModel)]="away" name="away" required />
    </label>

    <label style="margin-left:12px;">
      Duración (min)
      <input type="number" [(ngModel)]="mins" name="mins" min="1" />
    </label>

    <button type="submit" style="margin-left:12px;">Comenzar</button>
  </form>
  `,
})
export class NuevoPartidoComponent {
  home = 'Local';
  away = 'Visita';
  mins = 10;

  constructor(private api: ApiService, private partido: PartidoService) {}

  crear() {
    const durationSeconds = Math.max(1, this.mins) * 60;

    // 1) setear en el front
    this.partido.setTeams(this.home, this.away);
    this.partido.durationSeconds = durationSeconds;
    this.partido.reiniciar();

    // 2) notificar a la API y GUARDAR EL ID DEVUELTO
    this.api.startMatch({ homeTeam: this.home, awayTeam: this.away, durationSeconds })
      .subscribe({
        next: (saved: Match) => {
          // 👇 AQUÍ la clave para no duplicar al finalizar
          if (saved?.id != null) {
            this.partido.setMatchId(null);
          }
        },
        error: err => {
          console.error('No se pudo iniciar en API', err);
          alert('No se pudo iniciar el partido en la API. Se continuará localmente.');
          this.partido.setMatchId(null);
        }
      });
  }
}
