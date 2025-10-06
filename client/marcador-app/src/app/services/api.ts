import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Match {
  id?: number;
  homeTeam: string;
  awayTeam: string;
  quarter: number;
  scoreHome: number;
  scoreAway: number;
  foulsHome: number;
  foulsAway: number;
  startAt: string | null;           // puede ser null si no lo usas
  endAt: string | null;             // null mientras esté en curso
  status: 'in_progress' | 'finished';
}

const API_BASE = '/api';            // mismo origen (http://localhost:8081)
const MATCHES = `${API_BASE}/matches`;

@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(private http: HttpClient) {}

  // Historial
  getMatches(): Observable<Match[]> {
    return this.http.get<Match[]>(MATCHES);
  }

  // Partido en curso
  getInProgress(): Observable<Match | null> {
    return this.http.get<Match | null>(`${MATCHES}/in-progress`);
  }

  // Crear partido (general)
  createMatch(match: Match): Observable<Match> {
    return this.http.post<Match>(MATCHES, match);
  }

  // Actualizar por id
  updateMatch(id: number, match: Match): Observable<Match> {
    return this.http.put<Match>(`${MATCHES}/${id}`, match);
  }

  // Iniciar partido desde el front
  startMatch(input: { homeTeam: string; awayTeam: string; durationSeconds: number }): Observable<Match> {
    const body: Match = {
      homeTeam:  input.homeTeam,
      awayTeam:  input.awayTeam,
      quarter:   1,
      scoreHome: 0,
      scoreAway: 0,
      foulsHome: 0,
      foulsAway: 0,
      startAt:   new Date().toISOString(),
      endAt:     null,
      status:    'in_progress',
    };
    // OJO: endpoint correcto
    return this.http.post<Match>(MATCHES, body);
  }
}
