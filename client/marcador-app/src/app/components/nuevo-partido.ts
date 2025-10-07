import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService, Match } from '../services/api';
import { PartidoService } from '../services/partido';

@Component({
  selector: 'app-nuevo-partido',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './nuevo-partido.html'
})
export class NuevoPartidoComponent {
  home = '';
  away = '';
  minutes = 10;

  constructor(private api: ApiService, public partido: PartidoService) {}

  comenzar() {
    // 1) Preparar estado local inmediato (para que la UI muestre los nombres al instante)
    const homeTeam = (this.home || 'Local').trim();
    const awayTeam = (this.away || 'Visita').trim();
    const durationSeconds = Math.max(1, Math.floor(this.minutes)) * 60;

    this.partido.homeTeam.set(homeTeam);
    this.partido.awayTeam.set(awayTeam);

    // si tu servicio expone durationSeconds como propiedad:
    (this.partido as any).durationSeconds = durationSeconds;

    // reset de reloj y contadores según la duración recién fijada
    this.partido.reiniciar();

    // 2) Intentar iniciar en la API con /start (si existe)
    const startPayload = { homeTeam, awayTeam, durationSeconds };

    this.api.startMatch(startPayload).subscribe({
      next: (m) => {
        // sincronizar id y cualquier valor devuelto por la API
        if (m?.id != null) this.partido.currentMatchId.set(m.id);
        // dejar equipos por si API normaliza mayúsculas, etc.
        this.partido.homeTeam.set(m.homeTeam ?? homeTeam);
        this.partido.awayTeam.set(m.awayTeam ?? awayTeam);
      },
      error: (_err) => {
        // 3) Fallback: si /start no existe o falla, crear partido con POST /api/matches
        const now = new Date().toISOString();
        const match: Match = {
          homeTeam,
          awayTeam,
          quarter: 1,
          scoreHome: 0,
          scoreAway: 0,
          foulsHome: 0,
          foulsAway: 0,
          startAt: now,
          endAt: null,
          status: 'in_progress'
        };

        this.api.createMatch(match).subscribe({
          next: (saved) => {
            if (saved?.id != null) this.partido.currentMatchId.set(saved.id);
          },
          error: () => {
            // Sin API: continuar localmente
            alert('No se pudo iniciar el partido en la API. Se continuará localmente.');
          }
        });
      }
    });
  }
}
