export interface RegisterRequest {
  email: string;
  password: string;
  displayName: string;
  playerId: number | null;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResult {
  token: string;
  displayName: string;
  role: string;
}