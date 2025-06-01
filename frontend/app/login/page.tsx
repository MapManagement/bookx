import {AuthenticationForm} from "@/app/login/authenticator-form";
import {Tabs, TabsContent, TabsList, TabsTrigger} from "@/components/ui/tabs";

export default function LoginPage() {
  return (
    <div className={'w-[100vw] h-[100vh] flex justify-center items-center'}>
      <Tabs defaultValue="login" className="w-[400px]">
        <TabsList className={'w-full flex space-x-5'}>
          <TabsTrigger value="login">Login</TabsTrigger>
          <TabsTrigger value="register">Register</TabsTrigger>
        </TabsList>
        <TabsContent value="login"><AuthenticationForm mode={"login"} /></TabsContent>
        <TabsContent value="register"><AuthenticationForm mode={"register"} /></TabsContent>
      </Tabs>
    </div>
  )
}