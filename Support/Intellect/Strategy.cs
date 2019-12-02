using Support.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Support.Intellect
{
    public interface IStrategy
    {
        void AssignRequests();
    }

    public class StrategyFIFO : IStrategy
    {
        public void AssignRequests()
        {
            //Стратегия - FIFO с максимальной производительностью (задействовано как можно больше сотрудников, по иерархии)
            try
            {
                DB.DbMutex.WaitOne();
                using (DB Db = DB.Create())
                {
                    using (var dbContextTransaction = Db.Database.BeginTransaction())
                    {
                        var tm = Db.SupportParameter.FirstOrDefault(p => p.ParameterName == "Tm").ParameterValue;
                        DateTime dTm = System.DateTime.Now.AddSeconds(-tm);
                        var td = Db.SupportParameter.FirstOrDefault(p => p.ParameterName == "Td").ParameterValue;
                        DateTime dTd = System.DateTime.Now.AddSeconds(-td);

                        //Сколько заявок
                        var reqs = (from req in Db.ClientRequest
                                    where req.State == Models.ClientRequest.ClientRequestState.New
                                    select req).OrderBy(r => r.CreatedOn).ToList();
                        if (reqs.Count > 0)
                        {
                            //Смотрим сколько свободных сотрудников
                            var empls = (from empl in Db.Employee
                                         where !empl.IsFired &&
                                            !Db.ClientRequest.Any(req => (req.State == Models.ClientRequest.ClientRequestState.Process) && (req.Executor == empl))
                                         select empl).ToList();
                            if (empls.Count > 0)
                            {
                                //Из них операторов
                                var opers = empls.Where(e => e.Title == Models.Employee.TitleEnum.Operator).ToList();
                                //менеджеров
                                var mans = empls.Where(e => e.Title == Models.Employee.TitleEnum.Manager).ToList();
                                //директоров
                                var dirs = empls.Where(e => e.Title == Models.Employee.TitleEnum.Director).ToList();

                                //Число заявок, которые могут быть отработаны директорами
                                int canDirs = Math.Min(reqs.Count(r => r.CreatedOn < dTd), dirs.Count);
                                //Число заявок, которые могут быть отработаны менеджерами
                                int canMans = Math.Min(reqs.Count(r => r.CreatedOn < dTm), mans.Count);

                                //Число заявок которые должны быть отработаны операторами
                                int needOpersCnt = Math.Min(reqs.Count, opers.Count);
                                //Число заявок которые должны быть отработаны директорами
                                int needDirsCnt = Math.Min(Math.Max(0, reqs.Count - (needOpersCnt + canMans)), canDirs);
                                //Число заявок которые должны быть отработаны менеджерами
                                int needMansCnt = Math.Min(Math.Max(0, reqs.Count - (needOpersCnt + needDirsCnt)), canMans);

                                foreach (var req in reqs.Take(needOpersCnt + needDirsCnt + needMansCnt).OrderByDescending(r => r.CreatedOn).ToList())
                                {
                                    //Пробегаем по всем заявкам по порядку создания
                                    if (needOpersCnt > 0)
                                    {
                                        //Назначаем задачу оператору
                                        opers[--needOpersCnt].AssignRequest(Db, req.ID);
                                        continue;
                                    }
                                    if (td > tm)
                                    {
                                        if (needMansCnt > 0)
                                        {
                                            //Назначаем задачу менеджеру
                                            mans[--needMansCnt].AssignRequest(Db, req.ID);
                                            continue;
                                        }
                                        else if (needDirsCnt > 0)
                                        {
                                            //Назначаем задачу директору
                                            dirs[--needDirsCnt].AssignRequest(Db, req.ID);
                                            continue;
                                        }
                                    }
                                    else
                                    {                                        
                                        if (needDirsCnt > 0)
                                        {
                                            //Назначаем задачу директору
                                            dirs[--needDirsCnt].AssignRequest(Db, req.ID);
                                            continue;
                                        }
                                        else if (needMansCnt > 0)
                                        {
                                            //Назначаем задачу менеджеру
                                            mans[--needMansCnt].AssignRequest(Db, req.ID);
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                        dbContextTransaction.Commit();
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