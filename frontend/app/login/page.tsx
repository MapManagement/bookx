import {AuthenticationForm} from "@/app/login/authenticator-form";
import {Tabs, TabsContent, TabsList, TabsTrigger} from "@/components/ui/tabs";
import {Card} from "@/components/ui/card";
import {AuthModes} from "@/lib/types";

export default function LoginPage() {
  return (
    <div className={'w-[100vw] h-[100vh] flex justify-center items-center'}>
      <Card className={'p-12'}>
        <Tabs defaultValue="login" className="w-[400px]">
          <TabsList className={'w-full flex space-x-5'}>
            <TabsTrigger value="login">Login</TabsTrigger>
            <TabsTrigger value="register">Register</TabsTrigger>
          </TabsList>
          <TabsContent value="login"><AuthenticationForm mode={AuthModes.login} /></TabsContent>
          <TabsContent value="register"><AuthenticationForm mode={AuthModes.register} /></TabsContent>
        </Tabs>
      </Card>
    </div>
  )
}