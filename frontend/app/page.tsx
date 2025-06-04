import verifySession from "@/lib/verify-session";
import {getCurrentUserName} from "@/lib/user";

export default async function Home() {
  await verifySession()

  return (
    <div className={'w-[100vw] h-[100vh] flex justify-center items-center'}>
      <p className={'text-2xl text-destructive'}>{await getCurrentUserName() ?? "Could not decode session"}</p>
    </div>
  )
}