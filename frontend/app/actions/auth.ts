"use server";

import {LoginReply, LoginRequest, RegisterReply, RegisterRequest} from "@/lib/proto/authentication_pb";
import {AuthenticatorClient, ServiceError} from "@/lib/proto/authentication_pb_service";
import {NodeHttpTransport} from "@improbable-eng/grpc-web-node-http-transport";
import {grpc} from "@improbable-eng/grpc-web";
import {cookies} from "next/headers";


const client = new AuthenticatorClient("http://localhost:5001")
grpc.setDefaultTransport(NodeHttpTransport())

function registerRequest(
  email: string,
  username: string,
  password: string
): Promise<string> {
  return new Promise((resolve, reject) => {
    const req = new RegisterRequest();
    req.setMailAddress(email);
    req.setUsername(username);
    req.setPassword(password);

    client.register(
      req,
      (err: ServiceError | null, reply: RegisterReply | null) => {
        if (err) {
          return reject(new Error(err.message));
        }
        if (reply?.hasFailuremessage()) {
          return reject(new Error(reply.getFailuremessage()));
        }
        if (reply?.hasToken()) {
          return resolve(reply.getToken());
        }
        return reject(new Error("No token returned"));
      }
    );
  });
}

function loginRequest(username: string, password: string): Promise<string> {
  return new Promise((resolve, reject) => {
    const request = new LoginRequest();
    request.setUsername(username);
    request.setPassword(password);

    client.login(
      request,
      (err: ServiceError | null, reply: LoginReply | null) => {
        if (err) {
          return reject(err.message);
        }
        if (reply?.hasFailuremessage()) {
          return reject(reply.getFailuremessage());
        }
        if (reply?.hasToken()) {
          return resolve(reply.getToken());
        }
        return reject('No Token returned');
      }
    );
  });
}

export async function signOut() {
  const store = await cookies();
  store.delete("token");
  store.delete("username")
}

export async function signUp(formData: FormData) {
  const {email, username, password} = Object.fromEntries(formData) as {email: string, username: string, password: string}

  try {
    const token = await registerRequest(email, username, password)
    const store = await cookies();

    store.set("username", username)
    store.set("token", token)
    return {ok: true, message: ""}
  } catch (errorMessage: unknown) {
    return {ok: false, message: String(errorMessage)};
  }
}

export async function signIn(formData: FormData) {
  const {username, password} = Object.fromEntries(formData) as {username: string, password: string}

  try {
    const token = await loginRequest( username, password)
    const store = await cookies();

    store.set("username", username)
    store.set("token", token)

    return {ok: true, message: ""}
  } catch (errorMessage) {
    return {ok: false, message: String(errorMessage)};
  }
}