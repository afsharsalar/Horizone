using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MyTask
{
    interface IHttpGetExtention
    {
        void Execute(string url,ref string outPut);
    }

    class StringUseCase : IHttpGetExtention
    {
        public void Execute(string url,ref string outPut)
        {
            using (var client = new HttpClient())
            {
                var httpContent =Task.Run(() => client.GetAsync(url)) ;
                
                var data =  Task.Run(() => httpContent.Result.Content.ReadAsStringAsync());
                outPut = data.Result;
                //deserilize for different data
            }

        }

       
    }

    class IntUseCase : IHttpGetExtention
    {
        public void Execute(string url, ref string outPut)
        {
            using (var client = new HttpClient())
            {
                var httpContent = Task.Run(() => client.GetAsync(url));

                var data = Task.Run(() => httpContent.Result.Content.ReadAsStringAsync());
                outPut = data.Result;
                //deserilize for different data
            }

        }
    }





   
    class EmitterUseCase
    {
        public event EventHandler Start;
        public event EventHandler Error;
        public event EventHandler Next;
        public event EventHandler Cancel;
        private ConsoleKey _key;
        //private Timer _timer = new Timer(TimeSpan.FromSeconds(5).TotalMilliseconds);





        public void LoadDataFromApi()
        {
            
            var intUseCase = new IntUseCase();
            var result = "";
            intUseCase.Execute("https://kaverin-ddb.firebaseio.com/interview/useCase/int.json", ref result);
            Console.WriteLine("Result is:" + result);
            
        }

        public void ReadAlwaysKeyword()
        {
            while (true)
            {
                var input = Console.ReadKey(false);
                _key = input.Key;

                if (_key == ConsoleKey.Escape)
                {
                    Console.WriteLine("You canceled the operation!");
                    break;
                }
            }
            
            
        }

        public void Trigger(CancellationToken ct)
        {
            try
            {

                Console.WriteLine("if you want to cancel press ESC or press Enter to continue");
                Task.Run(ReadAlwaysKeyword, ct);
                while (true)
                {
                    if (_key == ConsoleKey.Escape)
                    {
                        OnCancel(EventArgs.Empty);
                        break;
                    }
                    else
                    {
                        OnStart(EventArgs.Empty);
                        try
                        {
                            LoadDataFromApi();
                            OnNext(EventArgs.Empty);
                        }
                        catch (Exception e)
                        {
                            OnError(EventArgs.Empty);
                        }
                    }
                    //simulate timer for 5 second.
                    //it's better to user system timer but can't use timer with event...
                    Thread.Sleep(5000);
                  
                    

                }
                
            }
            catch (Exception)
            {
                OnError(EventArgs.Empty);
            }

        }

        public void OnStart(EventArgs e)
        {
            Console.WriteLine("Start at:"+DateTime.Now.ToString("HH:mm:ss tt"));
            Start?.Invoke(this,e);
        }

        public void OnNext(EventArgs e)
        {
            Console.WriteLine("data fetched successfully at:" + DateTime.Now.ToString("HH:mm:ss"));
            Next?.Invoke(this, e);
        }

        public void OnError(EventArgs e)
        {
            Console.WriteLine("Error....");
            Error?.Invoke(this, e);
        }

      
        public void OnCancel(EventArgs e)
        {
            Console.WriteLine("Canceled at:" + DateTime.Now.ToString("HH:mm:ss tt"));
            Cancel?.Invoke(this, e);
            
        }
    }


    internal class Program
    {

        static void Main(string[] args)
        {
            //StringUseCase and IntUseCase
            var stringUserCase = new StringUseCase();

            string resultForStringUSeCase="";
            stringUserCase.Execute("https://kaverin-ddb.firebaseio.com/interview/useCase/string.json", ref resultForStringUSeCase);


            var intUseCase = new IntUseCase();

            string resultForIntUseCas="";
            intUseCase.Execute("https://kaverin-ddb.firebaseio.com/interview/useCase/int.json",ref resultForIntUseCas);


            Console.WriteLine("Result for string use case :"+ resultForStringUSeCase);
            Console.WriteLine("Result for int use case :"+ resultForIntUseCas);



            //Emitter UseCase
            var tokenSource = new CancellationTokenSource();

            var useCase = new EmitterUseCase();
            useCase.Trigger(tokenSource.Token);


            
            


        }

        


    }
}
