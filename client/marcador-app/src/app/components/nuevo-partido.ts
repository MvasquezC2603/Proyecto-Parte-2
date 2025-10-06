// src/app/components/nuevo-partido.ts
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../services/api';
import { PartidoService } from '../services/partido';

@Component({
  selector: 'app-nuevo-partido',
  standalone: true,
  imports: [FormsModule],
  template: `
    <h3>Nuevo Partido</h3>

    <form (ngSubmit)="crear()" style="display:flex; gap:12px; align-items:center; flex-wrap:wrap;">
      <label>
        Equipo Local
        <input [(ngModel)]="home" name="home" required />
      </label>

      <label>
        Equipo Visita
        <input [(ngModel)]="away" name="away" required />
      </label>

      <label>
        Duración (min)
        <input type="number" [(ngModel)]="mins" name="mins" min="1" />
      </label>

      <button type="submit">Comenzar</button>
    </form>
  `,
})
export class NuevoPartidoComponent {
  // 👇 estas propiedades evitan el error "Property 'home/away' does not exist"
  home = 'Local';
  away = 'Visita';
  mins = 10;

  constructor(private api: ApiService, private partido: PartidoService) {}

  crear() {
    // evita usar Math en el template
    const durationSeconds = Math.max(1, Number(this.mins || 10)) * 60;

    // 1) Actualiza el front
    this.partido.setTeams(this.home, this.away);
    this.partido.durationSeconds = durationSeconds;
    this.partido.reiniciar();

    // 2) Notifica a la API (si falla, seguimos localmente)
    this.api.startMatch({
      homeTeam: this.home,
      awayTeam: this.away,
      durationSeconds,
    }).subscribe({
      next: _ => {},
      error: err => {
        console.error('No se pudo iniciar en API', err);
        alert('No se pudo iniciar el partido en la API. Se continuará localmente.');
      }
    });
  }
}
