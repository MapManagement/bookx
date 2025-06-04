import "server-only"
import {jwtDecode} from "jwt-decode"
import {redirect} from "next/navigation";
import {cookies} from "next/headers";

export default async function verifySession() {
  const store = await cookies()
  const username = store.get("username")?.value
  const token = store.get("token")?.value

  if(!username || !token) redirect('/login')

  const decodedToken = jwtDecode(token)
  console.log(decodedToken.exp)
  console.log(Date.now())
  const isTokenExpired: boolean = decodedToken.exp ? (decodedToken.exp < Date.now()) : false

  if (isTokenExpired) {
    redirect('/login')
  }
}