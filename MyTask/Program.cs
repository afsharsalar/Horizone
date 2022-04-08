using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

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





   
    class EmitterUseCase
    {
        public event EventHandler Start;
        public event EventHandler Error;
        public event EventHandler Next;
        public event EventHandler Stop;


        

        public void Trigger()
        {
            try
            {



                OnStart(EventArgs.Empty);
                var intUseCase = new IntUseCase();
                var result = "";
                intUseCase.Execute("https://kaverin-ddb.firebaseio.com/interview/useCase/int.json",ref result);
                Console.WriteLine("Result is:"+result);

                OnNext(EventArgs.Empty);

            }
            catch (Exception e)
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

        public void OnStop(EventArgs e)
        {
            Console.WriteLine("Stopped at:" + DateTime.Now.ToString("HH:mm:ss tt"));
            Stop?.Invoke(this, e);
        }

        public void Cancel()
        {
            Console.WriteLine("Cancel callback");
            // Perform object cancellation here.
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

            
            var timer = new Timer(TimeSpan.FromSeconds(5).TotalMilliseconds); // Set the time (5 second in this case)
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(CallEmitter);
            timer.Start();

            Console.WriteLine("Emitter UseCase start in 5 second, if you want to cancel it enter 0");
            var input=Console.ReadLine();
            if (input == "0")
            {
                timer.Stop();
            }
        }

        private static void CallEmitter(object sender, ElapsedEventArgs e)
        {
            var instance = new EmitterUseCase();
            instance.Trigger();
        }


    }
}
