export interface Match {
  id?: number;
  homeTeam: string;
  awayTeam: string;
  startAt: string;     // ISO string
  endAt?: string | null;
  quarter: number;
  scoreHome: number;
  scoreAway: number;
  foulsHome: number;
  foulsAway: number;
  status: 'in_progress' | 'finished' | 'scheduled';
}
