using Quartz;
using Support.DAL;
using Support.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Support.Intellect
{
    public class RequestProcessor
    {
        private static RequestProcessor instance;
        public static IStrategy RequestProcessorStrategy { get; set; }

        public static RequestProcessor GetInstance()
        {
            if (instance == null)
            {
                instance = new RequestProcessor();
            }
            return instance;
        }

        private RequestProcessor()
        {
            //Подписались на создание заявок
            ClientRequest.OnInsert += ClientRequest_OnInsert;
        }
        
        private void ClientRequest_OnInsert(ClientRequest request)
        {
            try
            {
                //В системе появилась новая заявка
                DB.DbMutex.WaitOne();
                using (DB Db = new DB())
                {
                    //Поиск свободного оператора
                    var freeEmployee = (from empl in Db.Employee
                                        where empl.Title == Models.Employee.TitleEnum.Operator && !empl.IsFired &&
                                            !Db.ClientRequest.Any(req => (req.State == Models.ClientRequest.ClientRequestState.Process) && (req.Executor == empl))
                                        select empl).FirstOrDefault();
                    if (freeEmployee != null)
                    {
                        freeEmployee.AssignRequest(Db, request.ID);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DB.DbMutex.ReleaseMutex();
            }
        }
        
        public void Process()
        {
            try
            {
                //Бесконечный цикл обработки заявок
                while (true)
                {                
                    //Сначала освобождаем ресурсы, завершаем заявки
                    FreeResources();

                    //Назначаем заявки свободным сотрудникам
                    RequestProcessorStrategy.AssignRequests();                
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void FreeResources()
        {
            //Завки у которых время выполнения превысило время, требуемое для выполнения - переходят в статус выполнено
            try
            {
                DB.DbMutex.WaitOne();
                using (DB Db = new DB())
                {
                    var query = (from req in Db.ClientRequest
                                 where req.State == Models.ClientRequest.ClientRequestState.Process && req.StartTime != null &&
                                    (req.ProcessTime == 0 || req.StartTime <= System.Data.Entity.DbFunctions.AddSeconds(System.DateTime.Now, -req.ProcessTime))
                                 select req).ToList();
                    foreach (var item in query)
                    {
                        item.State = Models.ClientRequest.ClientRequestState.Complete;
                        Db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DB.DbMutex.ReleaseMutex();
            }
        }
    }
}