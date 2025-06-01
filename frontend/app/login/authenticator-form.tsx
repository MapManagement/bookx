"use client"

import {zodResolver} from "@hookform/resolvers/zod"
import {useForm} from "react-hook-form"
import {z} from "zod"
import {Form, FormControl, FormField, FormItem, FormLabel, FormMessage} from "@/components/ui/form";
import {Button} from "@/components/ui/button";
import {Input} from "@/components/ui/input";
import {LoginReply, LoginRequest, RegisterReply, RegisterRequest} from "@/lib/proto/authentication_pb";
import {AuthenticatorClient, ServiceError} from "@/lib/proto/authentication_pb_service";
import {useState} from "react";
import {toast} from "sonner";

const formSchema = z.object({
  email: z.string().email({
    message: "Enter a valid email address",
  }),
  username: z.string({
    required_error: "Username is required"
  }).min(2, {
    message: "Enter a valid username",
  }),
  password: z.string({
    required_error: "Password is required"
  }).min(12, {
    message: "Enter a valid password",
  })
})

interface AuthenticatorFormProps {
  mode: "login" | "register";
}

export function AuthenticationForm({mode}: AuthenticatorFormProps) {
  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      email: "",
      username: "",
      password: "",
    },
  })
  const [hasTriedSubmitting, setTriedSubmitting] = useState<boolean>(false)
  const [submitLoading, setSubmitLoading] = useState<boolean>(false)
  const client = new AuthenticatorClient("http://localhost:5001")

  function register(email: string, username: string, password: string) {
    const request = new RegisterRequest();
    request.setMailAddress(email);
    request.setUsername(username);
    request.setPassword(password);

    client.register(
      request,
      (error, responseMessage) => handleResponse(error, responseMessage))
  }

  function login(username: string, password: string) {
    const request = new LoginRequest();
    request.setUsername(username);
    request.setPassword(password);

    client.login(
      request,
      (error, responseMessage) => handleResponse(error, responseMessage))
  }

  function handleResponse(error: ServiceError | null, responseMessage: LoginReply | RegisterReply | null) {
    if (error) {
      toast.error(error.message)
    } else if (responseMessage?.hasFailuremessage()) {
      toast.error(responseMessage?.getFailuremessage())
    } else if (responseMessage?.hasToken()) {
      toast.success("Login complete!");
      console.log("JWT Token: ", responseMessage?.getToken())
      setTriedSubmitting(false);
    }
  }

  function onValidSubmit(values: z.infer<typeof formSchema>) {
    setSubmitLoading(true);
    if (mode === "login") {
      login(values.username, values.password)
    } else {
      register(values.email, values.username, values.password)
    }
    setSubmitLoading(false);
  }

  return (
    <div className={'w-full mt-4'}>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onValidSubmit, () => setTriedSubmitting(true))} className="space-y-8">

          {mode === "register" && (
            <FormField
              control={form.control}
              name="email"
              render={({field}) => (
                <FormItem>
                  <FormLabel>E-Mail</FormLabel>
                  <FormControl>
                    <Input type={"email"} {...field} />
                  </FormControl>
                  <FormMessage/>
                </FormItem>
              )}
            />
          )}

          <FormField
            control={form.control}
            name="username"
            render={({field}) => (
              <FormItem>
                <FormLabel>Username</FormLabel>
                <FormControl>
                  <Input {...field} />
                </FormControl>
                <FormMessage/>
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="password"
            render={({field}) => (
              <FormItem>
                <FormLabel>Password</FormLabel>
                <FormControl>
                  <Input type={"password"} {...field} />
                </FormControl>
                <FormMessage/>
              </FormItem>
            )}
          />
          <Button
            type="submit"
            className={'w-full'}
            disabled={!form.formState.isValid && hasTriedSubmitting}
          >
            {
              submitLoading ? "..." :
                mode === "login" ? "Login" : "Register"
            }
          </Button>
        </form>
      </Form>
    </div>
  )
}