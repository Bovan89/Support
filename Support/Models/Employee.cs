using Support.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Support.Models
{
    public class Employee
    {
        public enum FilterEnum : int
        {       
            Busy = 0,
            Fired = 1
        }

        public enum TitleEnum : int
        {
            Operator = 0,
            Manager = 1,
            Director = 2            
        }

        public int ID { get; set; }
        public string Name { get; set; }        
        public TitleEnum Title { get; set; }                
        public bool IsFired { get; set; }

        //Назначение заявки сотруднику
        public void AssignRequest(DB db, int requestId)
        {            
            ClientRequest req = db.ClientRequest.Find(requestId);
            if (req.State == ClientRequest.ClientRequestState.New)
            {
                req.State = ClientRequest.ClientRequestState.Process;
                req.Executor = this;
                req.StartTime = DateTime.Now;
                req.FillProcessTime(db);
                db.SaveChanges();
            }
        }
    }
}