using Salesforce.Common;
using Salesforce.Force;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleConsole
{
    class Program
    {
        private static readonly string SecurityToken = ConfigurationSettings.AppSettings["SecurityToken"];
        private static readonly string ConsumerKey = ConfigurationSettings.AppSettings["ConsumerKey"];
        private static readonly string ConsumerSecret = ConfigurationSettings.AppSettings["ConsumerSecret"];
        private static readonly string Username = ConfigurationSettings.AppSettings["Username"];
        private static readonly string Password = ConfigurationSettings.AppSettings["Password"] + SecurityToken;

        static void Main()
        {
            try
            {
                var task = RunSample();
                task.Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);

                var innerException = e.InnerException;
                while (innerException != null)
                {
                    Console.WriteLine(innerException.Message);
                    Console.WriteLine(innerException.StackTrace);

                    innerException = innerException.InnerException;
                }
            }
        }

        private static async Task RunSample()
        {
            var auth = new AuthenticationClient();

            // Autenticando e conectando
            Console.WriteLine("Autenticando no Salesforce");
            await auth.UsernamePasswordAsync(ConsumerKey, ConsumerSecret, Username, Password);
            Console.WriteLine("\r\n");
            Console.ReadKey();

            var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);
            Console.WriteLine("Conectado ao Salesforce");
            Console.WriteLine("\r\n");
            Console.ReadKey();


            // Criando registro
            Console.WriteLine("Criando registro de teste.");
            var account = new Account { Name = "Douglas Golino" };
            account.Id = await client.CreateAsync(Account.SObjectTypeName, account);
            if (account.Id == null)
            {
                Console.WriteLine("Falha ao criar o registro.");
                return;
            }

            Console.WriteLine("Registro criado com sucesso.");
            Console.WriteLine("\r\n");
            Console.ReadKey();

            // Consultando por ID
            Console.WriteLine("Consultando registro por ID.");
            account = await client.QueryByIdAsync<Account>(Account.SObjectTypeName, account.Id);
            if (account == null)
            {
                Console.WriteLine("Falha ao realizar a consulta por ID.");
                return;
            }

            Console.WriteLine("Registro recuperado com sucesso.");
            Console.WriteLine("Id: " + account.Id + " Nome: " + account.Name);
            Console.WriteLine("\r\n");
            Console.ReadKey();

            // Atualizando registro
            Console.WriteLine("Atualizando registro de teste");
            var success = await client.UpdateAsync(Account.SObjectTypeName, account.Id, new { Name = "Douglas Aguiar" });
            if (!success)
            {
                Console.WriteLine("Falha ao atualizar o registro.");
                return;
            }

            Console.WriteLine("Registro atualizado com sucesso.");
            Console.WriteLine("\r\n");
            Console.ReadKey();

            // Consultando usando Query - por nome
            Console.WriteLine("Consultando registro por Query.");
            var accounts = await client.QueryAsync<Account>("SELECT ID, Name FROM Account WHERE Name = 'Douglas Aguiar'");
            account = accounts.FirstOrDefault();
            if (account == null)
            {
                Console.WriteLine("Falha ao consultar o registro usando Query");
                return;
            }

            Console.WriteLine("Registro recuperado com sucesso.");
            Console.WriteLine("Id: " + account.Id + " Nome: " + account.Name);
            Console.WriteLine("\r\n");
            Console.ReadKey();

            // Deletando Conta
            Console.WriteLine("Deletando registro por ID");
            success = await client.DeleteAsync(Account.SObjectTypeName, account.Id);
            if (!success)
            {
                Console.WriteLine("Falha ao deletar o registro por ID");
                return;
            }
            Console.WriteLine("Registro deletado com sucesso.");
            Console.WriteLine("\r\n");
            Console.ReadKey();
        }

        private class Account
        {
            public const String SObjectTypeName = "Account";

            public String Id { get; set; }
            public String Name { get; set; }
        }
    }
}
