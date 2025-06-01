"use server"

import {z} from "zod";
import {authFormSchema} from "@/app/login/authenticator-form";
import {AuthenticatorClient, ServiceError} from "@/lib/proto/authentication_pb_service";
import {LoginReply, LoginRequest, RegisterReply, RegisterRequest} from "@/lib/proto/authentication_pb";
import {cookies} from "next/headers";
import {AuthModes} from "@/lib/types";

const client = new AuthenticatorClient("http://localhost:5001")

let success = false

export async function signOut() {
  const store = await cookies()
  store.delete("username")
  store.delete("token")
}

export async function authenticate (formData: z.infer<typeof authFormSchema>, mode: AuthModes) {
  const {email, username, password} = formData

  if (mode === AuthModes.register) {
    signUp(email, username, password);
  } else {
    signIn(username, password);
  }

  return success;
}

function signUp(email: string, username: string, password: string) {
  const request = new RegisterRequest()

  request.setMailAddress(email)
  request.setUsername(username)
  request.setPassword(password)

  client.register(request, (error, reply) => {
    void handleReply(error, reply, username)
  })

}

function signIn(username: string, password: string) {
  const request = new LoginRequest()


  request.setUsername(username)
  request.setPassword(password)

  client.login(request, (error, reply) => {
    void handleReply(error, reply, username)
  })
}

async function handleReply (error: ServiceError | null, reply: LoginReply | RegisterReply | null, username: string) {
  const store = await cookies()

  if (error) {
    console.log(error)

  } else if (reply && reply.hasFailuremessage()) {
    console.log(reply.getFailuremessage())

  } else if(reply && reply.hasToken()){
    store.set("username", username)
    store.set("token", reply.getToken(), {
        httpOnly: true,
        secure: true,
        sameSite: "lax",
        path: "/"
    })
    success = true
  }
}