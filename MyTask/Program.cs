using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyTask
{
    interface IHttpGetExtention
    {
        void Execute(string url,ref string output);
    }

    class StringUseCase : IHttpGetExtention
    {
        public void Execute(string url,ref string output)
        {
            using (var client = new HttpClient())
            {
                var httpContent =Task.Run(() => client.GetAsync(url)) ;
                
                var data =  Task.Run(() => httpContent.Result.Content.ReadAsStringAsync());
                output = data.Result;
                //deserilize for different data
            }

        }

       
    }

    class IntUseCase : IHttpGetExtention
    {
        public void Execute(string url, ref string output)
        {
            using (var client = new HttpClient())
            {
                var httpContent = Task.Run(() => client.GetAsync(url));

                var data = Task.Run(() => httpContent.Result.Content.ReadAsStringAsync());
                output = data.Result;
                //deserilize for different data
            }

        }
    }
    internal class Program
    {

        static void Main(string[] args)
        {

            var stringUserCase = new StringUseCase();

            string resultForStringUSeCase="";
            stringUserCase.Execute("https://kaverin-ddb.firebaseio.com/interview/useCase/string.json", ref resultForStringUSeCase);


            var intUseCase = new IntUseCase();

            string resultForIntUseCas="";
            intUseCase.Execute("https://kaverin-ddb.firebaseio.com/interview/useCase/int.json",ref resultForIntUseCas);


            Console.WriteLine("Result for string use case :"+ resultForStringUSeCase);
            Console.WriteLine("Result for int use case :"+ resultForIntUseCas);
            
        }
    }
}
