export enum AuthModes {
  login,
  register,
}

export interface JansCooleJWTPayloads {
  email: string
  exp: number
  iss: string
  sub: string,
}