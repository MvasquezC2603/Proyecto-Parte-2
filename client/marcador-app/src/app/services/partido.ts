import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class PartidoService {
  // Estado principal
  homeTeam = signal('Local');
  awayTeam = signal('Visita');
  currentMatchId = signal<number | null>(null);   // id de la API, si existe

  scoreHome = signal(0);
  scoreAway = signal(0);
  foulsHome = signal(0);
  foulsAway = signal(0);
  quarter   = signal(1);

  // Reloj
  durationSeconds = 600;                    // 10 min por defecto
  remainingSeconds = signal(this.durationSeconds);
  running = signal(false);
  private timerId: any = null;

  // --- utilitarios ---
  setTeams(home: string, away: string) {
    this.homeTeam.set(home?.trim() || 'Local');
    this.awayTeam.set(away?.trim() || 'Visita');
  }

  setMatchId(id: number | null) {
    this.currentMatchId.set(id);
  }

  // --- puntos ---
  sumarPuntos(equipo: 'home' | 'away', n: number) {
    if (equipo === 'home') this.scoreHome.set(this.scoreHome() + n);
    else                   this.scoreAway.set(this.scoreAway() + n);
  }

  restarPuntos(equipo: 'home' | 'away', n: number) {
    if (equipo === 'home') this.scoreHome.set(Math.max(0, this.scoreHome() - n));
    else                   this.scoreAway.set(Math.max(0, this.scoreAway() - n));
  }

  // --- faltas ---
  falta(equipo: 'home' | 'away') {
    if (equipo === 'home') this.foulsHome.set(this.foulsHome() + 1);
    else                   this.foulsAway.set(this.foulsAway() + 1);
  }

  // --- reloj ---
  iniciar() {
    if (this.running()) return;
    this.running.set(true);
    this.timerId = setInterval(() => {
      const next = this.remainingSeconds() - 1;
      if (next >= 0) this.remainingSeconds.set(next);
      if (next <= 0) this.pausar();
    }, 1000);
  }

  pausar() {
    if (this.timerId) clearInterval(this.timerId);
    this.timerId = null;
    this.running.set(false);
  }

  reiniciar() {
    this.pausar();
    this.scoreHome.set(0);
    this.scoreAway.set(0);
    this.foulsHome.set(0);
    this.foulsAway.set(0);
    this.quarter.set(1);
    this.remainingSeconds.set(this.durationSeconds);
  }

  avanzarCuarto() {
    this.pausar();
    this.quarter.set(Math.min(this.quarter() + 1, 4));
    this.remainingSeconds.set(this.durationSeconds);
  }

  // --- helpers ---
  formatMMSS(totalSeconds: number) {
    const m = Math.floor(totalSeconds / 60);
    const s = totalSeconds % 60;
    return `${m.toString().padStart(2,'0')}:${s.toString().padStart(2,'0')}`;
  }
}
// Servicio para manejar el estado de un partido deportivo (baloncesto)