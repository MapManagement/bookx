import "server-only"
import {cookies} from "next/headers";

export async function getCurrentUserName() {
  const store = await cookies()
  return store.get("username")?.value
}

