import { Component } from '@angular/core';
import { PartidoService } from '../../services/partido';
import { ApiService, Match } from '../../services/api';

@Component({
  selector: 'app-controles',
  standalone: true,
  templateUrl: './controles.html',
})
export class ControlesComponent {

  constructor(
    public partido: PartidoService,
    private api: ApiService
  ) {}

  iniciar()       { this.partido.iniciar(); }
  pausar()        { this.partido.pausar(); }
  reiniciar()     { this.partido.reiniciar(); }
  avanzarCuarto() { this.partido.avanzarCuarto(); }

  private buildPayload(): Match {
    return {
      id: this.partido.currentMatchId() ?? undefined,
      homeTeam:  this.partido.homeTeam(),
      awayTeam:  this.partido.awayTeam(),
      startAt:   new Date().toISOString(), // si guardas el real, ponlo aquí
      endAt:     new Date().toISOString(),
      quarter:   this.partido.quarter(),
      scoreHome: this.partido.scoreHome(),
      scoreAway: this.partido.scoreAway(),
      foulsHome: this.partido.foulsHome(),
      foulsAway: this.partido.foulsAway(),
      status:    'finished',
    };
  }

  private afterSave(saved: Match, msg: string) {
    alert(`${msg} (Id ${saved.id ?? 'desconocido'})`);
    this.partido.setMatchId(null);  // 👈 limpiar el id en memoria (nombre correcto)
    this.partido.reiniciar();
    this.partido.quarter.set(1);
  }

  finalizar() {
    this.partido.pausar();

    const payload = this.buildPayload();
    const id = this.partido.currentMatchId();
    console.log('Finalizar -> currentMatchId:', id, 'payload:', payload);

    // 1) Si tenemos el id en memoria -> PUT directo
    if (id) {
      this.api.updateMatch(id, payload).subscribe({
        next: saved => this.afterSave(saved, 'Partido actualizado'),
        error: err => {
          console.error('PUT falló con id en memoria:', err);
          this.tryResolveByQueryingInProgress(payload); // fallback
        }
      });
      return;
    }

    // 2) Si NO hay id en memoria -> intentar encontrar el in-progress en la API
    this.tryResolveByQueryingInProgress(payload);
  }

  private tryResolveByQueryingInProgress(payload: Match) {
    this.api.getInProgress().subscribe({
      next: m => {
        if (m?.id) {
          console.log('Encontrado in-progress en API con id:', m.id);
          this.partido.setMatchId(m.id); // 👈 fijar id correcto
          this.api.updateMatch(m.id, payload).subscribe({
            next: saved => this.afterSave(saved, 'Partido actualizado (por búsqueda)'),
            error: err2 => {
              console.error('PUT tras búsqueda falló:', err2);
              this.createAsLastResort(payload);
            }
          });
        } else {
          console.warn('No hay in-progress en API, se hará POST');
          this.createAsLastResort(payload);
        }
      },
      error: err => {
        console.error('GET in-progress falló:', err);
        this.createAsLastResort(payload);
      }
    });
  }

  private createAsLastResort(payload: Match) {
    this.api.createMatch(payload).subscribe({
      next: saved => this.afterSave(saved, 'Partido guardado'),
      error: err => {
        console.error('Error al guardar en API (POST):', err);
        alert('No se pudo guardar en la API. Se dejó el resumen en consola.');
        console.log('Resumen local:', payload);
      }
    });
  }
}

