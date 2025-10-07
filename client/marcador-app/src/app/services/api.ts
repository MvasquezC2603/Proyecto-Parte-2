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
  startAt: string;           // ISO
  endAt: string | null;
  status: 'in_progress' | 'finished';
}

const API_URL = '/api/matches'; // ← usamos el proxy

@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(private http: HttpClient) {}

  getMatches(): Observable<Match[]> {
    return this.http.get<Match[]>(API_URL);
  }

  getInProgress(): Observable<Match | null> {
    return this.http.get<Match | null>(`${API_URL}/in-progress`);
  }

  createMatch(match: Match): Observable<Match> {
    return this.http.post<Match>(API_URL, match);
  }

  updateMatch(id: number, match: Match): Observable<Match> {
    return this.http.put<Match>(`${API_URL}/${id}`, match);
  }
  startMatch(input: { homeTeam: string; awayTeam: string; durationSeconds: number }): Observable<Match> {
    // el backend ignora duration si no la usa; no pasa nada por enviarla
    return this.http.post<Match>(`${API_URL}/start`, {
      homeTeam: input.homeTeam,
      awayTeam: input.awayTeam,
      quarter: 1,
      scoreHome: 0,
      scoreAway: 0,
      foulsHome: 0,
      foulsAway: 0,
      startAt: new Date().toISOString(),
      endAt: null,
      status: 'in_progress',
      durationSeconds: input.durationSeconds
    } as any);
  }
}

