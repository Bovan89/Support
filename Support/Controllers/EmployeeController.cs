using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Support.DAL;
using Support.Models;

namespace Support.Controllers
{
    public class EmployeeController : BaseWebAPIController
    {
        // GET: api/Employee
        public IQueryable GetEmployees()
        {
            var empls = (from empl in db.Employee
                         join req in db.ClientRequest.Where(r => r.State == ClientRequest.ClientRequestState.Process) on empl equals req.Executor into gj
                         from subreq in gj.DefaultIfEmpty()
                         join reqs in db.ClientRequest.Where(r => r.State == ClientRequest.ClientRequestState.Complete) on empl equals reqs.Executor into g
                         select new
                         {
                             ID = empl.ID,
                             Name = empl.Name,
                             Title = empl.Title,
                             IsBusy = subreq != null,
                             CurrentRequest = subreq,
                             IsFired = empl.IsFired,
                             ReqCnt = g.Count()
                         }
                        ).OrderByDescending(e => e.ReqCnt);

            return empls;            
        }

        // GET: api/Employee
        public IQueryable GetEmployees(Employee.FilterEnum status, bool withThisState = true)
        {
            switch (status)
            {
                case Employee.FilterEnum.Busy:
                    var emplsB = (from empl in db.Employee
                             join req in db.ClientRequest.Where(r => r.State == ClientRequest.ClientRequestState.Process) on empl equals req.Executor into gj
                             from subreq in gj.DefaultIfEmpty()
                             where !((subreq == null && status != Employee.FilterEnum.Busy || subreq != null && status == Employee.FilterEnum.Busy) ^ withThisState)
                             join reqs in db.ClientRequest.Where(r => r.State == ClientRequest.ClientRequestState.Complete) on empl equals reqs.Executor into g
                             select new
                             {
                                 ID = empl.ID,
                                 Name = empl.Name,
                                 Title = empl.Title,
                                 IsBusy = subreq != null,
                                 CurrentRequest = subreq,
                                 IsFired = empl.IsFired,
                                 ReqCnt = g.Count()
                             }
                       ).OrderByDescending(e => e.ReqCnt); ;
                    return emplsB;
                case Employee.FilterEnum.Fired:
                    var emplsF = (from empl in db.Employee.Where(e => !((e.IsFired && status == Employee.FilterEnum.Fired || !e.IsFired && status != Employee.FilterEnum.Fired) ^ withThisState))
                                 join req in db.ClientRequest.Where(r => r.State == ClientRequest.ClientRequestState.Process) on empl equals req.Executor into gj
                                 from subreq in gj.DefaultIfEmpty()
                                 join reqs in db.ClientRequest.Where(r => r.State == ClientRequest.ClientRequestState.Complete) on empl equals reqs.Executor into g
                                 select new
                                 {
                                     ID = empl.ID,
                                     Name = empl.Name,
                                     Title = empl.Title,
                                     IsBusy = subreq != null,
                                     CurrentRequest = subreq,
                                     IsFired = empl.IsFired,
                                     ReqCnt = g.Count()
                                 }
                       ).OrderByDescending(e => e.ReqCnt); ;
                    return emplsF;
            }

            return null;
        }
        
        // GET: api/Employee/5
        [ResponseType(typeof(Employee))]
        public IHttpActionResult GetEmployee(int id)
        {
            Employee employee = db.Employee.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // PUT: api/Employee/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmployee(int id, Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employee.ID)
            {
                return BadRequest();
            }

            db.Entry(employee).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Employee
        [ResponseType(typeof(Employee))]
        public IHttpActionResult PostEmployee(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Employee.Add(employee);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = employee.ID }, employee);
        }

        // DELETE: api/Employee/5
        [ResponseType(typeof(Employee))]
        public IHttpActionResult DeleteEmployee(int id)
        {
            Employee employee = db.Employee.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            employee.IsFired = true;
            db.SaveChanges();

            return Ok(employee);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EmployeeExists(int id)
        {
            return db.Employee.Count(e => e.ID == id) > 0;
        }
    }
}