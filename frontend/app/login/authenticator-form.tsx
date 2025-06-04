"use client"

import {zodResolver} from "@hookform/resolvers/zod"
import {useForm} from "react-hook-form"
import {z} from "zod"
import {Form, FormControl, FormField, FormItem, FormLabel, FormMessage} from "@/components/ui/form";
import {Button} from "@/components/ui/button";
import {Input} from "@/components/ui/input";
import {signIn, signUp} from "@/app/actions/auth";
import {AuthModes} from "@/lib/types";
import {redirect} from "next/navigation";
import {toast} from "sonner";
import React from "react";

const registerFormSchema = z.object({
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
type RegisterData = z.infer<typeof registerFormSchema>

const loginFormSchema =  z.object({
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
type LoginData = z.infer<typeof loginFormSchema>


interface AuthenticationFormProps {
  mode: AuthModes;
}

export function AuthenticationForm({mode}: AuthenticationFormProps) {
  const form = useForm<LoginData | RegisterData>({
    resolver: zodResolver(mode === AuthModes.login ? loginFormSchema : registerFormSchema),
    defaultValues: {
      email: "",
      username: "",
      password: "",
    },
  })
  const authAction = mode === AuthModes.login ? signIn : signUp

  return (
    <div className={'w-full mt-4'}>
      <Form {...form}>
        <form
          action={async(formData) =>{
            const answer = await authAction(formData)

            if(answer.ok) {
              toast.success("YAY")
              redirect("/")
            } else {
              toast.error(answer.message)
            }
          }}

          className="space-y-8"
        >
          {mode == AuthModes.register && (
            <FormField
              control={form.control}
              name="email"
              render={({field}) => (
                <FormItem>
                  <FormLabel>E-Mail</FormLabel>
                  <FormControl>
                    <Input className={'bg-input'} type={"email"} {...field} />
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
                  <Input className={'bg-input'} {...field} />
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
                  <Input className={'bg-input'} type={"password"} {...field} />
                </FormControl>
                <FormMessage/>
              </FormItem>
            )}
          />
          <Button
            type="submit"
            className={'w-full'}
            disabled={!form.formState.isValid}
          >
            {mode == AuthModes.login ? "Login" : "Register"}
          </Button>
        </form>
      </Form>
    </div>
  )
}