using Newtonsoft.Json;
using HttpMultipartParser;
using System.Net;
using System.Text;
using System.IO;

namespace LeaveManagementSystem
{
    public class HttpServer
    {
        public int Port = 8080;
        private DataAccess db = new DataAccess();
        private HttpListener listener;
        

        public void Start(int? port = null)
        {
            if (port != null)
                Port = port.Value;

            listener = new HttpListener();
            listener.Prefixes.Add("http://*:" + Port.ToString() + "/");
            listener.Start();
            Receive();
        }

        public void Stop()
        {
            listener.Stop();
        }

        private void Receive()
        {
            listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
        }

        private void ListenerCallback(IAsyncResult result)
        {
            if (listener.IsListening)
            {
                var context = listener.EndGetContext(result);
                var request = context.Request;

                Console.WriteLine($"Method: {request.HttpMethod}  URL: {request.Url.AbsolutePath}");

                switch (request.HttpMethod)
                {
                    case "GET": Get(context); break;
                    case "POST": Post(context); break;
                    default:
                        context.Response.StatusCode = 405;
                        context.Response.Close();
                        Console.WriteLine("Invalid method");
                        break;
                }

                //context.Request.InputStream.Close();
                //context.Response.OutputStream.Close();
                

                Receive();
            }
        }
        private void Respond(string json, HttpListenerResponse response)
        {
            byte[] data = Encoding.UTF8.GetBytes(json);

            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "application/json; charset=utf-8";
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.ContentLength64 = data.Length;
            response.OutputStream.Write(data, 0, data.Length);
            response.OutputStream.Flush();
            response.OutputStream.Dispose();
            response.OutputStream.Close();
        }

        private void Get(HttpListenerContext context)
        {
            switch (context.Request.Url.AbsolutePath)
            {

                case "/":
                    Console.WriteLine("Devaul tpath");
                    context.Response.StatusCode = 200;
                    context.Response.Close();
                    break;

                case "/leave":
                    Console.WriteLine("/leave");


                    string data = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding).ReadToEnd();
                
                    Employee emp = JsonConvert.DeserializeObject<Employee>(data);
                    
                    Console.WriteLine($"Name:{emp.FirstName}  Surname:{emp.LastName}");

                    var table = db.RetrieveData("Employee", $"FirstName = '{emp.FirstName}' AND LastName = '{emp.LastName}'");
                   

                    if(table.Rows.Count == 0)
                    {                    
                        Console.WriteLine("No employee found");
                        Respond(JsonConvert.SerializeObject("EmployeeNotFound"), context.Response);
                        break;
                    }
                    //get the first matching employee
                    var row = table.Rows[0];

                    var daysLeft = (int)row["LeaveDaysLeft"];
                    Respond(JsonConvert.SerializeObject(daysLeft), context.Response);

                    break;

                case "/employees":
                    Console.WriteLine("/employees");
                    break;

                case "leave-requests":
                    Console.WriteLine("/leave-requests");
                    break;

                default:
                    context.Response.StatusCode = 404;
                    context.Response.Close();
                    break;
            }

            //if (context.Request.HasEntityBody)
            //{
            //    Console.WriteLine("Content type: {0}", context.Request.ContentType);

            //    string data = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding).ReadToEnd();

                
            //    //Console.WriteLine(data);


            //    Employee emp = JsonConvert.DeserializeObject<Employee>(data);

            //    Console.WriteLine($"Name:{emp.Name}  Surname:{emp.Surname}");
                
                

            //    //HttpMultipartParser parser = new HttpMultipartParser(context.Request.InputStream, context.Request.ContentType);
            //    //var parser = MultipartFormDataParser.Parse(context.Request.InputStream);

            //    //foreach (var param in parser.Parameters)
            //    //{
            //    //    Console.WriteLine($"Param: {param.Name} = {param.Data}");
            //    //}

            //}
        
         
        }
        void WriteBody(HttpListenerRequest request)
        {
            //if (request.HasEntityBody)

            var body = request.InputStream;
            var encoding = request.ContentEncoding;
            var reader = new StreamReader(body, encoding);
            if (request.ContentType != null)
            {
                Console.WriteLine("Client data content type {0}", request.ContentType);
            }
            Console.WriteLine("Client data content length {0}", request.ContentLength64);

            Console.WriteLine("Start of data:");
            string s = reader.ReadToEnd();
            
            Console.WriteLine(s);
            Console.WriteLine("End of data");
            reader.Close();
            body.Close();
        }

        private void Post(HttpListenerContext context)
        {
            var request = context.Request;

            switch (request.Url.AbsolutePath)
            {
                case "/":
                    Console.WriteLine("Default path");
                    context.Response.StatusCode = 200;
                    context.Response.Close();
                    break;
                case "/form-data":
                    
                    Console.WriteLine("/form-data");

                    string data = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding).ReadToEnd();

                    Console.WriteLine($"Recieved:{data}");
                    
                    Form form = JsonConvert.DeserializeObject<Form>(data);

                    form.WriteToDB();


                    Respond(JsonConvert.SerializeObject("Succes"), context.Response);
                    break;

                default:
                    context.Response.StatusCode = 404;
                    context.Response.Close();
                    break;
            }


            //if (context.Request.HasEntityBody)
            //{
            //    Console.WriteLine("Content type: {0}", context.Request.ContentType);

            //    using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            //    {
            //        Console.WriteLine("Client data: {0}", reader.ReadToEnd());
            //    }
            //}

        }
    }

    public class Leave
    {
        //public int? LeaveID { get; set; }
        public int EmployeeID { get; set; }
        public string LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysTaken { get; set;}
        public string Reason { get; set; }

        public Leave(int employeeID, string leaveType, DateTime startDate, DateTime endDate, int daysTaken, string reason)
        {
            //LeaveID = leaveID;
            EmployeeID = employeeID;
            LeaveType = leaveType;
            StartDate = startDate;
            EndDate = endDate;
            DaysTaken = daysTaken;
            Reason = reason;
        }

        public override string ToString()
        {
            return $"EmployeeID: {EmployeeID} LeaveType: {LeaveType} StartDate: {StartDate} EndDate: {EndDate} DaysTaken: {DaysTaken} Reason: {Reason}";
        }

    }
    public class Employee
    {
        public int EmployeeID { get; set; }
        public int LeaveDaysLeft { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
 
        public Employee(int employeeID, int leaveDaysLeft, string firstName, string lastName)
        {
            EmployeeID = employeeID;
            LeaveDaysLeft = leaveDaysLeft;
            FirstName = firstName;
            LastName = lastName;
        }

        public override string ToString()
        {
            return $"EmployeeID: {EmployeeID} ";
        }
    }
    public class Form
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime LeaveStartDate { get; set; }
        public DateTime LeaveEndDate { get; set; }
        public string LeaveType { get; set; }
        public string Message { get; set; }

        public Form(string firstName, string lastName, DateTime leaveStartDate, DateTime leaveEndDate, string message)
        {
            FirstName = firstName;
            LastName = lastName;
            LeaveStartDate = leaveStartDate;
            LeaveEndDate = leaveEndDate;
            Message = message;
        }

        public void WriteToDB()
        {
            DataAccess db = new DataAccess();

            //Console.WriteLine($"Name:{emp.FirstName}  Surname:{emp.LastName}");

            var table = db.RetrieveData("Employee", $"FirstName = '{this.FirstName}' AND LastName = '{this.LastName}'");


            if (table.Rows.Count == 0)
            {
                Console.WriteLine("No employee found");
                return;            
            }

            //get the first matching employee
            var employeeID = (int)table.Rows[0]["EmployeeID"];

            var leave = new Leave(employeeID, LeaveType, LeaveStartDate, LeaveEndDate, (int)LeaveEndDate.Subtract(LeaveStartDate).TotalDays, Message);


            Console.WriteLine(leave.ToString());



            db.InsertObject(leave);


        }


        public override string ToString()
        {
            return $"Firstname: {FirstName} ";
        }
    }




    internal class Program
    {
        private static bool keepRunning = true;


        
        public static void Go()
        {
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                Program.keepRunning = false;
            };

            Console.WriteLine("Starting HTTP listener...");

            var httpServer = new HttpServer();
            httpServer.Start();

            while (Program.keepRunning) { }

            httpServer.Stop();

            Console.WriteLine("Exiting gracefully...");
        }


        static void Main(string[] args)
        {
    
            Go();
        }
    }
}