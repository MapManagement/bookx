"use client"

import {Authenticator} from "@/lib/proto/authentication_pb_service";
import {RegisterRequest} from "@/lib/proto/authentication_pb";
import {Button} from "@/components/ui/button";
import {grpc} from "@improbable-eng/grpc-web";


export default function Home() {

  function register() {
    const request = new RegisterRequest();
    request.setUsername("tester");
    request.setPassword("bookx_user");
    request.setMailAddress("admin@bookx.local");

    grpc.unary(Authenticator.Register, {
      request,
      host: "http://localhost:5001",
      onEnd: res => {
        const { status, statusMessage, message } = res;
        if (status === grpc.Code.OK && message) {
          console.log("Success", message.toObject());
        } else {
          console.error("gRPC Error", status, statusMessage);
        }
      }
    });
  }



  return (
    <div className={'w-[100vw] h-[100vh] flex justify-center items-center'}>
      <Button onClick={register} variant={"destructive"}>Register</Button>
    </div>
  )
}
