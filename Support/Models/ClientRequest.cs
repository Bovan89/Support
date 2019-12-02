using Support.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Support.Models
{
    public class ClientRequest
    {   
        public enum ClientRequestState : int
        {
            New = 0,
            Process = 1,
            Complete = 2,
            Cancel = 3
        }
                
        public int ID { get; set; }
        public string Text { get; set; }
        public DateTime CreatedOn { get; set; }        
        public ClientRequestState State { get; set; }
        public int ProcessTime { get; set; }
        public DateTime? StartTime { get; set; }

        public Employee Executor { get; set; }

        public void Insert(DB db)
        {            
            db.ClientRequest.Add(this);
            db.SaveChanges();

            //Событие появления заявки в системе
            OnInsert(this);
        }

        public void Cancel(DB db)
        {
            State = ClientRequest.ClientRequestState.Cancel;            
            db.SaveChanges();
        }

        //Случайное заполнение времени выполнения заявки
        public void FillProcessTime(DB db)
        {
            var min = db.SupportParameter.FirstOrDefault(p => p.ParameterName == "E0").ParameterValue;
            var max = db.SupportParameter.FirstOrDefault(p => p.ParameterName == "E1").ParameterValue;

            //Длительность обработки заявки переданной на выполнение
            ProcessTime = new Random().Next(Math.Min(min, max), Math.Max(min, max));
        }

        public delegate void ClientRequestDelegate(ClientRequest sender);

        //Событие вставки заявки
        public static event ClientRequestDelegate OnInsert;
    }
}