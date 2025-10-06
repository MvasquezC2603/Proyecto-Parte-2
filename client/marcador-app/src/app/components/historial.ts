import { Component, OnInit } from '@angular/core';
import { NgFor } from '@angular/common';
import { ApiService, Match } from '../services/api'; // 👈 ruta y tipos correctos

@Component({
  selector: 'app-historial',
  standalone: true,
  imports: [NgFor],
  template: `
    <h3>Historial de Partidos</h3>
    <ul>
      <li *ngFor="let m of matches">
        #{{ m.id }} — {{ m.homeTeam }} {{ m.scoreHome }} : {{ m.scoreAway }} {{ m.awayTeam }}
        (Q{{ m.quarter }} · {{ m.status }})
      </li>
    </ul>
  `
})
export class HistorialComponent implements OnInit {
  matches: Match[] = [];  // 👈 tipado explícito

  constructor(private api: ApiService) {} // 👈 ApiService inyectable

  ngOnInit(): void {
    this.api.getMatches().subscribe((ms: Match[]) => { // 👈 sin any implícito
      this.matches = ms;
    });
  }
}
