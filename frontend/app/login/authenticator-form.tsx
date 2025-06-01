"use client"

import {zodResolver} from "@hookform/resolvers/zod"
import {useForm} from "react-hook-form"
import {z} from "zod"
import {Form, FormControl, FormField, FormItem, FormLabel, FormMessage} from "@/components/ui/form";
import {Button} from "@/components/ui/button";
import {Input} from "@/components/ui/input";
import {authenticate} from "@/app/actions/auth";
import {AuthModes} from "@/lib/types";
import {toast} from "sonner";

export const authFormSchema = z.object({
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

interface AuthenticationFormProps {
  mode: AuthModes;
}

export function AuthenticationForm({mode}: AuthenticationFormProps) {
  const form = useForm<z.infer<typeof authFormSchema>>({
    resolver: zodResolver(authFormSchema),
    defaultValues: {
      email: "",
      username: "",
      password: "",
    },
  })

  const handleSubmit = async (formData: FormData) => {
    const data = Object.fromEntries(formData.entries()) as { email: string, username: string, password: string };
    const success = await authenticate(data, mode);

    if(!success) toast.error("Failed to authenticate");
  }

  return (
    <div className={'w-full mt-4'}>
      <Form {...form}>
        <form action={handleSubmit} className="space-y-8">

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