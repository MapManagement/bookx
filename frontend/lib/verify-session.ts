import "server-only";
import {jwtDecode} from "jwt-decode";
import {redirect} from "next/navigation";
import {cookies} from "next/headers";

export default async function verifySession() {
  const store = await cookies();
  const mail = store.get("mail")?.value;
  const token = store.get("token")?.value;

  if(!mail || !token) redirect('/login');
  const decodedToken = jwtDecode(token);
  const isTokenExpired: boolean = decodedToken.exp ? (decodedToken.exp < Date.now() / 1000) : false;
  if (isTokenExpired) redirect('/login');
}