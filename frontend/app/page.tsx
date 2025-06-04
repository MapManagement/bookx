import verifySession from "@/lib/verify-session";
import {redirect} from "next/navigation";

export default async function Home() {
  await verifySession();
  redirect('/overview')
}