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
  startAt: string;          // ISO string
  endAt: string | null;     // puede ser null si sigue en curso
  status: 'in_progress' | 'finished';
}

const API_URL = '/api/matches';

@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(private http: HttpClient) {}

  // Historial completo (para el componente Historial)
  getMatches(): Observable<Match[]> {
    return this.http.get<Match[]>(API_URL);
  }

  // Partido en curso (para restaurar estado al cargar)
  getInProgress(): Observable<Match | null> {
    return this.http.get<Match | null>(`${API_URL}/in-progress`);
  }

  // Crear partido (POST genérico)
  createMatch(match: Match): Observable<Match> {
    return this.http.post<Match>(API_URL, match);
  }

  // Actualizar partido por id (PUT)
  updateMatch(id: number, match: Match): Observable<Match> {
    return this.http.put<Match>(`${API_URL}/${id}`, match);
  }

  // Iniciar partido desde el front (usado por NuevoPartidoComponent)
  // Enviamos los datos mínimos + estado 'in_progress'.
  // Si tu endpoint /start también recibe durationSeconds, lo incluimos.
  startMatch(input: { homeTeam: string; awayTeam: string; durationSeconds: number }): Observable<Match> {
    const body: Match & { durationSeconds?: number } = {
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
      // Si tu Program.cs no usa duración, puedes quitar esta línea.
      durationSeconds: input.durationSeconds,
    };
    return this.http.post<Match>(`${API_URL}/start`, body);
  }
}
